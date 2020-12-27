namespace Ooak.Testing.Models
{
    public class IntWrapper
    {
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.Always)] // Sadly, no equivalent for STJ...
        public int IntValue { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is IntWrapper other && object.Equals(this.IntValue, other.IntValue);
        }
    }
}
