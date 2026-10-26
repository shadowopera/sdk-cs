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

        public override object ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value)
            => Create((V)Convert.ChangeType((string)value!, typeof(V)));

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            => destinationType == typeof(string);

        public override object? ConvertTo(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
            => (value is T obj) ? obj.ToString() : null;

        /// <summary>
        /// Makes <see cref="TypeDescriptor.GetConverter(Type)"/> return this converter for <typeparamref name="T"/>.
        /// The generated <c>ConfigAtlas</c> calls it in its static constructor.
        /// </summary>
        /// <remarks>
        /// Newtonsoft.Json reads dictionary keys of type <typeparamref name="T"/> with the converter that
        /// <see cref="TypeDescriptor"/> returns. The <c>[TypeConverter]</c> attribute on <typeparamref name="T"/> names
        /// the converter type, but <see cref="TypeDescriptor"/> cannot find that type when its assembly is not loaded in
        /// the default <c>AssemblyLoadContext</c>, as in Godot.
        /// </remarks>
        public void Register()
        {
            TypeDescriptor.AddProvider(new ConverterProvider(TypeDescriptor.GetProvider(typeof(T)), this), typeof(T));
        }

        sealed class ConverterProvider : TypeDescriptionProvider
        {
            readonly TypeConverter _converter;

            public ConverterProvider(TypeDescriptionProvider parent, TypeConverter converter) : base(parent)
            {
                _converter = converter;
            }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object? instance)
            {
                return new ConverterDescriptor(base.GetTypeDescriptor(objectType, instance), _converter);
            }
        }

        sealed class ConverterDescriptor : CustomTypeDescriptor
        {
            readonly TypeConverter _converter;

            public ConverterDescriptor(ICustomTypeDescriptor? parent, TypeConverter converter) : base(parent)
            {
                _converter = converter;
            }

            public override TypeConverter GetConverter() => _converter;
        }
    }
}
