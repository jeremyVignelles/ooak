# Ooak - The TypeUnion implementation

This package allows to use type unions in C#. A type union represents a value that can be one of two types.

## Creating TypeUnion instances

Create TypeUnion instances:

https://github.com/jeremyVignelles/ooak/blob/1b5254aeb57fa8a79eb5b59be01d357835c5f3bb/Ooak.Testing/Examples/OoakExamples.cs#L18-L32

Sometimes, the value you represent can be of two types at the same time (the anyOf case of the JSON deserialization):

https://github.com/jeremyVignelles/ooak/blob/1b5254aeb57fa8a79eb5b59be01d357835c5f3bb/Ooak.Testing/Examples/OoakExamples.cs#L36-L49

## Using the TypeUnion object

https://github.com/jeremyVignelles/ooak/blob/1b5254aeb57fa8a79eb5b59be01d357835c5f3bb/Ooak.Testing/Examples/OoakExamples.cs#L55-L83

## Special notes
The type parameters cannot contain nullable types (you cannot write `TypeUnion<string?, DateTime>` nor `TypeUnion<string, DateTime?>`), but you can make your whole union nullable (`TypeUnion<string, DateTime>?`).

This library only supports 2 type parameters for now. If you need more parameters, nest them : `TypeUnion<A, TypeUnion<B, C>>`.
