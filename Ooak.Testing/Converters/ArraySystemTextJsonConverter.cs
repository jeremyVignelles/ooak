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
    public class ArraySystemTextJsonConverter<TItemConverter, TItemType> : JsonConverter<TItemType[]>
        where TItemConverter: JsonConverter<TItemType>, new()
    {
        private readonly JsonConverter<TItemType> _converter = new TItemConverter();

        public override TItemType[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            List<TItemType> result = new();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return result.ToArray();
                }

                var value = this._converter.Read(ref reader, typeof(TItemType), options);
                if (value is not null)
                {
                    result.Add(value);
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TItemType[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var v in value)
            {
                this._converter.Write(writer, v, options);
            }
            writer.WriteEndArray();
        }
    }
}
