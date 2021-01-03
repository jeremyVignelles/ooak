using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ooak.Testing.Converters
{
    /// <summary>
    /// System.Text.Json doesn't have an equivalent of JsonProperty(ItemConverterType=),
    /// so I create a quick and dirty converter here
    /// See discussion here : https://github.com/dotnet/runtime/issues/1562#issuecomment-629372882
    /// </summary>
    public class DictionarySystemTextJsonConverter<TItemConverter, TItemType> : JsonConverter<Dictionary<string, TItemType>>
        where TItemConverter: JsonConverter<TItemType>, new()
    {
        private readonly JsonConverter<TItemType> _converter = new TItemConverter();

        public override Dictionary<string, TItemType>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            Dictionary<string, TItemType> result = new();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return result;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                if (propertyName is null || !reader.Read())
                {
                    throw new JsonException();
                }
                var value = this._converter.Read(ref reader, typeof(TItemType), options);
                if (value is not null)
                {
                    result[propertyName] = value;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, TItemType> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kvp in value)
            {
                writer.WritePropertyName(kvp.Key);
                this._converter.Write(writer, kvp.Value, options);
            }
            writer.WriteEndObject();
        }
    }
}
