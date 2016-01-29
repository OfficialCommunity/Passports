using System;
using System.Linq;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Domains;
using System.Collections.Generic;

namespace OCC.Passports.Common.Infrastructure
{
    public class PassportScopeManager
    {
        internal readonly object Lock = new object();

        private readonly Dictionary<string, PassportScope> _scopes = new Dictionary<string, PassportScope>();
        private readonly Stack<PassportScope> _workingStack = new Stack<PassportScope>();

        public PassportScope Push(IPassport passport, string name)
        {
            PassportScope current = null;

            lock (Lock)
            {
                if (passport.Scope != null && _scopes.ContainsKey(name + passport.Scope.Id))
                {
                    current = _scopes[name + passport.Scope.Id];
                }
                else
                {

                    current = new PassportScope(passport, name);
                    _scopes[name+current.Id] = current;
                }
                _workingStack.Push(current);
            }

            return current;
        }

        public PassportScope Pop()
        {
            lock (Lock)
            {
                return _workingStack.Pop();
            }
        }

        public List<string> Serialize()
        {
            lock (Lock)
            {
                return (_scopes.Values.OrderBy(x => x.Timestamp)
                                        .Select(x => x.Serialize())
                                        .ToList()
                );
            }
        }
    }
}
