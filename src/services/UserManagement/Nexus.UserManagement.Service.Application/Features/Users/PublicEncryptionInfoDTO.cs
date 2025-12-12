namespace Nexus.UserManagement.Service.Application.Features.Users
{
    public sealed record PublicEncryptionInfoDTO(string ClientSalt, string EncryptedDek);
}