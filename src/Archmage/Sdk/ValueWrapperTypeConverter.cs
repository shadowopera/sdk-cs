#nullable enable

using System;
using System.ComponentModel;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Type converter for value wrapper structs that converts between <typeparamref name="T"/> and <see cref="string"/>.
    /// </summary>
    /// <typeparam name="T">The value wrapper struct type.</typeparam>
    /// <typeparam name="V">The underlying value type.</typeparam>
    public abstract class ValueWrapperTypeConverter<T, V> : TypeConverter
        where T : struct
        where V : IConvertible
    {
        protected abstract T Create(V value);

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
            => Create((V)Convert.ChangeType((string)value, typeof(V))!);

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            => destinationType == typeof(string);

        public override object? ConvertTo(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
            => (value is T obj) ? obj.ToString() : null;
    }
}
