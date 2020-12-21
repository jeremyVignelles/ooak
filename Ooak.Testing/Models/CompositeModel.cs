namespace Ooak.Testing.Models
{
    public class CompositeModel
    {
        [Newtonsoft.Json.JsonConverter(typeof(Ooak.NewtonsoftJson.OneOfJsonConverter<int, IntWrapper>))]
        [System.Text.Json.Serialization.JsonConverter(typeof(Ooak.SystemTextJson.OneOfJsonConverter<int, IntWrapper>))]
        public TypeUnion<int, IntWrapper>? Child { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompositeModel other && object.Equals(this.Child, other.Child);
        }
    }
}
