#nullable enable

#if UNITY_5_3_OR_NEWER

using UnityEditor;
using UnityEngine;
using System;

namespace Shadop.Archmage.Editor
{
    /// <summary>
    /// Abstract base for config ID property drawers. Renders an <see cref="ArchmageTools.DrawEasyDropdown{TValue}"/>
    /// dropdown in the Inspector. Subclasses provide the ID values and display names, typically
    /// populated from a config table at editor load time.
    /// </summary>
    /// <typeparam name="TValue">The underlying numeric or string type of the config ID.</typeparam>
    public abstract class CfgIdDrawer<TValue> : PropertyDrawer
        where TValue : IComparable<TValue>
    {
        protected abstract TValue[] GetIdValues();
        protected abstract string[] GetDisplayNames();

        /// <summary>
        /// Builds a display-name array from <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The ID values array.</param>
        /// <param name="format">Formatter that converts an ID value to its display string.</param>
        /// <param name="start">Index to start formatting from. Entries before <paramref name="start"/>
        /// are left as <c>null</c> so the caller can fill them manually (e.g., a "Default" label at index 0).</param>
        protected static string[] BuildDisplayNames(TValue[] values, Func<TValue, string> format, int start)
        {
            if (values.Length == 0)
                return Array.Empty<string>();

            var displayNames = new string[values.Length];
            for (var i = start; i < values.Length; i++)
                displayNames[i] = format(values[i]);
            return displayNames;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ArchmageTools.DrawEasyDropdown(
                position, property.FindPropertyRelative("Value"), label,
                GetIdValues(), GetDisplayNames());
        }
    }
}

#endif
