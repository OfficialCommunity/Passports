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
    public sealed class LogParametersToPassportAttribute : OnMethodBoundaryAspect
    {
        private string[] _parameterNames;
        private string[] _passportParameters;

        private readonly string _passportParameter;
        private int _passportParameterIndex = -1;

        public LogParametersToPassportAttribute(string passportParameter = null)
        {
            _passportParameter = passportParameter;
        }

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            var parameters = method.GetParameters().ToList();
            _parameterNames = parameters.Select(p => p.Name).ToArray();

            if (_passportParameter != null)
            {
                _passportParameterIndex = Array.IndexOf(_parameterNames, _passportParameter);
                if (_passportParameterIndex == -1)
                {
                    throw new ArgumentOutOfRangeException(_passportParameter);
                }

                if (typeof (IPassport) != parameters[_passportParameterIndex].ParameterType)
                {
                    throw new InvalidCastException(string.Format("Cannot convert {0} to {1}"
                        , parameters[_passportParameterIndex].ParameterType.AssemblyQualifiedName
                        , typeof (IPassport).AssemblyQualifiedName));
                }
            }
            else
            {
                // Use first IPassport type 

                var firstPassport = parameters.FirstOrDefault(x => typeof(IPassport) == x.ParameterType);
                if (firstPassport == null)
                {
                    throw new Exception("Missing IPassport parameter.");
                }
            
                _passportParameterIndex = Array.IndexOf(_parameterNames, firstPassport.Name);
            }

            // Safeguard - we don't want to attach the Passport to itself
            // For example: If you are using an IOC, then depending on the Lifetime of the Passport
            //  it might be the same passport as the caller
            //      Using AutoFac the same Passport instance will be passed to each service/parameter
            //          in the constructor when using InstancePerHttpRequest lifetime
            _passportParameters = parameters.Where(x => typeof(IPassport).IsAssignableFrom(x.ParameterType))
                                            .Select(p => p.Name).ToArray()
                                            ;
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            IPassport thisPassport = null;
            if (_passportParameterIndex >= 0)
            {
                thisPassport = (IPassport)args.Arguments[_passportParameterIndex];
            }

            if (thisPassport== null)
                return;

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
            thisPassport.Scope.RecordParameters(() => parameters, Constants.PassportScope.Enter);
            thisPassport.Debug("Has been entered", includeContext: true, includeScopes: true);

        }
    }
}