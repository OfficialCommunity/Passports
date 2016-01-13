using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Extensions;
using PostSharp.Aspects;

namespace OCC.Passports.Common.Aspects
{
    [Serializable]
    public sealed class UsePassportAttribute : OnMethodBoundaryAspect
    {
        private string[] _parameterNames;
        private readonly string _sessionParameter;
        private int _sessionParameterIndex = -1;

        public UsePassportAttribute(string sessionParameter = null)
        {
            _sessionParameter = sessionParameter;
        }

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            _parameterNames = method.GetParameters().Select(p => p.Name).ToArray();
            if (_sessionParameter != null)
            {
                _sessionParameterIndex = Array.IndexOf(_parameterNames, _sessionParameter);
            }
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            var instance = args.Instance as IHasPassport;
            if (instance == null) return;

            var thisPassport = instance.Passport;
            if (thisPassport == null) return;

            var scopeName = args.ScopeName();

            thisPassport.PushScope(scopeName);

            IDictionary<string, object> parameters = new Dictionary<string, object>();

            for (var i = 0; i < args.Arguments.Count; i++)
            {
                if (_sessionParameterIndex == i)
                {
                    thisPassport.SessionId = args.Arguments[i];
                }
                else
                {
                    parameters[_parameterNames[i]] = args.Arguments[i] ?? "null";
                }
            }

            thisPassport.PassportId = Guid.NewGuid();
            thisPassport.Scope.Record(() => parameters, Constants.PassportScope.Enter);
        }
    }
}
