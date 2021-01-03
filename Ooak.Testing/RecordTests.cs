#if NET5_0
using NUnit.Framework;
using Ooak.Testing.Converters;
using Ooak.Testing.Models;

namespace Ooak.Testing
{
    public class RecordTests
    {
        [Test]
        public void TestWrappers()
        {
            Helpers.TestSuccess<TypeUnion<RecordIntWrapper, RecordStringWrapper>, RecordIntWrapper, RecordStringWrapper>(
                "{\"StringValue\": \"kickban\", \"IntValue\": 42}",
                new TypeUnion<RecordIntWrapper, RecordStringWrapper>.Both(new RecordIntWrapper(42), new RecordStringWrapper("kickban")),
                "AllOf");
        }

        [Test]
        public void TestComposite()
        {
            Helpers.TestSuccess<CompositeRecordWrapper>(
                "{\"Child\": 42}",
                new CompositeRecordWrapper(new TypeUnion<int, RecordIntWrapper>.Left(42)));
            Helpers.TestSuccess<CompositeRecordWrapper>(
                "{\"Child\": {\"IntValue\": 42}}",
                new CompositeRecordWrapper(new TypeUnion<int, RecordIntWrapper>.Right(new RecordIntWrapper(42))));
            Helpers.TestFailure<CompositeRecordWrapper>("{\"Child\":true}");
        }

        [Test]
        public void TestDifferences()
        {
            Helpers.TestSuccess<TypeUnion<RecordIntWrapper, RecordStringWrapper>>(
                "{\"IntValue\": 42}",
                new TypeUnion<RecordIntWrapper, RecordStringWrapper>.Left(new RecordIntWrapper(42)),
                new RecordIntWrapperStringWrapperSystemTextJsonConverter(),
                Helpers.MakeNewtonsoftJsonConverter<RecordIntWrapper, RecordStringWrapper>("OneOf"));
            Helpers.TestSuccess<TypeUnion<RecordIntWrapper, RecordStringWrapper>>(
                "{\"StringValue\": \"kickban\"}",
                new TypeUnion<RecordIntWrapper, RecordStringWrapper>.Right(new RecordStringWrapper("kickban")),
                new RecordIntWrapperStringWrapperSystemTextJsonConverter(),
                Helpers.MakeNewtonsoftJsonConverter<RecordIntWrapper, RecordStringWrapper>("OneOf"));
        }
    }
}
#endif
