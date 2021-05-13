# Ooak - The TypeUnion implementation

This package allows to use type unions in C#. A type union represents a value that can be one of two types.

## Creating TypeUnion instances

Create TypeUnion instances:
```cs
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
```

Sometimes, the value you represent can be of two types at the same time (the anyOf case of the JSON deserialization):

```cs
// Yes, C#9 records are supported !
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
```

## Using the TypeUnion object

```cs
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
        case TypeUnion<Paging, Ordering>.Left left:
            Console.WriteLine("Left");
            break;
        case TypeUnion<Paging, Ordering>.Right right:
            Console.WriteLine("Right");
            break;
        case TypeUnion<Paging, Ordering>.Both right:
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
```

## Special notes
The type parameters cannot contain nullable types (you cannot write `TypeUnion<string?, DateTime>` nor `TypeUnion<string, DateTime?>`), but you can make your whole union nullable (`TypeUnion<string, DateTime>?`).

This library only supports 2 type parameters for now. If you need more parameters, nest them : `TypeUnion<A, TypeUnion<B, C>>`.
