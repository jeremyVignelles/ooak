using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ooak.SystemTextJson
{
    /// <summary>
    /// The <see cref="System.Text.Json.Serialization.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public abstract class OoakSystemTextJsonConverter<TLeft, TRight> : JsonConverter<TypeUnion<TLeft, TRight>>
        where TLeft : notnull
        where TRight : notnull
    {
        /// <summary>
        /// The kind of converter
        /// </summary>
        public enum ConverterKind
        {
            /// <summary>
            /// The converted value must be of one and only one type (either TLeft or TRight)
            /// </summary>
            OneOf,

            /// <summary>
            /// The converted value must be of at least one type (either TLeft or TRight)
            /// </summary>
            AnyOf,

            /// <summary>
            /// The converted value must be deserializable in both TLeft and TRight types
            /// </summary>
            AllOf
        }

        /// <summary>
        /// The kind of conversion to apply
        /// </summary>
        public ConverterKind Kind { get; }

        /// <summary>
        /// A value indicating whether this instance will deserialize entities like `TypeUnion{A,TypeUnion{B, C}}` with the same Kind property
        /// </summary>
        public bool Recursive { get; }

        /// <summary>
        /// The converter's constructor
        /// </summary>
        /// <param name="kind">The kind of conversion to apply</param>
        /// <param name="recursive">A value indicating whether this instance will deserialize entities like `TypeUnion{A,TypeUnion{B, C}}` with the same Kind property</param>
        internal OoakSystemTextJsonConverter(ConverterKind kind, bool recursive = false)
        {
            this.Kind = kind;
            this.Recursive = recursive;
        }

        /// <summary>
        /// Deserializes the entity using the <see cref="ConverterKind"/> rules.
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to deserialize</param>
        /// <param name="options">The serializer options</param>
        /// <returns>The deserialized value</returns>
        public override TypeUnion<TLeft, TRight>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var position = reader.BytesConsumed;
            var leftReader = reader;
            var rightReader = reader;
            TLeft? left = default;
            var leftIsValid = false;
            Exception? leftException = null;
            try
            {
                left = this.DeserializeAsLeft(ref leftReader, options);
                leftIsValid = left is not null && this.LeftIsValid(left);
            }
            catch (JsonException ex)
            {
                leftException = ex;
            }

            TRight? right = default;
            var rightIsValid = false;
            Exception? rightException = null;
            try
            {
                right = this.DeserializeAsRight(ref rightReader, options);
                rightIsValid = right is not null && this.RightIsValid(right);
            }
            catch (JsonException ex)
            {
                rightException = ex;
            }

            if (leftIsValid && rightIsValid)
            {
                if (this.Kind == ConverterKind.OneOf)
                {
                    throw new JsonException(
                        "Matches both types where OneOf was specified. Use AnyOf for that scenario.");
                }

                reader = leftReader;
                return new TypeUnion<TLeft, TRight>.Both(left!, right!);
            }

            if (leftIsValid)
            {
                if (this.Kind == ConverterKind.AllOf)
                {
                    throw new JsonException(
                        $"Matches only the left type while allOf was specified. The value didn't match type {typeof(TRight).Name}", rightException);
                }

                reader = leftReader;
                return new TypeUnion<TLeft, TRight>.Left(left!);
            }

            if (rightIsValid)
            {
                if (this.Kind == ConverterKind.AllOf)
                {
                    throw new JsonException(
                        $"Matches only the right type while allOf was specified. The value didn't match type {typeof(TLeft).Name}", rightException);
                }

                reader = rightReader;
                return new TypeUnion<TLeft, TRight>.Right(right!);
            }

            throw new JsonException(
                $"Unable to deserialize data as either {typeof(TLeft).Name} or {typeof(TRight).Name} at position {position}",
                new AggregateException(
                    leftException ?? new Exception("Left was deserialized as null or as an invalid value"),
                    rightException ?? new Exception("Right was deserialized as null or as an invalid value")));
        }

        /// <summary>
        /// The method that tries to perform a deserialization as TLeft. Throws <see cref="JsonException"/> on error.
        /// </summary>
        /// <param name="reader">The reader to read values from</param>
        /// <param name="options">The serialization options</param>
        /// <returns>The deserialized value.</returns>
        protected virtual TLeft? DeserializeAsLeft(ref Utf8JsonReader reader, JsonSerializerOptions options) => this.DefaultDeserialize<TLeft>(ref reader, options);

        /// <summary>
        /// The method that tries to perform a deserialization as TRight. Throws <see cref="JsonException"/> on error.
        /// </summary>
        /// <param name="reader">The reader to read values from</param>
        /// <param name="options">The serialization options</param>
        /// <returns>The deserialized value.</returns>
        protected virtual TRight? DeserializeAsRight(ref Utf8JsonReader reader, JsonSerializerOptions options) => this.DefaultDeserialize<TRight>(ref reader, options);

        /// <summary>
        /// Returns a value indicating whether the deserialized TLeft value is valid
        /// </summary>
        /// <param name="value">The deserialized value</param>
        /// <returns>A boolean indicating whether the deserialized value is valid</returns>
        protected virtual bool LeftIsValid(TLeft value) => true;

        /// <summary>
        /// Returns a value indicating whether the deserialized TRight value is valid
        /// </summary>
        /// <param name="value">The deserialized value</param>
        /// <returns>A boolean indicating whether the deserialized value is valid</returns>
        protected virtual bool RightIsValid(TRight value) => true;

        /// <summary>
        /// This method is not yet implemented
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, TypeUnion<TLeft, TRight> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private delegate TItem? RecursiveRead<TItem>(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);

        /// <summary>
        /// The default deserialization method that can deserialize any TItem
        /// </summary>
        /// <typeparam name="TItem">The item type to deserialize</typeparam>
        /// <param name="reader">The reader to read values from</param>
        /// <param name="options">The serialization options</param>
        /// <returns>The deserialized value.</returns>
        protected virtual TItem? DefaultDeserialize<TItem>(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (this.Recursive) {
                var itemType = typeof(TItem);
                if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(TypeUnion<,>))
                {
                    var defaultRecursiveConverterType = this.Kind switch
                    {
                        ConverterKind.OneOf => typeof(RecursiveOneOfJsonConverter<,>),
                        ConverterKind.AnyOf => typeof(RecursiveAnyOfJsonConverter<,>),
                        ConverterKind.AllOf => typeof(RecursiveAllOfJsonConverter<,>),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    var recursiveConverterType = defaultRecursiveConverterType.MakeGenericType(itemType.GenericTypeArguments);
                    var recursiveConverter = recursiveConverterType.GetConstructor(Array.Empty<Type>()).Invoke(Array.Empty<object>());
                    return ((RecursiveRead<TItem>)recursiveConverterType.GetMethod("Read").CreateDelegate(typeof(RecursiveRead<TItem>), recursiveConverter)).Invoke(ref reader, itemType, options);
                }
            }
            return JsonSerializer.Deserialize<TItem>(ref reader, options);
        }
    }

    /// <summary>
    /// The <see cref="System.Text.Json.Serialization.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/>
    /// using the <see cref="OoakSystemTextJsonConverter{TLeft,TRight}.ConverterKind.OneOf"/> rules.
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class OneOfJsonConverter<TLeft, TRight> : OoakSystemTextJsonConverter<TLeft, TRight>
        where TLeft : notnull
        where TRight : notnull
    {
        /// <summary>
        /// The constructor that initializes the converter
        /// </summary>
        public OneOfJsonConverter() : base(ConverterKind.OneOf)
        {
        }
    }

    /// <summary>
    /// The <see cref="System.Text.Json.Serialization.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/>
    /// using the <see cref="OoakSystemTextJsonConverter{TLeft,TRight}.ConverterKind.AnyOf"/> rules.
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class AnyOfJsonConverter<TLeft, TRight> : OoakSystemTextJsonConverter<TLeft, TRight>
        where TLeft : notnull
        where TRight : notnull
    {
        /// <summary>
        /// The constructor that initializes the converter
        /// </summary>
        public AnyOfJsonConverter() : base(ConverterKind.AnyOf)
        {
        }
    }

    /// <summary>
    /// The <see cref="System.Text.Json.Serialization.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/>
    /// using the <see cref="OoakSystemTextJsonConverter{TLeft,TRight}.ConverterKind.AllOf"/> rules.
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class AllOfJsonConverter<TLeft, TRight> : OoakSystemTextJsonConverter<TLeft, TRight>
        where TLeft : notnull
        where TRight : notnull
    {
        /// <summary>
        /// The constructor that initializes the converter
        /// </summary>
        public AllOfJsonConverter() : base(ConverterKind.AllOf)
        {
        }
    }

    /// <summary>
    /// The <see cref="System.Text.Json.Serialization.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/> recusively
    /// using the <see cref="OoakSystemTextJsonConverter{TLeft,TRight}.ConverterKind.OneOf"/> rules.
    /// This is a recursive (= can deserialize TypeUnion of TypeUnion with the same kind) version of the <see cref="OneOfJsonConverter{TLeft, TRight}"/> class
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class RecursiveOneOfJsonConverter<TLeft, TRight> : OoakSystemTextJsonConverter<TLeft, TRight>
        where TLeft : notnull
        where TRight : notnull
    {
        /// <summary>
        /// The constructor that initializes the converter
        /// </summary>
        public RecursiveOneOfJsonConverter() : base(ConverterKind.OneOf, true)
        {
        }
    }

    /// <summary>
    /// The <see cref="System.Text.Json.Serialization.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/> recusively
    /// using the <see cref="OoakSystemTextJsonConverter{TLeft,TRight}.ConverterKind.AnyOf"/> rules.
    /// This is a recursive (= can deserialize TypeUnion of TypeUnion with the same kind) version of the <see cref="AnyOfJsonConverter{TLeft, TRight}"/> class
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class RecursiveAnyOfJsonConverter<TLeft, TRight> : OoakSystemTextJsonConverter<TLeft, TRight>
        where TLeft : notnull
        where TRight : notnull
    {
        /// <summary>
        /// The constructor that initializes the converter
        /// </summary>
        public RecursiveAnyOfJsonConverter() : base(ConverterKind.AnyOf, true)
        {
        }
    }

    /// <summary>
    /// The <see cref="System.Text.Json.Serialization.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/> recusively
    /// using the <see cref="OoakSystemTextJsonConverter{TLeft,TRight}.ConverterKind.AllOf"/> rules.
    /// This is a recursive (= can deserialize TypeUnion of TypeUnion with the same kind) version of the <see cref="AllOfJsonConverter{TLeft, TRight}"/> class
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class RecursiveAllOfJsonConverter<TLeft, TRight> : OoakSystemTextJsonConverter<TLeft, TRight>
        where TLeft : notnull
        where TRight : notnull
    {
        /// <summary>
        /// The constructor that initializes the converter
        /// </summary>
        public RecursiveAllOfJsonConverter() : base(ConverterKind.AllOf, true)
        {
        }
    }
}
