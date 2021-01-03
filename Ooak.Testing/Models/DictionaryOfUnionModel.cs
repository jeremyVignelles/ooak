using System;
using System.Collections.Generic;
using System.Linq;
using Ooak.Testing.Converters;

namespace Ooak.Testing.Models
{
    public class DictionaryOfUnionModel
    {
        [Newtonsoft.Json.JsonProperty(ItemConverterType = typeof(Ooak.NewtonsoftJson.OneOfJsonConverter<int, DateTime>))]
        [System.Text.Json.Serialization.JsonConverter(typeof(DictionarySystemTextJsonConverter<Ooak.SystemTextJson.OneOfJsonConverter<int, DateTime>, TypeUnion<int, DateTime>>))]
        public Dictionary<string, TypeUnion<int, DateTime>> Values { get; set; } = new ();

        public override bool Equals(object? obj)
        {
            return obj is DictionaryOfUnionModel other && this.Values.SequenceEqual(other.Values);
        }
    }
}
