using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Extensions;

namespace OCC.Passports.Common.Domains
{
    public class PassportScope
    {
        internal readonly object Lock = new object();

        [JsonIgnore]
        private readonly IPassport _passport;

        [JsonProperty(Order = 1)]
        public string Name { get; private set; }
        [JsonProperty(Order = 0)]
        public Guid Id { get; private set; }
        [JsonIgnore]
        public PassportScope Parent { get; private set; }
        [JsonProperty(Order = 2)]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty(Order = 3)]
        public readonly List<IHistory> History = new List<IHistory>();

        protected PassportScope(PassportScope scope)
        {
            Name = scope.Name;
            Timestamp = scope.Timestamp;
            Id = scope.Id;
        }

        public PassportScope(IPassport passport, string name, Guid id, PassportScope parent)
        {
            _passport = passport;
            Name = name;
            Timestamp = new DateTimeOffset(DateTime.UtcNow);
            Id = id;
            Parent = parent;
        }

        public void RecordParameters<T>(Expression<Func<T>> expression, string operation = null)
        {
            var nvp = GetNameAndValue(expression);
            if (nvp == null)
                return;

            var name = nvp.Item1;
            var value = nvp.Item2;

            lock (Lock)
            {
                var recording = new RecordingOfParameters(name, value);
                History.Add(recording);
            }
        }

        public void Record<T>(Expression<Func<T>> expression, string operation = null
                                , bool debug = true
                                , [System.Runtime.CompilerServices.CallerMemberNameAttribute] string memberName = ""
                                , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
                             )
        {
            var nvp = GetNameAndValue(expression);
            if (nvp == null)
                return;

            var name = nvp.Item1;
            var value = nvp.Item2;

            lock (Lock)
            {
                var recording = new Recording(name, value, operation, memberName, sourceFilePath, sourceLineNumber);

                History.Add(recording);

                if (debug)
                    _passport.Debug("{Name} has changed to {@Value}", new object[]
                    {
                        name,
                        value
                    });
            }
        }

        public string Serialize(DateTimeOffset? timestamp = null)
        {
            var result = string.Empty;
            lock (Lock)
            {
                if (!timestamp.HasValue)
                {
                    result = JsonConvert.SerializeObject(this);
                }
                else
                {
                    var scope = new PassportScope(this);
                    scope.History.AddRange(History.Where(x => x.Timestamp <= timestamp).ToList());
                    result = JsonConvert.SerializeObject(scope);
                }
            }
            return result;
        }

        public static Tuple<string, TSource> GetNameAndValue<TSource>(Expression<Func<TSource>> sourceExpression)
        {
            Tuple<string, TSource> result = null;
            Func<MemberExpression, Tuple<String, TSource>> process = delegate(MemberExpression memberExpression)
            {
                var constantExpression = (ConstantExpression)memberExpression.Expression;
                var name = memberExpression.Member.Name;
                var value = ((FieldInfo)memberExpression.Member).GetValue(constantExpression.Value);
                return new Tuple<string, TSource>(name, (TSource)value);
            };

            var expression = sourceExpression.Body;
            if (expression is MemberExpression)
            {
                result = process((MemberExpression)sourceExpression.Body);
            }
            else if (expression is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)sourceExpression.Body;
                result = process((MemberExpression)unaryExpression.Operand);
            }

            return result;
        }

        [Serializable]
        public class RecordingOfParameters : IHistory
        {
            [JsonProperty(Order = 1)]
            public string Name { get; set; }
            [JsonProperty(Order = 2)]
            public JToken Value { get; set; }
            [JsonProperty(Order = 3)]
            public DateTimeOffset Timestamp { get; set; }

            public RecordingOfParameters(string name, object value)
            {
                Timestamp = new DateTimeOffset(DateTime.UtcNow);
                Name = name;
                Value = JToken.Parse(JsonConvert.SerializeObject(value, Formatting.None));
            }
        }

        [Serializable]
        public class Recording : IHistory
        {
            [JsonProperty(Order = 1)]
            public string Name { get; set; }
            [JsonProperty(Order = 2)]
            public string Operation { get; set; }
            [JsonProperty(Order = 3)]
            public JToken Value { get; set; }
            [JsonProperty(Order = 4)]
            public DateTimeOffset Timestamp { get; set; }
            [JsonProperty(Order = 5)]
            public string MemberName { get; set; }
            [JsonProperty(Order = 6)]
            public string SourceFilePath { get; set; }
            [JsonProperty(Order = 7)]
            public int SourceLineNumber { get; set; }

            public Recording(string name
                            , object value
                            , string operation = null
                            , string memberName = null
                            , string sourceFilePath = null
                            , int sourcelineNumber = 0
                )
            {
                Timestamp = new DateTimeOffset(DateTime.UtcNow);
                MemberName = memberName;
                SourceFilePath = sourceFilePath;
                SourceLineNumber = sourcelineNumber;
                Name = name;
                Value = JToken.Parse(JsonConvert.SerializeObject(value, Formatting.None));
                Operation = operation;
            }
        }
    }
}
