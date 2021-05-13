using System;
using System.Linq;
#pragma warning disable IDE0018 // Inline variable declaration - I find it clearer that way

namespace Ooak.Testing.Models
{
    public class UnionOfArrayModel
    {
        [Newtonsoft.Json.JsonConverter(typeof(Ooak.NewtonsoftJson.OneOfJsonConverter<int[], DateTime[]>))]
        [System.Text.Json.Serialization.JsonConverter(typeof(Ooak.SystemTextJson.OneOfJsonConverter<int[], DateTime[]>))]
        public TypeUnion<int[], DateTime[]> Values { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is not UnionOfArrayModel other)
            {
                return false;
            }

            int[]? left;
            DateTime[]? right;
            int[]? otherLeft;
            DateTime[]? otherRight;
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
