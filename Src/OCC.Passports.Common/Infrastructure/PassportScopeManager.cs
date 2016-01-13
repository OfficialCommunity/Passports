using System.Linq;
using OCC.Passports.Common.Domains;
using System.Collections.Generic;

namespace OCC.Passports.Common.Infrastructure
{
    public class PassportScopeManager
    {
        internal readonly object Lock = new object();

        private readonly Dictionary<string,PassportScope> _scopes = new Dictionary<string, PassportScope>();
        private readonly Stack<PassportScope> _workingStack = new Stack<PassportScope>();

        public PassportScope Push(string name)
        {
            PassportScope current;

            lock (Lock)
            {
                if (_scopes.ContainsKey(name))
                {
                    current = _scopes[name];
                }
                else
                {
                    current = new PassportScope(name);
                    _scopes[name] = current;
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
                return (_scopes.Values.Select(x => x.Serialize()).ToList());
            }
        }
    }
}
