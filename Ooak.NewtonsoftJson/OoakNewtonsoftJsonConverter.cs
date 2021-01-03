using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ooak.NewtonsoftJson
{
    /// <summary>
    /// The <see cref="Newtonsoft.Json.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public abstract class OoakNewtonsoftJsonConverter<TLeft, TRight> : JsonConverter<TypeUnion<TLeft, TRight>>
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
        internal OoakNewtonsoftJsonConverter(ConverterKind kind)
        {
            this.Kind = kind;
        }

        /// <summary>
        /// Deserializes the entity using the <see cref="ConverterKind"/> rules.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
        /// <param name="hasExistingValue">The existing value has a value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override TypeUnion<TLeft, TRight> ReadJson(JsonReader reader, Type objectType, TypeUnion<TLeft, TRight>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken token = JToken.ReadFrom(reader);

            TLeft left = default;
            var leftIsValid = false;
            Exception? leftException = null;
            try
            {
                left = token.ToObject<TLeft>(serializer);
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
                right = token.ToObject<TRight>(serializer);
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
                    throw new JsonSerializationException("Matches both types where OneOf was specified. Use AnyOf for that scenario.");
                }
                return new TypeUnion<TLeft, TRight>.Both(left!, right!);
            }

            if (leftIsValid)
            {
                if (this.Kind == ConverterKind.AllOf)
                {
                    throw new JsonSerializationException($"Matches only the left type while allOf was specified. The value didn't match type {typeof(TRight).Name}", rightException!);
                }
                return new TypeUnion<TLeft, TRight>.Left(left!);
            }

            if (rightIsValid)
            {
                if (this.Kind == ConverterKind.AllOf)
                {
                    throw new JsonSerializationException($"Matches only the right type while allOf was specified. The value didn't match type {typeof(TLeft).Name}", leftException!);
                }
                return new TypeUnion<TLeft, TRight>.Right(right!);
            }

            throw new JsonSerializationException($"Unable to deserialize data as either {typeof(TLeft).Name} or {typeof(TRight).Name}", new AggregateException(leftException, rightException));
        }

        /// <summary>
        /// This method is not yet implemented
        /// </summary>
        /// <param name="writer">The <see cref="Newtonsoft.Json.JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, TypeUnion<TLeft, TRight>? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The <see cref="Newtonsoft.Json.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/>
    /// using the <see cref="OoakNewtonsoftJsonConverter{TLeft,TRight}.ConverterKind.OneOf"/> rules.
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class OneOfJsonConverter<TLeft, TRight> : OoakNewtonsoftJsonConverter<TLeft, TRight>
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
    /// The <see cref="Newtonsoft.Json.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/>
    /// using the <see cref="OoakNewtonsoftJsonConverter{TLeft,TRight}.ConverterKind.AnyOf"/> rules.
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class AnyOfJsonConverter<TLeft, TRight> : OoakNewtonsoftJsonConverter<TLeft, TRight>
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
    /// The <see cref="Newtonsoft.Json.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/>
    /// using the <see cref="OoakNewtonsoftJsonConverter{TLeft,TRight}.ConverterKind.AllOf"/> rules.
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class AllOfJsonConverter<TLeft, TRight> : OoakNewtonsoftJsonConverter<TLeft, TRight>
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
