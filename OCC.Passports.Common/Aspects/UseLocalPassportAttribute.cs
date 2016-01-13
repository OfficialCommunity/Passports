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
    public sealed class UseLocalPassportAttribute : OnMethodBoundaryAspect
    {
        private string[] _parameterNames;
        private string[] _passportParameters;

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            var parameters = method.GetParameters().ToList();
            _parameterNames = parameters.Select(p => p.Name).ToArray();

            // Safeguard - we don't want to dump the passort incase it is passed as parameter
            // Using AutoFac the same Passport instance will be passed to each service/parameter
            //      in the constructor using InstancePerHttpRequest lifetime
            _passportParameters = parameters.Where(x => x.ParameterType == typeof (IPassport))
                                            .Select(p => p.Name).ToArray()
                                            ;
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
                if (!_passportParameters.Contains(_parameterNames[i]))
                {
                    parameters[_parameterNames[i]] = args.Arguments[i] ?? "null";
                }
            }
            thisPassport.Scope.Record(() => parameters, Constants.PassportScope.Enter);
        }
    }
}