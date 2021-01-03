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
        /// The converter's constructor
        /// </summary>
        /// <param name="kind">The kind of conversion to apply</param>
        internal OoakSystemTextJsonConverter(ConverterKind kind)
        {
            this.Kind = kind;
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
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            
            try
            {
                TLeft left = default;
                var leftIsValid = false;
                Exception? leftException = null;
                try
                {
                    var reader2 = reader;
                    left = JsonSerializer.Deserialize<TLeft>(ref reader2, options);
                    leftIsValid = true;
                }
                catch (JsonException ex)
                {
                    leftException = ex;
                }
                
                TRight right = default;
                var rightIsValid = false;
                Exception? rightException = null;
                try
                {
                    var reader2 = reader;
                    right = JsonSerializer.Deserialize<TRight>(ref reader2, options);
                    rightIsValid = true;
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

                    return new TypeUnion<TLeft, TRight>.Both(left!, right!);
                }

                if (leftIsValid)
                {
                    if (this.Kind == ConverterKind.AllOf)
                    {
                        throw new JsonException(
                            $"Matches only the left type while allOf was specified. The value didn't match type {typeof(TRight).Name}", rightException);
                    }

                    return new TypeUnion<TLeft, TRight>.Left(left!);
                }

                if (rightIsValid)
                {
                    if (this.Kind == ConverterKind.AllOf)
                    {
                        throw new JsonException(
                            $"Matches only the right type while allOf was specified. The value didn't match type {typeof(TLeft).Name}", rightException);
                    }

                    return new TypeUnion<TLeft, TRight>.Right(right!);
                }

                throw new JsonException(
                    $"Unable to deserialize data as either {typeof(TLeft).Name} or {typeof(TRight).Name}", new AggregateException(leftException, rightException));
            }
            finally
            {
                reader.Skip();
            }
        }

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
}
