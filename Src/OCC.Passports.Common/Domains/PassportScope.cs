using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OCC.Passports.Common.Domains
{
    public class PassportScope 
    {
        internal readonly object Lock = new object();

        private readonly Dictionary<string, Entry> _members = new Dictionary<string, Entry>();

        public string Name { get; private set; }

        public PassportScope(string name)
        {
            Name = name;
        }

        public void Record<T>(Expression<Func<T>> expression, string operation = null)
        {
            var nvp = GetNameAndValue(expression);
            if (nvp == null)
                return;

            var name = nvp.Item1;
            var value = nvp.Item2;

            lock (Lock)
            {
                var history = new History(Name, name, value, operation);

                if (!_members.ContainsKey(name))
                {
                    _members.Add(name, new Entry(history));
                }
                else
                {
                    _members[name].AddHistory(history);
                }
            }
        }

        public string Serialize()
        {
            lock (Lock)
            {
                var entries = _members.SelectMany(x => x.Value.ValueHistory)
                                        .OrderBy(x => x.Timestamp)
                                        .ToList()
                                        ;
                return JsonConvert.SerializeObject(entries);
            }
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
        public class Entry
        {
            public readonly List<History> ValueHistory = new List<History>();

            public Entry(History history)
            {
                AddHistory(history);
            }

            public void AddHistory(History history)
            {
                ValueHistory.Add(history);
            }
        }

        [Serializable]
        public class History 
        {
            public string Scope { get; set; }
            public DateTimeOffset Timestamp { get; set; }
            public string Name { get; set; }
            public string Operation { get; set; }
            public JToken Value { get; set; }
            public History(string scope, string name, object value, string operation = null)
            {
                Scope = scope;
                Timestamp = new DateTimeOffset(DateTime.UtcNow);
                Name = name;
                Value = JToken.Parse(JsonConvert.SerializeObject(value, Formatting.None));
                Operation = operation;

                //this[Constants.PassportScope.Entry.Scope] = scope;
                //this[Constants.PassportScope.Entry.Timestamp] = new DateTimeOffset(DateTime.UtcNow);
                //if (!string.IsNullOrWhiteSpace(operation))
                //{
                //    this[Constants.PassportScope.Entry.Operation] = operation;
                //}
                //var serialized = JsonConvert.SerializeObject(value);
                //this[name] = JToken.Parse(serialized);
            }
        }
    }
}
