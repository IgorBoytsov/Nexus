using System.Text;
using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexus.UserManagement.Service.Application.Abstractions.Clients;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Constants;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Common;
using Shared.Kernel.Errors;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeAvatar
{
    public sealed class ChangeAvatarCommandHandler(
        IFileService fileService, 
        IUserRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ChangeAvatarCommandHandler> logger) : IRequestHandler<ChangeAvatarCommand, Result<Unit>>
    {
        private static readonly IReadOnlyCollection<string> allowedMimeTypes = ["image/jpeg", "image/png", "image/svg+xml"];

        public async Task<Result<Unit>> Handle(ChangeAvatarCommand request, CancellationToken cancellationToken)
        {
            Maybe<User> maybe = await repository.GetByAsync(u => u.Id == request.UserId);

            if (maybe.IsNone)
                return new Error(ErrorCode.NotFound, "Пользователь не найден");

            User user = maybe.Value;
            S3Key? storageKey = user.AvatarKey;

            string mimeType = await DetectMimeTypeAsync(request.File, request.FileName);
            if (!allowedMimeTypes.Contains(mimeType))
                return new Error(AppErrors.Validation, $"Недопустимый тип файла: {mimeType}");

            var avatarKey = S3Key.Create(FileStorageConstants.Bucket, [.. FileStorageConstants.AvatarFolders], request.FileName);
            user.ChangeAvatar(avatarKey);

           Result<Unit> fileSaveResult = await fileService.Upload(avatarKey.Bucket, avatarKey.FolderPath, avatarKey.FileName, mimeType, request.File);

            if (fileSaveResult.IsFailure)
            {
                logger.LogError("Произошла ошибка сохранения файла аватара у пользователя {login}, ошибка: {error}", user.Login, fileSaveResult.StringMessage);
                return new Error(AppErrors.Api, "Ошибка сохранение файла на стороне сервера, пожалуйста повторите попытку позже");
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            if (storageKey != null)
                await fileService.Delete(storageKey.Value.Bucket, storageKey.Value.FolderPath, storageKey.Value.FileName);

            return Unit.Value;
        }

        private static async Task<string> DetectMimeTypeAsync(Stream stream, string fileName)
        {
            if (stream == null || !stream.CanRead)
                return "application/octet-stream";

            const int headerSize = 8;
            byte[] buffer = new byte[headerSize];
            int bytesRead = await stream.ReadAsync(buffer, 0, headerSize);

            string mimeType = DetectMimeType(buffer, bytesRead, fileName);

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            return mimeType;
        }

        private static string DetectMimeType(byte[] buffer, int bytesRead, string fileName)
        {
            if (buffer == null || bytesRead < 3)
                return "application/octet-stream";

            // JPEG: FF D8 FF
            if (bytesRead >= 3 && buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                return "image/jpeg";

            // PNG: 89 50 4E 47 0D 0A 1A 0A
            if (bytesRead >= 8 &&
                buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A)
                return "image/png";

            if (bytesRead > 0)
            {
                string header = Encoding.UTF8.GetString(buffer, 0, bytesRead).TrimStart();
                if (header.StartsWith("<svg", StringComparison.OrdinalIgnoreCase) || 
                    header.Contains("<svg", StringComparison.OrdinalIgnoreCase))
                    return "image/svg+xml";
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg"  => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png"  => "image/png",
                ".svg"  => "image/svg+xml",
                _       => "application/octet-stream"
            };
        }
    }
}