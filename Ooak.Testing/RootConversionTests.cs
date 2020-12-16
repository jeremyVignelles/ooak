using System;
using NUnit.Framework;

namespace Ooak.Testing
{
    public class RootConversionTests
    {
        [Test]
        public void TestRoot()
        {
            Helpers.TestSuccess<TypeUnion<int, double>, int, double> ("2", new TypeUnion<int, double>.Both(2, 2), "AnyOf");
            //Helpers.TestSuccess<TypeUnion<int, string>, int, string>("2", new TypeUnion<int, string>.Left(2), "AnyOf");
            //Helpers.TestSuccess<TypeUnion<int, string>, int, string>("\"2\"", new TypeUnion<int, string>.Right("2"), "AnyOf");
            Helpers.TestSuccess<TypeUnion<int, string>, int, string>("\"kickban\"", new TypeUnion<int, string>.Right("kickban"), "AnyOf");
            //Helpers.TestSuccess<TypeUnion<DateTime, string>, int, double>("\"2020-01-01T00:00:00Z\"", new TypeUnion<DateTime, string>.Both(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), "2020-01-01T00:00:00Z"), "AnyOf");
            //Helpers.TestFailure<TypeUnion<int, string>, int, string>("2.5", "AnyOf");
            Helpers.TestFailure<TypeUnion<int, string>, int, string>("{\"Hello\":\"World\"}", "AnyOf");
        }
    }
}
