using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Shadop.Archmage;

/// <summary>
/// Provides utility functions.
/// </summary>
public static partial class Archmage
{
    /// <summary>
    /// Merges source JSON into target object following RFC 7396 merge semantics.
    /// - Non-object types: source replaces target
    /// - Objects: recursively merge members
    /// - Dictionaries: merge keys, recursing for object/dictionary values
    /// </summary>
    public static void Merge(object target, JToken sourceJson)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        // Handle dictionaries (including Dictionary<string, T>)
        if (target is IDictionary dict)
        {
            MergeDictionary(dict, sourceJson);
            return;
        }

        // Handle regular objects
        MergeObject(target, sourceJson);
    }

    private static void MergeDictionary(IDictionary target, JToken sourceJson)
    {
        if (sourceJson.Type != JTokenType.Object)
            throw new ArchmageException("Cannot merge non-object JSON into dictionary");

        var dictType = target.GetType();
        var genericArgs = dictType.IsGenericType ? dictType.GetGenericArguments() : null;
        var valueType = genericArgs?.Length == 2 ? genericArgs[1] : typeof(object);

        foreach (var property in ((JObject)sourceJson).Properties())
        {
            var key = property.Name;
            var sourceValue = property.Value;

            // Check if key exists in target
            if (target.Contains(key))
            {
                var existingValue = target[key];

                // If both are objects/dictionaries, recurse
                if (existingValue != null &&
                    (existingValue is IDictionary || IsComplexObject(existingValue)) &&
                    sourceValue.Type == JTokenType.Object)
                {
                    Merge(existingValue, sourceValue);
                }
                else
                {
                    // Replace with new value
                    target[key] = DeserializeJToken(sourceValue, valueType);
                }
            }
            else
            {
                // Insert new key
                target[key] = DeserializeJToken(sourceValue, valueType);
            }
        }
    }

    private static void MergeObject(object target, JToken sourceJson)
    {
        if (sourceJson.Type != JTokenType.Object)
            throw new ArchmageException("Cannot merge non-object JSON into object");

        var targetType = target.GetType();
        var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var jsonProperty in ((JObject)sourceJson).Properties())
        {
            // Try to find matching property (case-insensitive)
            if (!properties.TryGetValue(jsonProperty.Name, out var propertyInfo))
                continue;

            var sourceValue = jsonProperty.Value;
            var currentValue = propertyInfo.GetValue(target);

            // If both are objects/dictionaries, recurse
            if (currentValue != null &&
                (currentValue is IDictionary || IsComplexObject(currentValue)) &&
                sourceValue.Type == JTokenType.Object)
            {
                Merge(currentValue, sourceValue);
            }
            else
            {
                // Replace with new value (including explicit null)
                var newValue = DeserializeJToken(sourceValue, propertyInfo.PropertyType);
                propertyInfo.SetValue(target, newValue);
            }
        }
    }

    private static bool IsComplexObject(object obj)
    {
        var type = obj.GetType();

        // Exclude primitive types, strings, and common value types
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) ||
            type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(Guid))
            return false;

        // Exclude arrays and lists
        if (type.IsArray || obj is IList)
            return false;

        // It's a complex object if it has settable properties
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Any(p => p.CanWrite);
    }

    private static object? DeserializeJToken(JToken token, Type targetType)
    {
        if (token.Type == JTokenType.Null)
            return null;

        return token.ToObject(targetType);
    }
}
