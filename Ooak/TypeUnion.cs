using System;
using System.Diagnostics.CodeAnalysis;

namespace Ooak
{
    /// <summary>
    /// Represents an union of two types for JSON deserialization.
    /// Either the value read is of type TLeft, either of type TRight, or sometimes it can be both if the schema claims allOf
    /// </summary>
    /// <typeparam name="TLeft">The first parameter type of the union</typeparam>
    /// <typeparam name="TRight">The second parameter type of the union</typeparam>
    public abstract class TypeUnion<TLeft, TRight>
        where TLeft : notnull
        where TRight : notnull
    {
        /// <summary>
        /// The class that represents only the left part of the union
        /// </summary>
        public sealed class Left : TypeUnion<TLeft, TRight>, IEquatable<Left>
        {
            /// <summary>
            /// The constructor
            /// </summary>
            /// <param name="value">The value held by the left part of the union</param>
            public Left(TLeft value)
            {
                this.Value = value;
            }

            /// <summary>
            /// The value
            /// </summary>
            public TLeft Value { get; }

            /// <summary>
            /// Tests if the value contained in this instance is equal to the one in the other instance
            /// </summary>
            /// <param name="other">The instance to compare to</param>
            /// <returns>true if both instances wraps the same value, according to object.Equals rules</returns>
            public bool Equals(Left other)
            {
                return object.Equals(other.Value, this.Value);
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return (obj is Left other) && this.Equals(other);
            }

            /// <inheritdoc />
            public override int GetHashCode() => HashCode.Combine(this.Value);

            /// <inheritdoc />
            public override string ToString() => $"Left[{this.Value}]";
        }

        /// <summary>
        /// The class that represents only the right part of the union
        /// </summary>
        public sealed class Right : TypeUnion<TLeft, TRight>, IEquatable<Right>
        {
            /// <summary>
            /// The constructor
            /// </summary>
            /// <param name="value">The value held by the right part of the union</param>
            public Right(TRight value)
            {
                this.Value = value;
            }

            /// <summary>
            /// The value
            /// </summary>
            public TRight Value { get; }

            /// <summary>
            /// Tests if the value contained in this instance is equal to the one in the other instance
            /// </summary>
            /// <param name="other">The instance to compare to</param>
            /// <returns>true if both instances wraps the same value, according to object.Equals rules</returns>
            public bool Equals(Right other)
            {
                return object.Equals(other.Value, this.Value);
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return (obj is Right other) && this.Equals(other);
            }

            /// <inheritdoc />
            public override int GetHashCode() => HashCode.Combine(this.Value);

            /// <inheritdoc />
            public override string ToString() => $"Right[{this.Value}]";
        }

        /// <summary>
        /// The class that represents the union when both type matches
        /// </summary>
        public sealed class Both : TypeUnion<TLeft, TRight>, IEquatable<Both>
        {
            /// <summary>
            /// The constructor
            /// </summary>
            /// <param name="left">The left part of the union</param>
            /// <param name="right">The right part of the union</param>
            public Both(TLeft left, TRight right)
            {
                this.LeftValue = left;
                this.RightValue = right;
            }

            /// <summary>
            /// The left part of the union
            /// </summary>
            public TLeft LeftValue { get; }

            /// <summary>
            /// The right part of the union
            /// </summary>
            public TRight RightValue { get; }

            /// <summary>
            /// Tests if the value contained in this instance is equal to the one in the other instance
            /// </summary>
            /// <param name="other">The instance to compare to</param>
            /// <returns>true if both instances wraps the same value, according to object.Equals rules</returns>
            public bool Equals(Both other)
            {
                return object.Equals(other.LeftValue, this.LeftValue) && object.Equals(other.RightValue, this.RightValue);
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return (obj is Both other) && this.Equals(other);
            }

            /// <inheritdoc />
            public override int GetHashCode() => HashCode.Combine(this.LeftValue, this.RightValue);

            /// <inheritdoc />
            public override string ToString() => $"Both[{this.LeftValue},{this.RightValue}]";
        }

        /// <summary>
        /// Tests if the value is of the Left type only, and if it is, stores its value into the output parameter and return true
        /// </summary>
        /// <param name="output">The out parameter that will receive the value if the function succeeds</param>
        /// <returns>A value indicating whether this instance is of the Left type</returns>
        public bool TryGetLeft([MaybeNullWhen(false)] out TLeft output)
        {
            if (this is Left value)
            {
                output = value.Value;
                return true;
            }

            output = default;
            return false;
        }

        /// <summary>
        /// Tests if the value is of the Right type only, and if it is, stores its value into the output parameter and return true
        /// </summary>
        /// <param name="output">The out parameter that will receive the value if the function succeeds</param>
        /// <returns>A value indicating whether this instance is of the Right type</returns>
        public bool TryGetRight([MaybeNullWhen(false)] out TRight output)
        {
            if (this is Right value)
            {
                output = value.Value;
                return true;
            }

            output = default;
            return false;
        }

        /// <summary>
        /// Tests if the deserialized value can be store in both types, and if it can, stores the values into the output parameters and return true
        /// </summary>
        /// <param name="left">The out parameter that will receive the value interpreted as the left type if the function succeeds</param>
        /// <param name="right">The out parameter that will receive the value interpreted as the right type if the function succeeds</param>
        /// <returns>A value indicating whether this instance is of the Both type</returns>
        public bool TryGetBoth([MaybeNullWhen(false)] out TLeft left, [MaybeNullWhen(false)] out TRight right)
        {
            if (this is Both value)
            {
                left = value.LeftValue;
                right = value.RightValue;
                return true;
            }

            left = default;
            right = default;
            return false;
        }
    }
}
