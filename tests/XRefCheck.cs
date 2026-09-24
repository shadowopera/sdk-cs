using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Xunit;

namespace Shadop.Archmage.Sdk.Tests
{
    public partial class AtlasTests
    {
        /// <summary>
        /// Walks every XRef reachable from the ready atlas items and checks that its binding
        /// matches its CfgId: a zero CfgId has a null Ref, and a non-zero CfgId has a Ref
        /// whose Id equals the CfgId. Returns the number of bound refs.
        /// </summary>
        internal static int CheckXRefs(IAtlas atlas)
        {
            var walker = new XRefWalker();
            foreach (var kvp in atlas.AtlasItems())
            {
                if (kvp.Value.Ready)
                {
                    walker.Walk(kvp.Value.Cfg, kvp.Key);
                }
            }

            Assert.True(walker.Errors.Count == 0, string.Join("\n", walker.Errors));
            return walker.Bound;
        }

        class XRefWalker
        {
            readonly HashSet<object> _visited = new(ReferenceEqualityComparer.Instance);
            public List<string> Errors { get; } = new();
            public int Bound { get; private set; }

            public void Walk(object? obj, string path)
            {
                if (obj is null or string)
                    return;

                var type = obj.GetType();
                if (type.IsPrimitive || type.IsEnum)
                    return;
                if (!type.IsValueType && !_visited.Add(obj))
                    return;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(XRef<,>))
                {
                    Check(obj, type, path);
                    return;
                }

                switch (obj)
                {
                    case IDictionary dict:
                        foreach (DictionaryEntry entry in dict)
                            Walk(entry.Value, $"{path}[{entry.Key}]");
                        return;
                    case IEnumerable seq:
                        var i = 0;
                        foreach (var elem in seq)
                            Walk(elem, $"{path}[{i++}]");
                        return;
                }

                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (prop.GetIndexParameters().Length == 0 && prop.IsDefined(typeof(JsonPropertyAttribute)))
                        Walk(prop.GetValue(obj), $"{path}.{prop.Name}");
                }
            }

            void Check(object xref, Type type, string path)
            {
                var cfgId = type.GetProperty("CfgId")!.GetValue(xref)!;
                var target = type.GetProperty("Ref")!.GetValue(xref);
                if (((IZero)cfgId).IsZero)
                {
                    if (target is not null)
                        Errors.Add($"{path}: zero CfgId but Ref is bound");
                }
                else if (target is null)
                {
                    Errors.Add($"{path}: CfgId {cfgId} but Ref is null");
                }
                else
                {
                    var id = target.GetType().GetProperty("Id")!.GetValue(target);
                    if (!cfgId.Equals(id))
                        Errors.Add($"{path}: CfgId {cfgId} but Ref.Id is {id}");
                    Bound++;
                }
            }
        }
    }
}
