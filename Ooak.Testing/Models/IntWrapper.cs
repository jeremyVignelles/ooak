using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Ooak.Testing.Models
{
    public class IntWrapper
    {
        [Newtonsoft.Json.JsonProperty(Required = Required.Always)] // Sadly, no equivalent for STJ...
        public int IntValue { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is IntWrapper other && object.Equals(this.IntValue, other.IntValue);
        }
    }
}
