using NUnit.Framework;
// ReSharper disable JoinDeclarationAndInitializer
// ReSharper disable NotAccessedVariable
#pragma warning disable IDE0059 // Unnecessary assignment of a value - This is a demo file

#if NET5_0
namespace Ooak.Testing.Examples
{
    using System;

    /// <summary>
    /// The examples on the Documentation/ooak.md (ensure that they do build)
    /// </summary>
    public class OoakExamples
    {
        #region Example 1 - Create TypeUnion instances
        /// <summary>
        /// Tries to parse the date. Returns either a DateTime on success, the original string otherwise.
        /// This example is stupid, but demonstrates how things work
        /// </summary>
        TypeUnion<DateTime, string> TryParseDate(string date)
        {
            if (DateTime.TryParse(date, out var outDateTime))
            {
                // Return a `Left`, i.e. a `DateTime` instance
                return new TypeUnion<DateTime, string>.Left(outDateTime);
            }

            // Return a `Right`, i.e. a `DateTime` instance
            return new TypeUnion<DateTime, string>.Right(date);
        }
        #endregion

        #region Example 2 - Can be of two types at the same time
        public record Paging(int Page, int ItemsPerPage);
        public record Ordering(bool Ascending);
        
        public void DemoBoth()
        {
            TypeUnion<Paging, Ordering> queryParameters;
            // Can be left (Paging)
            queryParameters = new TypeUnion<Paging, Ordering>.Left(new Paging(1, 50));
            // Can be right (Ordering)
            queryParameters = new TypeUnion<Paging, Ordering>.Right(new Ordering(true));

            // But can also be both at the same time!
            queryParameters = new TypeUnion<Paging, Ordering>.Both(new Paging(10, 10), new Ordering(false));
        }
        #endregion


        #region Example 3 - Using the type union object

        public void UsingTypeUnion(TypeUnion<Paging, Ordering> queryParameters)
        {
            // Detect with `is` and the Left, Right and Both classes
            if (queryParameters is TypeUnion<Paging, Ordering>.Left p)
            {
                Console.WriteLine($"This is a Paging only at page {p.Value.Page} with {p.Value.ItemsPerPage} items per page");
            }

            // With pattern matching
            switch (queryParameters)
            {
                case TypeUnion<Paging, Ordering>.Left:
                    Console.WriteLine("Left");
                    break;
                case TypeUnion<Paging, Ordering>.Right:
                    Console.WriteLine("Right");
                    break;
                case TypeUnion<Paging, Ordering>.Both:
                    Console.WriteLine("Both");
                    break;
            }

            // But if you need ordering info, you will need to test both Right and Both.
            // Or you could use this helper syntax :
            if (queryParameters.TryGetRight(out var ordering))
            {
                Console.WriteLine($"Ordering {(ordering.Ascending ? "Ascending" : "Descending")}");
            }
        }
        #endregion
    }
}
#endif
