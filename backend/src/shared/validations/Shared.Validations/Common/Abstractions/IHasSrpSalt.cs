namespace Shared.Validations.Common.Abstractions
{
    public interface IHasSrpSalt
    {
        public string SrpSalt { get; }
    }
}