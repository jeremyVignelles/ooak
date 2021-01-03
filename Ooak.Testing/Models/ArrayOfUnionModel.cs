using System;
using System.Linq;
using Ooak.Testing.Converters;

namespace Ooak.Testing.Models
{
    public class ArrayOfUnionModel
    {
        [Newtonsoft.Json.JsonProperty(ItemConverterType = typeof(Ooak.NewtonsoftJson.OneOfJsonConverter<int, DateTime>))]
        [System.Text.Json.Serialization.JsonConverter(typeof(ArraySystemTextJsonConverter<Ooak.SystemTextJson.OneOfJsonConverter<int, DateTime>, TypeUnion<int, DateTime>>))]
        public TypeUnion<int, DateTime>[] Values { get; set; } = new TypeUnion<int, DateTime>[0];

        public override bool Equals(object? obj)
        {
            return obj is ArrayOfUnionModel other && this.Values.SequenceEqual(other.Values);
        }
    }
}
