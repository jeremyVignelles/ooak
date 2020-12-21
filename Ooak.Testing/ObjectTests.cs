using System;
using System.Linq;
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
        /*[Test]
        public void TestComplex()
        {
            Helpers.TestSuccess("{\"Child\":{\"OneOf\":\"test\"}}",
                new TestComplex {
                    Child = new TypeUnion<int, TestIntString>.Right(new TestIntString
                    {
                        OneOf = new TypeUnion<int, string>.Right("test")
                    })
                });
            Helpers.TestSuccess("{\"Child\":{\"OneOf\":1000000}}",
                new TestComplex
                {
                    Child = new TypeUnion<int, TestIntString>.Right(new TestIntString
                    {
                        OneOf = new TypeUnion<int, string>.Left(1000000)
                    })
                });
            Helpers.TestSuccess("{\"Child\":42}",
                new TestComplex
                {
                    Child = new TypeUnion<int, TestIntString>.Left(42)
                });
            Helpers.TestFailure<TestComplex>("{\"Child\":true}");
        }

#if NET5_0
        [Test]
        public void TestRecord()
        {
            Helpers.TestSuccess("{\"Data\":1.5}", new TestRecord(new TypeUnion<int, double>.Right(1.5)));
            Helpers.TestSuccess("{\"Data\":42}", new TestRecord(new TypeUnion<int, double>.Both(42, 42)));
            Helpers.TestSuccess("{}", new TestRecord(null!));
            Helpers.TestFailure<TestRecord>("{\"Data\":true}");
        }
#endif
        [Test]
        public void TestWrappers()
        {
            Helpers.TestSuccess("{\"Content\":{\"Value\":1}}", new IntStringWrapperContainer { Content = new TypeUnion<IntWrapper,StringWrapper>.Left(new IntWrapper { Value = 1 })});
            Helpers.TestSuccess("{\"Content\":{\"Value\":\"1\"}}", new IntStringWrapperContainer { Content = new TypeUnion<IntWrapper, StringWrapper>.Right(new StringWrapper { Value = "1" }) });

            Helpers.TestSuccess("{\"Content\":{\"Value\":\"2020-01-01T00:00:00Z\"}}", new DateStringWrapperContainer
            {
                Content = new TypeUnion<DateWrapper, StringWrapper>.Both(
                    new DateWrapper { Value = new DateTime(2020,1,1,0,0,0,DateTimeKind.Utc) },
                    new StringWrapper { Value = "2020-01-01T00:00:00Z" })
            });
            Helpers.TestSuccess("{\"Content\":{\"Value\":\"now\"}}", new DateStringWrapperContainer { Content = new TypeUnion<DateWrapper, StringWrapper>.Right(new StringWrapper { Value = "now" }) });
        }*/
    }
}
