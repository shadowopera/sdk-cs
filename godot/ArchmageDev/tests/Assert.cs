using System;
using System.Collections.Generic;

namespace ArchmageDev.Tests
{
    public sealed class AssertionException : Exception
    {
        public AssertionException(string message) : base(message)
        {
        }
    }

    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual, string? message = null)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                Fail($"Expected: {expected}\nBut was:  {actual}", message);
        }

        public static void AreNotEqual<T>(T notExpected, T actual, string? message = null)
        {
            if (EqualityComparer<T>.Default.Equals(notExpected, actual))
                Fail($"Expected a value other than: {notExpected}", message);
        }

        public static void IsTrue(bool condition, string? message = null)
        {
            if (!condition)
                Fail("Expected: True\nBut was:  False", message);
        }

        public static void IsNotNull(object? value, string? message = null)
        {
            if (value is null)
                Fail("Expected: not null\nBut was:  null", message);
        }

        static void Fail(string detail, string? message)
        {
            throw new AssertionException(message is null ? detail : $"{message}\n{detail}");
        }
    }
}
