using System.Data;
using Dapper;
using Nexus.UserManagement.Service.Domain.ValueObjects.Common;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Extensions.Dapper
{
    public class S3KeyResponseTypeHandler : SqlMapper.TypeHandler<S3KeyResponse>
    {
        public override void SetValue(IDbDataParameter parameter, S3KeyResponse? value)
            => parameter.Value = value?.Key; 

        public override S3KeyResponse? Parse(object value)
        {
            if (value is null || value is DBNull) 
                return null;
                
            var stringValue = value.ToString();
            if (string.IsNullOrWhiteSpace(stringValue)) 
                return null;

            var s3Key = S3Key.Restore(stringValue);
            
            return new S3KeyResponse(
                Key: s3Key.FileName,
                Bucket: s3Key.Bucket,
                FolderPath: s3Key.FolderPath
            );
        }
    }
}