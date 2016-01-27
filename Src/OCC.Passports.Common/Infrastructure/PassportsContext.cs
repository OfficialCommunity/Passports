using System.Collections.Generic;
using OCC.Passports.Common.Contracts.Infrastructure;

namespace OCC.Passports.Common.Infrastructure
{
    public class PassportsContext : Dictionary<string, IContext> 
    {
    }
}
