using NUnit.Framework;
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
            Helpers.TestSuccess<TypeUnion<IntWrapper, StringWrapper>, IntWrapper, StringWrapper>(
                "{\"IntValue\": 42}",
                new TypeUnion<IntWrapper, StringWrapper>.Both(new IntWrapper { IntValue = 42 }, new StringWrapper()), // This is bad... https://github.com/dotnet/runtime/issues/1256
                new TypeUnion<IntWrapper, StringWrapper>.Left(new IntWrapper { IntValue = 42 }),
                "AnyOf");
            Helpers.TestSuccess<TypeUnion<IntWrapper, StringWrapper>, IntWrapper, StringWrapper>(
                "{\"StringValue\": \"kickban\"}",
                new TypeUnion<IntWrapper, StringWrapper>.Both(new IntWrapper { IntValue = 0 }, new StringWrapper { StringValue = "kickban" }), // This is bad... https://github.com/dotnet/runtime/issues/1256
                new TypeUnion<IntWrapper, StringWrapper>.Right(new StringWrapper { StringValue = "kickban" }),
                "AnyOf");
        }
    }
}
