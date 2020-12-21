#if NET5_0
namespace Ooak.Testing.Models
{
    public record RecordStringWrapper(string StringValue);
    public record RecordIntWrapper(int IntValue);

    public record CompositeRecordWrapper(
        [property: System.Text.Json.Serialization.JsonConverter(typeof(Ooak.SystemTextJson.OneOfJsonConverter<RecordStringWrapper, RecordIntWrapper>))]
        [property: Newtonsoft.Json.JsonConverter(typeof(Ooak.NewtonsoftJson.OneOfJsonConverter<RecordStringWrapper, RecordIntWrapper>))]
        TypeUnion<RecordStringWrapper, RecordIntWrapper> Value
    );
}
#endif
