using System;

namespace Ooak.Testing.Models
{
    public class DateWrapper
    {
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.Always)] // Sadly, no equivalent for STJ...
        public DateTime DateValue { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DateWrapper other && object.Equals(this.DateValue, other.DateValue);
        }
    }
}
