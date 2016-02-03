using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace OCC.Passports.Common.Infrastructure
{
    [Serializable]
    public class StandardError
    {
        public List<string> Errors;

        public StandardError()
        {
            Errors = new List<string>();
        }

        public StandardError(string error)
        {
            Errors = new List<string> { error };
        }

        public StandardError(IEnumerable<string> errors)
        {
            Errors = new List<string>(errors);
        }

        public void Error(string error)
        {
            Errors.Add(error);
        }
        public void Error(IEnumerable<string> errors)
        {
            Errors.AddRange(errors);
        }
    }
}
