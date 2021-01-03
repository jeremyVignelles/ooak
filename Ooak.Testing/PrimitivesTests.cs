using System;
using NUnit.Framework;
using Ooak.Testing.Converters;

namespace Ooak.Testing
{
    public class PrimitiveTests
    {
        [Test]
        public void TestPrimitives()
        {
            Helpers.TestSuccess<TypeUnion<int, DateTime>, int, DateTime>("42", new TypeUnion<int, DateTime>.Left(42), "OneOf");
            Helpers.TestSuccess<TypeUnion<int, DateTime>, int, DateTime>("\"2020-01-01T14:00:00Z\"", new TypeUnion<int, DateTime>.Right(new DateTime(2020, 1, 1, 14, 0, 0, DateTimeKind.Utc)), "OneOf");
            Helpers.TestSuccess<TypeUnion<DateTime, string>, DateTime, string>("\"2020-01-01T00:00:00Z\"", new TypeUnion<DateTime, string>.Both(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), "2020-01-01T00:00:00Z"), "AnyOf");
            Helpers.TestFailure<TypeUnion<int, DateTime>, int, DateTime>("{\"OneOf\":true}", "OneOf");
            Helpers.TestSuccess<TypeUnion<int, string>, int, string>("\"kickban\"", new TypeUnion<int, string>.Right("kickban"), "AnyOf");
            Helpers.TestSuccess<TypeUnion<int, double>, int, double>("2020", new TypeUnion<int, double>.Both(2020, 2020), "AnyOf");
        }

        /// <summary>
        /// Tests the cases where System.Text and Newtonsoft behave differently
        /// </summary>
        [Test]
        public void TestDifferences()
        {
            Helpers.TestSuccess<TypeUnion<int, string>>(
                "2",
                new TypeUnion<int, string>.Left(2),
                Helpers.MakeSystemTextJsonConverter<int, string>("OneOf"),
                new IntStringNewtonsoftJsonConverter());
            Helpers.TestSuccess<TypeUnion<int, string>>(
                "\"2\"",
                new TypeUnion<int, string>.Right("2"),
                Helpers.MakeSystemTextJsonConverter<int, string>("OneOf"),
                new IntStringNewtonsoftJsonConverter());
        }
    }
}
