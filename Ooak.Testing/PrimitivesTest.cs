using System;
using NUnit.Framework;

namespace Ooak.Testing
{
    public class TestIntString
    {
        [Newtonsoft.Json.JsonConverter(typeof(Ooak.NewtonsoftJson.OneOfJsonConverter<int, string>))]
        [System.Text.Json.Serialization.JsonConverter(typeof(Ooak.SystemTextJson.OneOfJsonConverter<int, string>))]
        public TypeUnion<int, string>? OneOf { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is TestIntString other && object.Equals(this.OneOf, other.OneOf);
        }
    }

    public class TestComplex
    {
        [Newtonsoft.Json.JsonConverter(typeof(Ooak.NewtonsoftJson.OneOfJsonConverter<int, TestIntString>))]
        [System.Text.Json.Serialization.JsonConverter(typeof(Ooak.SystemTextJson.OneOfJsonConverter<int, TestIntString>))]
        public TypeUnion<int, TestIntString>? Child { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is TestComplex other && object.Equals(this.Child, other.Child);
        }
    }

#if NET5_0
    public record TestRecord([property:Newtonsoft.Json.JsonConverter(typeof(Ooak.NewtonsoftJson.AnyOfJsonConverter<int, double>))] [property:System.Text.Json.Serialization.JsonConverter(typeof(Ooak.SystemTextJson.AnyOfJsonConverter<int, double>))] TypeUnion<int, double> Data);
#endif

    public class IntWrapper
    {
        public int Value { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is IntWrapper other && object.Equals(this.Value, other.Value);
        }
    }

    public class StringWrapper
    {
        public string Value { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is StringWrapper other && object.Equals(this.Value, other.Value);
        }
    }

    public class DateWrapper
    {
        public DateTime Value { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DateWrapper other && object.Equals(this.Value, other.Value);
        }
    }

    public class IntStringWrapperContainer
    {
        [Newtonsoft.Json.JsonConverter(typeof(Ooak.NewtonsoftJson.OneOfJsonConverter<IntWrapper, StringWrapper>))]
        [System.Text.Json.Serialization.JsonConverter(typeof(Ooak.SystemTextJson.OneOfJsonConverter<IntWrapper, StringWrapper>))]
        public TypeUnion<IntWrapper, StringWrapper>? Content { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is IntStringWrapperContainer other && object.Equals(this.Content, other.Content);
        }
    }

    public class DateStringWrapperContainer
    {
        [Newtonsoft.Json.JsonConverter(typeof(Ooak.NewtonsoftJson.AnyOfJsonConverter<DateWrapper, StringWrapper>))]
        [System.Text.Json.Serialization.JsonConverter(typeof(Ooak.SystemTextJson.AnyOfJsonConverter<DateWrapper, StringWrapper>))]
        public TypeUnion<DateWrapper, StringWrapper>? Content { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DateStringWrapperContainer other && object.Equals(this.Content, other.Content);
        }
    }

    public class Tests
    {
        [Test]
        public void TestPrimitives()
        {
            Helpers.TestSuccess("{\"OneOf\":\"test\"}", new TestIntString { OneOf = new TypeUnion<int, string>.Right("test") });
            Helpers.TestSuccess("{\"OneOf\":1000000}", new TestIntString { OneOf = new TypeUnion<int, string>.Left(1000000) });
            Helpers.TestSuccess("{}", new TestIntString { OneOf = null });
            Helpers.TestFailure<TestIntString>("{\"OneOf\":true}");
        }

        [Test]
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
        }
    }
}
