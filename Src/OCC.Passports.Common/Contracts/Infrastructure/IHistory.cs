using System;

namespace OCC.Passports.Common.Contracts.Infrastructure
{
    public interface IHistory
    {
        DateTimeOffset Timestamp { get; }
    }
}