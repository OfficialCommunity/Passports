namespace OCC.Passports.Common.Contracts.Infrastructure
{
    public interface IHasPassport
    {
        IPassport Passport { get; }
    }
}
