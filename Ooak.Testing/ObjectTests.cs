using NUnit.Framework;
using Ooak.Testing.Converters;
using Ooak.Testing.Models;

namespace Ooak.Testing
{
    public class ObjectTests
    {
        [Test]
        public void TestWrappers()
        {
            Helpers.TestSuccess<TypeUnion<IntWrapper, StringWrapper>, IntWrapper, StringWrapper>(
                "{\"StringValue\": \"kickban\", \"IntValue\": 42}",
                new TypeUnion<IntWrapper, StringWrapper>.Both(new IntWrapper { IntValue = 42 }, new StringWrapper { StringValue = "kickban" }),
                "AllOf");
        }

        [Test]
        public void TestComposite()
        {
            Helpers.TestSuccess<CompositeModel>(
                "{\"Child\": 42}",
                new CompositeModel { Child = new TypeUnion<int, IntWrapper>.Left(42) });
            Helpers.TestSuccess<CompositeModel>(
                "{\"Child\": {\"IntValue\": 42}}",
                new CompositeModel { Child = new TypeUnion<int, IntWrapper>.Right(new IntWrapper { IntValue = 42 }) });
            Helpers.TestFailure<CompositeModel>("{\"Child\":true}");
        }

        [Test]
        public void TestDifferences()
        {
            // Default System.Text.Json doesn't crash deserialization if a non-null property is missing (no equivalent for JsonRequired) https://github.com/dotnet/runtime/issues/1256
            Helpers.TestSuccess<TypeUnion<IntWrapper, StringWrapper>>(
                "{\"IntValue\": 42}",
                new TypeUnion<IntWrapper, StringWrapper>.Left(new IntWrapper { IntValue = 42 }),
                new IntWrapperStringWrapperSystemTextJsonConverter(),
                Helpers.MakeNewtonsoftJsonConverter<IntWrapper, StringWrapper>("OneOf"));
            Helpers.TestSuccess<TypeUnion<IntWrapper, StringWrapper>>(
                "{\"StringValue\": \"kickban\"}",
                new TypeUnion<IntWrapper, StringWrapper>.Right(new StringWrapper { StringValue = "kickban" }),
                new IntWrapperStringWrapperSystemTextJsonConverter(),
                Helpers.MakeNewtonsoftJsonConverter<IntWrapper, StringWrapper>("OneOf"));
        }
    }
}
