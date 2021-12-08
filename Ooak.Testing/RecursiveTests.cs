using System;
using NUnit.Framework;

namespace Ooak.Testing
{
    public class RecursiveTests
    {
        [Test]
        public void TestRecursive()
        {
            Helpers.TestSuccess<TypeUnion<int, TypeUnion<Guid, DateTime>>, int, TypeUnion<Guid, DateTime>>("42", new TypeUnion<int, TypeUnion<Guid, DateTime>>.Left(42), "RecursiveOneOf");
            Helpers.TestSuccess<TypeUnion<int, TypeUnion<Guid, DateTime>>, int, TypeUnion<Guid, DateTime>>("\"00000000-0000-0000-0000-000000000000\"", new TypeUnion<int, TypeUnion<Guid, DateTime>>.Right(new TypeUnion<Guid, DateTime>.Left(Guid.Empty)), "RecursiveOneOf");

            Helpers.TestSuccess<TypeUnion<int, TypeUnion<string, DateTime>>, int, TypeUnion<string, DateTime>>("\"00000000-0000-0000-0000-000000000000\"", new TypeUnion<int, TypeUnion<string, DateTime>>.Right(new TypeUnion<string, DateTime>.Left("00000000-0000-0000-0000-000000000000")), "RecursiveOneOf");
            // Matches both in TypeUnion<string, DateTime>
            Helpers.TestFailure<TypeUnion<int, TypeUnion<string, DateTime>>, int, TypeUnion<string, DateTime>>("\"2021-12-08T18:42:00Z\"", "RecursiveOneOf");
            
            Helpers.TestSuccess<TypeUnion<TypeUnion<double, DateTime>, int>, TypeUnion<double, DateTime>, int>("42", new TypeUnion<TypeUnion<double, DateTime>, int>.Both(new TypeUnion<double, DateTime>.Left(42), 42), "RecursiveAnyOf");

        }
    }
}
