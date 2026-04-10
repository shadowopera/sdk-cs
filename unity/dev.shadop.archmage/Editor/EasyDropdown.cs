#nullable enable

#if UNITY_5_3_OR_NEWER

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Shadop.Archmage.Editor
{
    public static partial class ArchmageTools
    {
        /// <summary>
        /// Draws a strongly-typed AdvancedDropdown for configurations with automatic serialization.
        /// </summary>
        /// <typeparam name="TValue">The underlying data type of the ID (supports: sbyte, byte, short, ushort, int, uint, long, ulong, string).</typeparam>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">SerializedProperty of the field to modify (must contain a value).</param>
        /// <param name="label">The label of the property.</param>
        /// <param name="values">The array of configuration ID values.</param>
        /// <param name="displayNames">Optional: custom display strings. Length must match values if provided.</param>
        /// <param name="minWindowSize">Optional: minimum size for dropdown window.</param>
        public static void DrawEasyDropdown<TValue>(
            Rect position, SerializedProperty property, GUIContent label,
            TValue[] values, string[]? displayNames = null,
            Vector2 minWindowSize = default)
        {
            EditorGUI.BeginProperty(position, label, property);
            try
            {
                DrawDropdown(position, property, label, values, displayNames, minWindowSize);
            }
            finally
            {
                EditorGUI.EndProperty();
            }
        }

        static void DrawDropdown<TValue>(
            Rect position, SerializedProperty property, GUIContent label,
            TValue[]? values, string[]? displayNames,
            Vector2 minWindowSize)
        {
            EditorGUI.PrefixLabel(position, label);
            Rect fieldRect = new(
                position.x + EditorGUIUtility.labelWidth - 2,
                position.y,
                position.width - EditorGUIUtility.labelWidth + 2,
                position.height);

            // Fallback to default property field if values is not ready or empty.
            if (values is null || values.Length == 0)
            {
                fieldRect.x++;
                fieldRect.width -= 1;
                EditorGUI.PropertyField(fieldRect, property, GUIContent.none);
                return;
            }

            // Determine the text to display on the dropdown button.
            var currentValue = GetPropertyValue<TValue>(property);
            var buttonText = string.Empty;
            var found = false;
            for (int i = 0; i < values.Length; i++)
            {
                if (EqualityComparer<TValue>.Default.Equals(values[i], currentValue))
                {
                    var useStrRepr = displayNames is not null && displayNames.Length == values.Length;
                    buttonText = useStrRepr ? displayNames![i] : $"{values[i]}";
                    buttonText = buttonText.Length > 0 ? buttonText : "\"\"";
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                var str = $"{currentValue}";
                buttonText = str.Length > 0 ? $"<unknown: {str}>" : $"<unknown: \"\">";
            }

            // Draw the button. If clicked, instantiate and show the AdvancedDropdown.
            if (EditorGUI.DropdownButton(fieldRect, new GUIContent(buttonText), FocusType.Keyboard))
            {
                var dropdown = new EasyDropdown<TValue>(
                    new AdvancedDropdownState(),
                    property,
                    values, displayNames,
                    minWindowSize
                );

                dropdown.Show(fieldRect);
            }
        }

        /// <summary>
        /// Gets the current value from SerializedProperty based on the generic type.
        /// </summary>
        static TValue GetPropertyValue<TValue>(SerializedProperty property)
        {
            var type = typeof(TValue);

            if (type == typeof(sbyte))
                return (TValue)(object)(sbyte)property.intValue;
            if (type == typeof(short))
                return (TValue)(object)(short)property.intValue;
            if (type == typeof(int))
                return (TValue)(object)property.intValue;
            if (type == typeof(long))
                return (TValue)(object)property.longValue;
            if (type == typeof(byte))
                return (TValue)(object)(byte)property.uintValue;
            if (type == typeof(ushort))
                return (TValue)(object)(ushort)property.uintValue;
            if (type == typeof(uint))
                return (TValue)(object)property.uintValue;
            if (type == typeof(ulong))
                return (TValue)(object)property.ulongValue;
            if (type == typeof(string))
                return (TValue)(object)property.stringValue;

            throw new NotSupportedException($"TValue type {type.Name} is not supported.");
        }

        /// <summary>
        /// Sets the value in SerializedProperty based on the generic type.
        /// </summary>
        static void SetPropertyValue<TValue>(SerializedProperty property, TValue value)
        {
            var type = typeof(TValue);

            if (type == typeof(sbyte))
                property.intValue = (sbyte)(object)value!;
            else if (type == typeof(short))
                property.intValue = (short)(object)value!;
            else if (type == typeof(int))
                property.intValue = (int)(object)value!;
            else if (type == typeof(long))
                property.longValue = (long)(object)value!;
            else if (type == typeof(byte))
                property.uintValue = (byte)(object)value!;
            else if (type == typeof(ushort))
                property.uintValue = (ushort)(object)value!;
            else if (type == typeof(uint))
                property.uintValue = (uint)(object)value!;
            else if (type == typeof(ulong))
                property.ulongValue = (ulong)(object)value!;
            else if (type == typeof(string))
                property.stringValue = (string)(object)value!;
            else
                throw new NotSupportedException($"TValue type {type.Name} is not supported.");

            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Custom AdvancedDropdown implementation for configuration selection.
        /// </summary>
        private class EasyDropdown<TValue> : AdvancedDropdown
        {
            readonly TValue[] _values;
            readonly string[] _displayNames;
            readonly SerializedProperty _property;

            public EasyDropdown(
                AdvancedDropdownState state,
                SerializedProperty property,
                TValue[] values, string[]? displayNames,
                Vector2 minWindowSize) : base(state)
            {
                if (values is null)
                    throw new ArgumentNullException(nameof(values));

                _values = values;
                _property = property;

                // Pre-calculate display names
                var useStrRepr = displayNames is not null && displayNames.Length == _values.Length;
                if (useStrRepr)
                {
                    _displayNames = displayNames!;
                }
                else
                {
                    _displayNames = new string[_values.Length];
                    for (int i = 0; i < _values.Length; i++)
                    {
                        var str = $"{_values[i]}";
                        _displayNames[i] = str.Length > 0 ? str : "\"\"";
                    }
                }

                // Set minimum size for the dropdown window
                minimumSize = minWindowSize == default ? new Vector2(180, 260) : minWindowSize;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var root = new AdvancedDropdownItem("root");

                // Build the dropdown tree with index stored in id
                for (int i = 0; i < _displayNames.Length; i++)
                {
                    var child = new AdvancedDropdownItem(_displayNames[i]);
                    root.AddChild(child);
                    // Must set after root.AddChild!
                    child.id = i;
                }

                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                // Use id to reliably get the index (avoids issues with duplicate display names)
                if (item.id >= 0 && item.id < _values.Length)
                    SetPropertyValue(_property, _values[item.id]);
            }
        }
    }
}

#endif
