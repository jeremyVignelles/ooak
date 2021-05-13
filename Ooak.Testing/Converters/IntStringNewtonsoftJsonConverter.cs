using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ooak.NewtonsoftJson;

namespace Ooak.Testing.Converters
{
    /// <summary>
    /// The converter that resolves ambiguous deserialization for int,string like "2"
    /// </summary>
    public class IntStringNewtonsoftJsonConverter : OneOfJsonConverter<int, string>
    {
        protected override int DeserializeAsLeft(JToken token, JsonSerializer serializer)
        {
            if (token.Type != JTokenType.Integer)
            {
                throw new JsonSerializationException("This is not an integer type");
            }
            return token.Value<int>();
        }

        protected override string? DeserializeAsRight(JToken token, JsonSerializer serializer)
        {
            if (token.Type != JTokenType.String)
            {
                throw new JsonSerializationException("This is not a string type");
            }

            return token.Value<string>();
        }
    }
}
