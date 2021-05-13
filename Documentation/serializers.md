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

https://github.com/jeremyVignelles/ooak/blob/1b5254aeb57fa8a79eb5b59be01d357835c5f3bb/Ooak.Testing/Examples/SystemTextJsonExamples.cs#L11-L45

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
