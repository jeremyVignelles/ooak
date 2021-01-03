using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ooak.Testing.Models;

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

        [Test]
        public void ArrayOfUnionTests()
        {
            Helpers.TestSuccess("{\"Values\":[3, 2, \"2020-01-01T14:00:00Z\"]}", new ArrayOfUnionModel
            {
                Values = new TypeUnion<int, DateTime>[]
                {
                    new TypeUnion<int, DateTime>.Left(3),
                    new TypeUnion<int, DateTime>.Left(2),
                    new TypeUnion<int, DateTime>.Right(new DateTime(2020, 1, 1, 14, 0, 0, DateTimeKind.Utc)),
                }
            });
        }

        [Test]
        public void UnionOfArray()
        {
            Helpers.TestSuccess("{\"Values\":[1, 2, 3, 0]}", new UnionOfArrayModel
            {
                Values = new TypeUnion<int[], DateTime[]>.Left(new [] {1, 2, 3, 0})
            });
            Helpers.TestSuccess("{\"Values\":[\"2020-01-01T14:00:00Z\"]}", new UnionOfArrayModel
            {
                Values = new TypeUnion<int[], DateTime[]>.Right(new []
                {
                    new DateTime(2020, 1, 1, 14, 0, 0, DateTimeKind.Utc)
                })
            });
            Helpers.TestFailure<UnionOfArrayModel>("{\"Values\":[3, 2, \"2020-01-01T14:00:00Z\"]}");
        }

        [Test]
        public void DictionaryOfUnion()
        {
            Helpers.TestSuccess("{\"Values\":{\"a\": 3, \"b\": 2, \"c\": \"2020-01-01T14:00:00Z\"}}", new DictionaryOfUnionModel
            {
                Values = new Dictionary<string, TypeUnion<int, DateTime>>
                {
                    { "a", new TypeUnion<int, DateTime>.Left(3) },
                    { "b", new TypeUnion<int, DateTime>.Left(2) },
                    { "c", new TypeUnion<int, DateTime>.Right(new DateTime(2020, 1, 1, 14, 0, 0, DateTimeKind.Utc)) },
                }
            });
        }

        [Test]
        public void UnionOfDictionary()
        {
            Helpers.TestSuccess("{\"Values\":{\"a\": 1, \"b\": 2, \"c\": 3, \"d\": 0}}", new UnionOfDictionaryModel
            {
                Values = new TypeUnion<Dictionary<string, int>, Dictionary<string, DateTime>>.Left(new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 }, { "d", 0 } })
            });
            Helpers.TestSuccess("{\"Values\":{\"a\": \"2020-01-01T14:00:00Z\"}}", new UnionOfDictionaryModel
            {
                Values = new TypeUnion<Dictionary<string, int>, Dictionary<string, DateTime>>.Right(new Dictionary<string, DateTime>
                {
                    { "a", new DateTime(2020, 1, 1, 14, 0, 0, DateTimeKind.Utc) }
                })
            });
            Helpers.TestFailure<UnionOfDictionaryModel>("{\"Values\":{\"a\": 3, \"b\": 2, \"c\": \"2020-01-01T14:00:00Z\"}}");
        }
    }
}
