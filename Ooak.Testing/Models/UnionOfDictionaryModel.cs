using System;
using System.Collections.Generic;
using System.Linq;

namespace Ooak.Testing.Models
{
    public class UnionOfDictionaryModel
    {
        [Newtonsoft.Json.JsonConverter(typeof(Ooak.NewtonsoftJson.OneOfJsonConverter<Dictionary<string, int>, Dictionary<string, DateTime>>))]
        [System.Text.Json.Serialization.JsonConverter(typeof(Ooak.SystemTextJson.OneOfJsonConverter<Dictionary<string, int>, Dictionary<string, DateTime>>))]
        public TypeUnion<Dictionary<string, int>, Dictionary<string, DateTime>> Values { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is not UnionOfDictionaryModel other)
            {
                return false;
            }

            Dictionary<string, int>? left;
            Dictionary<string, DateTime>? right;
            Dictionary<string, int>? otherLeft;
            Dictionary<string, DateTime>? otherRight;
            if (this.Values.TryGetBoth(out left, out right) && other.Values.TryGetBoth(out otherLeft, out otherRight))
            {
                return left.SequenceEqual(otherLeft) && right.SequenceEqual(otherRight);
            }

            if (this.Values.TryGetLeft(out left) && other.Values.TryGetLeft(out otherLeft))
            {
                return left.SequenceEqual(otherLeft);
            }

            if (this.Values.TryGetRight(out right) && other.Values.TryGetRight(out otherRight))
            {
                return right.SequenceEqual(otherRight);
            }

            return false;
        }
    }
}
