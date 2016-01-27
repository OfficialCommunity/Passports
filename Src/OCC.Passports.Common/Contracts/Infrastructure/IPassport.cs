using OCC.Passports.Common.Domains;
using OCC.Passports.Common.Infrastructure;
using System;

namespace OCC.Passports.Common.Contracts.Infrastructure
{
    public interface IPassport
    {
        object SessionId { get; set; }
        Guid PassportId { get; set; }
        PassportScope Scope { get; }
        StandardResponse<bool> Stamp(MessageContext messageContext, bool includeCallContext, bool includeSnapshot);
        StandardResponse<bool> StampException(Exception e);
        void PushScope(string name);
        void PopScope();
    }
}
