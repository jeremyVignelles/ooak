# Ooak serializers/deserializers

There are two packages provided out of the box : One for Newtonsoft.Json and one for System.Text.Json.
Be careful : These deserialization libraries sometimes behave differently, it's important to understand the limitations of each method!

The goal is to provide JSON serializers and deserializers for the anyOf/allOf/oneOf use cases. The serializers have the same name between the two packages to ease the transition between the two.

NOTE : The serialization part is not yet implemented.

## The converters

All these converters are used to deserialize a part of a JSON document into a `TypeUnion<TLeft, TRight>`. The behavior differs between the converters chosen

- `AnyOfJsonConverter<TLeft, TRight>`: The deserialized value can be either a `TLeft`, a `TRight` or may deserialize as both a `TLeft` and a `TRight` at the same time
- `AllOfJsonConverter<TLeft, TRight>`: Throws a `JsonSerializationException` (Newtonsoft.Json) or `JsonException` (System.Text.Json) if the value fails to deserialize as `TLeft` or `TRight`. The value must be valid as a `TLeft` and a `TRight`
- `OneOfJsonConverter<TLeft, TRight>`: Throws a `JsonSerializationException` (Newtonsoft.Json) or `JsonException` (System.Text.Json) if the value fails to deserialize with both `TLeft` and `TRight` type. The value must be valid as either `TLeft` or `TRight`


## Using the converters

You can use the converters, as any other JsonConverter.

```cs
public class TestModel
{
    // You can put the converter directly on the property:
    [JsonConverter(typeof(OneOfJsonConverter<int, DateTime>))]
    public TypeUnion<int, DateTime> DateOrTimestamp { get; set; }
}

#if NET5_0
public record TestRecord(
    // You can put the converter on a record's property
    [property: JsonConverter(typeof(OneOfJsonConverter<int, DateTime>))]
    TypeUnion<int, DateTime> DateOrTimestamp
);
#endif

public TypeUnion<int, DateTime> DeserializeMethodSystemTextJson()
{
    // With STJ, as the deserializer options. Useful for example when the TypeUnion<> is the deserialization root
    return JsonSerializer.Deserialize<TypeUnion<int, DateTime>>("\"2000-01-01T00:00:00Z\"",
        new JsonSerializerOptions()
        {
            Converters = { new OneOfJsonConverter<int, DateTime>() }
        })!;
}
public TypeUnion<int, DateTime> DeserializeMethodNewtonsoftJson()
{
    // With Newtonsoft.Json, as the deserializer options. Useful for example when the TypeUnion<> is the deserialization root
    // Note: Here, I'm using fully-qualified names with the Newtonsoft namespace. This is only needed because in this example,
    // I have mixed STJ and Newtonsoft in the same file. You shouldn't need to be that verbose in your own code (don't mix serializers!)
    return Newtonsoft.Json.JsonConvert.DeserializeObject<TypeUnion<int, DateTime>>("\"2000-01-01T00:00:00Z\"",
        new Newtonsoft.Json.JsonSerializerSettings()
        {
            Converters = { new NewtonsoftJson.OneOfJsonConverter<int, DateTime>() }
        })!;
}
```

## How it works

Both deserializer implementations work in the same simple way:

1. Try to deserialize the value as `TLeft`.
2. If the result is `null` or if an exception is thrown by the deserializer, consider the left part invalid
3. If the value is not invalid, call the `LeftIsValid` method with the deserialized value. If that method returns `false` consider the left part invalid
4. Repeat the steps 1-3 with `TRight`
5. Resolve the required rules and throw if necessary (for example if it deserializes as both `Left` and `Right` while `OneOfJsonSerializer` is used)
6. Wrap the result in a new `TypeUnion` instance and return

## Subclassing the JsonConverter

There are methods you can override in a subclass of any of the Deserializer :

- `DeserializeAsLeft`/`DeserializeAsRight` the deserialize function that can be overriden to handle special deserialization logic.
- `LeftIsValid`/`RightIsValid` These may be use to check the deserialized values to ensure they are valid. This is especially useful for System.Text.Json [which doesn't fail deserialization of `non-nullable reference type` when a `null` is given in the JSON](https://github.com/dotnet/runtime/issues/1256).

## Behavior differences between STJ and Newtonsoft.Json

- [STJ doesn't fail deserialization on non-nullable reference types](https://github.com/dotnet/runtime/issues/1256)
- [Newtonsoft.Json interprets strings that looks like dates](https://github.com/JamesNK/Newtonsoft.Json/issues/862)

// TODO: There are more edge cases, please document them !
