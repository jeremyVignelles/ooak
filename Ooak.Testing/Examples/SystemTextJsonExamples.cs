using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using Ooak.SystemTextJson;

namespace Ooak.Testing.Examples
{
    public class SystemTextJsonExamples
    {
        public class TestModel
        {
            // You can put the converter directly on the property:
            [JsonConverter(typeof(OneOfJsonConverter<int, DateTime>))]
            public TypeUnion<int, DateTime> DateOrTimestamp { get; set; }
        }

#if NET5_0
        public record TestRecord(
            // You can put the converter on a record's property
            [property: JsonConverter(typeof(OneOfJsonConverter<int, DateTime>))]
            TypeUnion<int, DateTime> DateOrTimestamp
        );
#endif

        public TypeUnion<int, DateTime> DeserializeMethodSystemTextJson()
        {
            // With STJ, as the deserializer options. Useful for example when the TypeUnion<> is the deserialization root
            return JsonSerializer.Deserialize<TypeUnion<int, DateTime>>("\"2000-01-01T00:00:00Z\"",
                new JsonSerializerOptions()
                {
                    Converters = { new OneOfJsonConverter<int, DateTime>() }
                })!;
        }
        public TypeUnion<int, DateTime> DeserializeMethodNewtonsoftJson()
        {
            // With Newtonsoft.Json, as the deserializer options. Useful for example when the TypeUnion<> is the deserialization root
            // Note: Here, I'm using fully-qualified names with the Newtonsoft namespace. This is only needed because in this example,
            // I have mixed STJ and Newtonsoft in the same file. You shouldn't need to be that verbose in your own code (don't mix serializers!)
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TypeUnion<int, DateTime>>("\"2000-01-01T00:00:00Z\"",
                new Newtonsoft.Json.JsonSerializerSettings()
                {
                    Converters = { new NewtonsoftJson.OneOfJsonConverter<int, DateTime>() }
                })!;
        }

        [Test]
        public void TestExamples()
        {
            var deserialized = JsonSerializer.Deserialize<TestModel>("{\"DateOrTimestamp\": \"2000-01-01T00:00:00Z\"}");
            this.AssertExpectedDate(deserialized!.DateOrTimestamp);
#if NET5_0
            var deserializedRecord = JsonSerializer.Deserialize<TestRecord>("{\"DateOrTimestamp\": \"2000-01-01T00:00:00Z\"}");
            this.AssertExpectedDate(deserialized!.DateOrTimestamp);
#endif
            this.AssertExpectedDate(this.DeserializeMethodSystemTextJson());
            this.AssertExpectedDate(this.DeserializeMethodNewtonsoftJson());
        }

        private void AssertExpectedDate(TypeUnion<int, DateTime> value)
        {
            Assert.IsTrue(value is TypeUnion<int, DateTime>.Right);
        }
    }
}
