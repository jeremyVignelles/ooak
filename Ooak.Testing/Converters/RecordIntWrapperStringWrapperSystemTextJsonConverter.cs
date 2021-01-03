#if NET5_0
using Ooak.SystemTextJson;
using Ooak.Testing.Models;

namespace Ooak.Testing.Converters
{
    /// <summary>
    /// The converter that allows deserialization of <see cref="TypeUnion{IntWrapper, StringWrapper}" /> in STJ while
    /// taking into account that STJ just ignores fields that are not present in the JSON.
    /// </summary>
    public class RecordIntWrapperStringWrapperSystemTextJsonConverter : OneOfJsonConverter<RecordIntWrapper, RecordStringWrapper>
    {
        protected override bool LeftIsValid(RecordIntWrapper value)
        {
            // int.MaxValue is the property default value. If it hasn't change, we consider the property absent from the json
            return value.IntValue != int.MaxValue;
        }

        protected override bool RightIsValid(RecordStringWrapper value)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse Not always true ! missing field will still yield null here, that's why we check!
            // See https://github.com/dotnet/runtime/issues/1256
            return value.StringValue is not null;
        }
    }
}
#endif
