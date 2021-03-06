﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ooak.Testing
{
    public static class Helpers
    {

        public static System.Text.Json.Serialization.JsonConverter MakeSystemTextJsonConverter<TLeft, TRight>(string converterType)
            where TLeft : notnull
            where TRight : notnull
        {
            var type = typeof(Ooak.SystemTextJson.OoakSystemTextJsonConverter<TLeft, TRight>).Assembly
                    .GetType($"Ooak.SystemTextJson.{converterType}JsonConverter`2")
                !.MakeGenericType(typeof(TLeft), typeof(TRight));
            return (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(type)!;
        }

        public static Newtonsoft.Json.JsonConverter MakeNewtonsoftJsonConverter<TLeft, TRight>(string converterType)
            where TLeft : notnull
            where TRight : notnull
        {
            var type = typeof(Ooak.NewtonsoftJson.OoakNewtonsoftJsonConverter<TLeft, TRight>).Assembly
                    .GetType($"Ooak.NewtonsoftJson.{converterType}JsonConverter`2")
                !.MakeGenericType(typeof(TLeft), typeof(TRight));
            return (Newtonsoft.Json.JsonConverter)Activator.CreateInstance(type)!;
        }

        [return: MaybeNull]
        public static T DeserializeSystemTextJson<T>(string input, params System.Text.Json.Serialization.JsonConverter[] converters)
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            foreach (var c in converters)
            {
                options.Converters.Add(c);
            }

            return System.Text.Json.JsonSerializer.Deserialize<T>(input, options);
        }

        [return: MaybeNull]
        public static T DeserializeNewtonsoftJson<T>(string input, params Newtonsoft.Json.JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(input, new Newtonsoft.Json.JsonSerializerSettings
            {
                Converters = converters,
                DateParseHandling = DateParseHandling.None // Otherwise, OneOf<DateTime, string> would have a string component that is a dateTime.ToString() rather than the original value.
            });
        }
        
        public static void TestSuccess<TDeserialized, TLeft, TRight>(string input, TDeserialized expected, string converterType)
            where TLeft : notnull
            where TRight : notnull
        {
            TestSuccess<TDeserialized>(input, expected, expected, MakeSystemTextJsonConverter<TLeft, TRight>(converterType), MakeNewtonsoftJsonConverter<TLeft, TRight>(converterType));
        }

        public static void TestSuccess<TDeserialized>(
            string input,
            TDeserialized expected,
            System.Text.Json.Serialization.JsonConverter systemTextJsonSerializer,
            Newtonsoft.Json.JsonConverter newtonsoftJsonConverter)
        {
            TestSuccess<TDeserialized>(input, expected, expected, systemTextJsonSerializer, newtonsoftJsonConverter);
        }

        public static void TestSuccess<TDeserialized>(
            string input,
            TDeserialized systemTextJsonExpected, TDeserialized newtonsoftJsonExpected,
            System.Text.Json.Serialization.JsonConverter systemTextJsonSerializer, Newtonsoft.Json.JsonConverter newtonsoftJsonConverter)
        {
            var deserialized = DeserializeSystemTextJson<TDeserialized>(input, systemTextJsonSerializer);
            Console.WriteLine("Checking System.Text.Json deserialized value");
            Assert.AreEqual(systemTextJsonExpected, deserialized);

            var deserializedNewtonsoft = DeserializeNewtonsoftJson<TDeserialized>(input, newtonsoftJsonConverter);
            Console.WriteLine("Checking Newtonsoft.Json deserialized value");
            Assert.AreEqual(newtonsoftJsonExpected, deserializedNewtonsoft);
        }

        public static void TestSuccess<TDeserialized>(string input, TDeserialized expected)
        {
            TestSuccess<TDeserialized>(input, expected, expected);
        }

        public static void TestSuccess<TDeserialized>(string input, TDeserialized systemTextJsonExpected, TDeserialized newtonsoftJsonExpected)
        {
            var deserialized = DeserializeSystemTextJson<TDeserialized>(input);
            Console.WriteLine("Checking System.Text.Json deserialized value");
            Assert.AreEqual(systemTextJsonExpected, deserialized);

            var deserializedNewtonsoft = DeserializeNewtonsoftJson<TDeserialized>(input);
            Console.WriteLine("Checking Newtonsoft.Json deserialized value");
            Assert.AreEqual(newtonsoftJsonExpected, deserializedNewtonsoft);
        }

        public static void TestFailure<TDeserialized>(string input)
        {
            Console.WriteLine("Attempting System.Text.Json deserialization");
            Assert.Throws<System.Text.Json.JsonException>(() => DeserializeSystemTextJson<TDeserialized>(input));
            Console.WriteLine("Attempting Newtonsoft.Json deserialization");
            Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => DeserializeNewtonsoftJson<TDeserialized>(input));
        }

        public static void TestFailure<TDeserialized, TLeft, TRight>(string input, string converterType)
            where TLeft : notnull
            where TRight : notnull
        {
            Console.WriteLine("Attempting System.Text.Json deserialization");
            Assert.Throws<System.Text.Json.JsonException>(() => DeserializeSystemTextJson<TDeserialized>(input, MakeSystemTextJsonConverter<TLeft, TRight>(converterType)));
            Console.WriteLine("Attempting Newtonsoft.Json deserialization");
            Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => DeserializeNewtonsoftJson<TDeserialized>(input, MakeNewtonsoftJsonConverter<TLeft, TRight>(converterType)));
        }
    }
}
