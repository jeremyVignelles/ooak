#if NET5_0
namespace Ooak.Testing.Models
{
    public record RecordStringWrapper([property: Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.Always)] string StringValue);
    public record RecordIntWrapper([property: Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.Always)] int IntValue);

    public record CompositeRecordWrapper(
        [property: System.Text.Json.Serialization.JsonConverter(typeof(Ooak.SystemTextJson.OneOfJsonConverter<int, RecordIntWrapper>))]
        [property: Newtonsoft.Json.JsonConverter(typeof(Ooak.NewtonsoftJson.OneOfJsonConverter<int, RecordIntWrapper>))]
        TypeUnion<int, RecordIntWrapper> Child
    );
}
#endif
