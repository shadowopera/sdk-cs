#nullable enable

using System;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// JSON converter for value wrapper structs that hold a single <typeparamref name="V"/> value.
    /// </summary>
    /// <typeparam name="T">The value wrapper struct type.</typeparam>
    /// <typeparam name="V">The underlying value type.</typeparam>
    public abstract class ValueWrapperJsonConverter<T, V> : JsonConverter<T>
        where T : struct
        where V : IConvertible
    {
        protected abstract T Create(V value);
        protected abstract V GetValue(T obj);

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
            => reader.Value is null ? default : reader.Value is V v ?
            Create(v) : Create((V)Convert.ChangeType(reader.Value, typeof(V))!);

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
            => writer.WriteValue(GetValue(value));
    }
}
