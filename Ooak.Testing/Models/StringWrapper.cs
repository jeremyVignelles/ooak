namespace Ooak.Testing.Models
{
    public class StringWrapper
    {
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.Always)] // Sadly, no equivalent for STJ...
        public string StringValue { get; set; } = default!;

        public override bool Equals(object? obj)
        {
            return obj is StringWrapper other && object.Equals(this.StringValue, other.StringValue);
        }
    }
}
