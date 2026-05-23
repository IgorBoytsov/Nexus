using System.Data;
using System.Text.Json;
using Dapper;

namespace Shared.Dapper.TypeHandlers
{
    public class JsonListStringHandler : SqlMapper.TypeHandler<List<string>>
    {
        public override List<string>? Parse(object value)
        {
            if (value is string json && !string.IsNullOrWhiteSpace(json))
                return JsonSerializer.Deserialize<List<string>>(json) ?? [];

                return [];
        }

        public override void SetValue(IDbDataParameter parameter, List<string>? value)
        {
            parameter.Value = value != null ? JsonSerializer.Serialize(value) : DBNull.Value;
        }
    }
}