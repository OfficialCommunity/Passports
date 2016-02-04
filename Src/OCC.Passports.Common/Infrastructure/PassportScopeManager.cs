using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Domains;
using System.Collections.Generic;

namespace OCC.Passports.Common.Infrastructure
{
    public class PassportScopeManager
    {
        internal readonly object Lock = new object();

        private readonly Dictionary<string, PassportScope> _scopes = new Dictionary<string, PassportScope>();
        //private readonly Stack<PassportScope> _workingStack = new Stack<PassportScope>();

        public PassportScope Current()
        {
            var callContext = CallContext.LogicalGetData(Constants.Passports.KeyCurrentCallContext);
            if (callContext == null)
                return null;
            lock (Lock)
            {
                return _scopes.ContainsKey((string)callContext) ? _scopes[(string)callContext] : null;
            }
        }

        public PassportScope Push(IPassport passport, string name)
        {
            PassportScope parent = null;

            var parentCallContext = CallContext.LogicalGetData(Constants.Passports.KeyCurrentCallContext);
            if (parentCallContext != null)
                parent = _scopes[(string)parentCallContext];
            var currentId = Guid.NewGuid();
            var key = currentId.ToString("N");
            CallContext.LogicalSetData(Constants.Passports.KeyCurrentCallContext, key);

            var current = new PassportScope(passport, name, currentId, parent);
            lock (Lock)
            {
                _scopes[key] = current;
            }

            return current;
        }

        public List<string> Serialize()
        {
            var results = new List<string>();

            var current = Current();
            var timestamp = current.Timestamp;

            lock (Lock)
            {
                if (current != null)
                {
                    results.Add(current.Serialize());
                    current = current.Parent;
                    while (current != null)
                    {
                        results.Add(current.Serialize(timestamp));
                        current = current.Parent;
                    }
                }
            }

            return results;
        }
    }
}
