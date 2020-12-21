using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Ooak.Testing
{
    public class CollectionTests
    {
        [Test]
        public void CollectionTest()
        {
            Helpers.TestSuccess<TypeUnion<int, DateTime>[], int, DateTime>("[3, 2, \"2020-01-01T14:00:00Z\"]", new TypeUnion<int, DateTime>[]
            {
                new TypeUnion<int, DateTime>.Left(3),
                new TypeUnion<int, DateTime>.Left(2),
                new TypeUnion<int, DateTime>.Right(new DateTime(2020, 1, 1, 14, 0, 0, DateTimeKind.Utc)),
            }, "OneOf");

            Helpers.TestFailure<TypeUnion<int, bool>[], int, bool>("[3, 2, \"failure\"]", "OneOf");

            Helpers.TestSuccess<Dictionary<string, TypeUnion<string, DateTime>>, string, DateTime>("{\"value1\":\"kickban\", \"value2\":\"2020-01-01T14:00:00Z\"}", new Dictionary<string, TypeUnion<string, DateTime>>
            {
                {"value1", new TypeUnion<string, DateTime>.Left("kickban")},
                {"value2", new TypeUnion<string, DateTime>.Both("2020-01-01T14:00:00Z", new DateTime(2020, 1, 1, 14, 0, 0, DateTimeKind.Utc))}
            }, "AnyOf");

            Helpers.TestFailure<Dictionary<string, TypeUnion<string, bool>>, string, bool>("{\"value1\":\"kickban\", \"value2\":2}", "OneOf");
        }
    }
}
