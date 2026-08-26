namespace Nexus.UserManagement.Service.Application.Abstractions.Validators
{
    public interface IHasSrpSalt
    {
        public string SrpSalt { get; }
    }
}