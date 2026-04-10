#nullable enable

#if UNITY_5_3_OR_NEWER

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

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
        protected abstract string GetHeader();
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
                GetHeader(),
                GetIdValues(), GetDisplayNames());
        }
    }

    /// <summary>
    /// Generic base for config ID property drawers backed by a <see cref="Dictionary{TId,TCfg}"/> table
    /// whose raw value is an unmanaged numeric type.
    /// Manages the static ID-value and display-name arrays; subclasses populate them by calling <see cref="Initialize"/>.
    /// </summary>
    /// <typeparam name="TId">The config ID struct type (e.g. <c>HeroCfgId</c>).</typeparam>
    /// <typeparam name="TValue">The underlying unmanaged value type (e.g. <c>long</c>).</typeparam>
    public abstract class IntCfgIdDrawer<TId, TValue> : CfgIdDrawer<TValue>
        where TValue : unmanaged, IComparable<TValue>
    {
        // Static fields are per generic instantiation, so each concrete drawer type gets its own copy.
        static string _header = string.Empty;
        static TValue[]? _idValues;
        static string[]? _displayNames;

        protected override string GetHeader() => _header;
        protected override TValue[] GetIdValues() => _idValues ?? Array.Empty<TValue>();
        protected override string[] GetDisplayNames() => _displayNames ?? Array.Empty<string>();

        /// <summary>
        /// Populates the static ID-value and display-name arrays from a table's ID collection.
        /// Index 0 is reserved for a "0 (Default)" entry; remaining entries are sorted ascending.
        /// </summary>
        /// <param name="header">The header text displayed at the top of the dropdown window.</param>
        /// <param name="ids">The ID collection of the config table.</param>
        /// <param name="id2value">Extracts the raw value from a config ID (e.g. <c>id => id.Value</c>).</param>
        /// <param name="format">Formats a raw value into its display string.</param>
        protected static void Initialize(string header, ICollection<TId> ids, Func<TId, TValue> id2value, Func<TValue, string> format)
        {
            var i = 1;
            _header = header;
            _idValues = ids.Count > 0 ? new TValue[ids.Count + 1] : Array.Empty<TValue>();
            foreach (var id in ids)
                _idValues[i++] = id2value(id);
            Array.Sort(_idValues);
            _displayNames = BuildDisplayNames(_idValues, format, 1);
            if (_displayNames.Length > 0)
                _displayNames[0] = "0 (Default)";
        }
    }

    /// <summary>
    /// Generic base for config ID property drawers backed by a <see cref="Dictionary{TId,TCfg}"/> table
    /// whose raw value is a <see cref="string"/>.
    /// Manages the static ID-value and display-name arrays; subclasses populate them by calling <see cref="Initialize"/>.
    /// </summary>
    /// <typeparam name="TId">The config ID struct type (e.g. <c>RaceCfgId</c>).</typeparam>
    public abstract class StrCfgIdDrawer<TId> : CfgIdDrawer<string>
    {
        // Static fields are per generic instantiation, so each concrete drawer type gets its own copy.
        static string _header = string.Empty;
        static string[]? _idValues;
        static string[]? _displayNames;

        protected override string GetHeader() => _header;
        protected override string[] GetIdValues() => _idValues ?? Array.Empty<string>();
        protected override string[] GetDisplayNames() => _displayNames ?? Array.Empty<string>();

        /// <summary>
        /// Populates the static ID-value and display-name arrays from a table's ID collection.
        /// Index 0 is reserved for a "\"\" (Default)" entry; remaining entries are sorted ascending.
        /// </summary>
        /// <param name="header">The header text displayed at the top of the dropdown window.</param>
        /// <param name="ids">The ID collection of the config table.</param>
        /// <param name="id2value">Extracts the raw value from a config ID (e.g. <c>id => id.Value</c>).</param>
        /// <param name="format">Formats a raw value into its display string.</param>
        protected static void Initialize(string header, ICollection<TId> ids, Func<TId, string> id2value, Func<string, string> format)
        {
            var i = 1;
            _header = header;
            _idValues = ids.Count > 0 ? new string[ids.Count + 1] : Array.Empty<string>();
            foreach (var id in ids)
                _idValues[i++] = id2value(id);
            _idValues[0] = string.Empty;
            Array.Sort(_idValues);
            _displayNames = BuildDisplayNames(_idValues, format, 1);
            if (_displayNames.Length > 0)
                _displayNames[0] = "\"\" (Default)";
        }
    }
}

#endif
