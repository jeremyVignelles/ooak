using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ooak.Testing
{
    public class CollectionTests
    {
        [Test]
        public void CollectionTest()
        {
            Helpers.TestSuccess<TypeUnion<int, bool>[], int, bool>("[3, 2, true]", new TypeUnion<int, bool>[]
            {
                new TypeUnion<int, bool>.Left(3),
                new TypeUnion<int, bool>.Left(2),
                new TypeUnion<int, bool>.Right(true),
            }, "OneOf");

            Helpers.TestFailure<TypeUnion<int, bool>[], int, string>("[1, 2, \"failure\"]", "OneOf");

            Helpers.TestSuccess<Dictionary<string, TypeUnion<string, int>>, string, int>("{\"value1\":\"kickban\", \"value2\":42}", new Dictionary<string, TypeUnion<string, int>>
            {
                {"value1", new TypeUnion<string, int>.Left("kickban")},
                {"value2", new TypeUnion<string, int>.Right(42)}
            }, "OneOf");

            Helpers.TestFailure<Dictionary<string, TypeUnion<string, bool>>, string, bool>("{\"value1\":\"kickban\", \"value2\":2}", "OneOf");
        }
    }
}
