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
        /// A value indicating whether this instance will deserialize entities like `TypeUnion{A,TypeUnion{B, C}}` with the same Kind property
        /// </summary>
        public bool Recursive { get; }

        /// <summary>
        /// The converter's constructor
        /// </summary>
        /// <param name="kind">The kind of conversion to apply</param>
        /// <param name="recursive">A value indicating whether this instance will deserialize entities like `TypeUnion{A,TypeUnion{B, C}}` with the same Kind property</param>
        internal OoakNewtonsoftJsonConverter(ConverterKind kind, bool recursive = false)
        {
            this.Kind = kind;
            this.Recursive = recursive;
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
            var path = reader.Path;
            JToken token = JToken.ReadFrom(reader);
            return this.ReadToken(token, serializer, path);
        }

        /// <summary>
        /// Deserializes the TypeUnion instance from a JToken
        /// </summary>
        /// <param name="token">The token to deserialize</param>
        /// <param name="serializer">The serializer to use for the deserialization process</param>
        /// <param name="path">The path of the current token being examined, for debugging purpose only</param>
        /// <returns>The object value.</returns>
        /// <exception cref="JsonSerializationException">The deserialization failed</exception>
        public virtual TypeUnion<TLeft, TRight> ReadToken(JToken token, JsonSerializer serializer, string path)
        {
            TLeft? left = default;
            var leftIsValid = false;
            Exception? leftException = null;
            try
            {
                left = this.DeserializeAsLeft(token, serializer, path);
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
                right = this.DeserializeAsRight(token, serializer, path);
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

            throw new JsonSerializationException($"Unable to deserialize data as either {typeof(TLeft).Name} or {typeof(TRight).Name} at path {path}",
                new AggregateException(
                    leftException ?? new Exception("Left was deserialized as null or as an invalid value"),
                    rightException ?? new Exception("Right was deserialized as null or as an invalid value")));
        }

        /// <summary>
        /// The method that tries to perform a deserialization as TLeft. Throws <see cref="JsonSerializationException"/> on error.
        /// </summary>
        /// <param name="token">The token to deserialize</param>
        /// <param name="serializer">The serializer that contains serializer configuration</param>
        /// <param name="path">The path of the current token being examined, for debugging purpose only</param>
        /// <returns>The deserialized value.</returns>
        protected virtual TLeft? DeserializeAsLeft(JToken token, JsonSerializer serializer, string path) => this.DefaultDeserialize<TLeft>(token, serializer, path);

        /// <summary>
        /// The method that tries to perform a deserialization as TRight. Throws <see cref="JsonSerializationException"/> on error.
        /// </summary>
        /// <param name="token">The token to deserialize</param>
        /// <param name="serializer">The serializer that contains serializer configuration</param>
        /// <param name="path">The path of the current token being examined, for debugging purpose only</param>
        /// <returns>The deserialized value.</returns>
        protected virtual TRight? DeserializeAsRight(JToken token, JsonSerializer serializer, string path) => this.DefaultDeserialize<TRight>(token, serializer, path);

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

        private delegate TItem RecursiveReadToken<TItem>(JToken token, JsonSerializer serializer, string path);

        /// <summary>
        /// The default deserialization method that can deserialize any TItem
        /// </summary>
        /// <typeparam name="TItem">The item type to deserialize</typeparam>
        /// <param name="token">The token to deserialize</param>
        /// <param name="serializer">The serializer that contains serializer configuration</param>
        /// <param name="path">The path of the current token being examined, for debugging purpose only</param>
        /// <returns>The deserialized value.</returns>
        protected virtual TItem? DefaultDeserialize<TItem>(JToken token, JsonSerializer serializer, string path)
        {
            if (this.Recursive)
            {
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
                    return ((RecursiveReadToken<TItem>)recursiveConverterType.GetMethod("ReadToken").CreateDelegate(typeof(RecursiveReadToken<TItem>), recursiveConverter)).Invoke(token, serializer, path);
                }
            }
            return token.ToObject<TItem>(serializer);
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

    /// <summary>
    /// The <see cref="Newtonsoft.Json.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/> recusively
    /// using the <see cref="OoakNewtonsoftJsonConverter{TLeft,TRight}.ConverterKind.OneOf"/> rules.
    /// This is a recursive (= can deserialize TypeUnion of TypeUnion with the same kind) version of the <see cref="OneOfJsonConverter{TLeft, TRight}"/> class
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class RecursiveOneOfJsonConverter<TLeft, TRight> : OoakNewtonsoftJsonConverter<TLeft, TRight>
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
    /// The <see cref="Newtonsoft.Json.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/> recusively
    /// using the <see cref="OoakNewtonsoftJsonConverter{TLeft,TRight}.ConverterKind.AnyOf"/> rules.
    /// This is a recursive (= can deserialize TypeUnion of TypeUnion with the same kind) version of the <see cref="AnyOfJsonConverter{TLeft, TRight}"/> class
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class RecursiveAnyOfJsonConverter<TLeft, TRight> : OoakNewtonsoftJsonConverter<TLeft, TRight>
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
    /// The <see cref="Newtonsoft.Json.JsonConverter" /> that can deserialize instances of <see cref="TypeUnion{TLeft,TRight}"/> recusively
    /// using the <see cref="OoakNewtonsoftJsonConverter{TLeft,TRight}.ConverterKind.AllOf"/> rules.
    /// This is a recursive (= can deserialize TypeUnion of TypeUnion with the same kind) version of the <see cref="AllOfJsonConverter{TLeft, TRight}"/> class
    /// </summary>
    /// <typeparam name="TLeft">The left parameter</typeparam>
    /// <typeparam name="TRight">The right parameter</typeparam>
    public class RecursiveAllOfJsonConverter<TLeft, TRight> : OoakNewtonsoftJsonConverter<TLeft, TRight>
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
