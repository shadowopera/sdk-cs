#nullable enable

#if UNITY_5_3_OR_NEWER

using UnityEngine;

namespace Shadop.Archmage.Sdk
{
    public static class VecExtensions
    {
        #region Vec2 Extensions

        public static Vector2Int ToVector2Int(this Vec2<byte> vec) => new Vector2Int((int)vec.X, (int)vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<sbyte> vec) => new Vector2Int((int)vec.X, (int)vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<short> vec) => new Vector2Int((int)vec.X, (int)vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<ushort> vec) => new Vector2Int((int)vec.X, (int)vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<int> vec) => new Vector2Int(vec.X, vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<uint> vec) => new Vector2Int((int)vec.X, (int)vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<long> vec) => new Vector2Int((int)vec.X, (int)vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<ulong> vec) => new Vector2Int((int)vec.X, (int)vec.Y);

        public static Vector2 ToVector2(this Vec2<float> vec) => new Vector2(vec.X, vec.Y);
        public static Vector2 ToVector2(this Vec2<double> vec) => new Vector2((float)vec.X, (float)vec.Y);

        #endregion

        #region Vec3 Extensions

        public static Vector3Int ToVector3Int(this Vec3<byte> vec) => new Vector3Int((int)vec.X, (int)vec.Y, (int)vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<sbyte> vec) => new Vector3Int((int)vec.X, (int)vec.Y, (int)vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<short> vec) => new Vector3Int((int)vec.X, (int)vec.Y, (int)vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<ushort> vec) => new Vector3Int((int)vec.X, (int)vec.Y, (int)vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<int> vec) => new Vector3Int(vec.X, vec.Y, vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<uint> vec) => new Vector3Int((int)vec.X, (int)vec.Y, (int)vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<long> vec) => new Vector3Int((int)vec.X, (int)vec.Y, (int)vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<ulong> vec) => new Vector3Int((int)vec.X, (int)vec.Y, (int)vec.Z);

        public static Vector3 ToVector3(this Vec3<float> vec) => new Vector3(vec.X, vec.Y, vec.Z);
        public static Vector3 ToVector3(this Vec3<double> vec) => new Vector3((float)vec.X, (float)vec.Y, (float)vec.Z);

        #endregion

        #region Vec4 Extensions

        public static Vector4 ToVector4(this Vec4<float> vec) => new Vector4(vec.X, vec.Y, vec.Z, vec.W);
        public static Vector4 ToVector4(this Vec4<double> vec) => new Vector4((float)vec.X, (float)vec.Y, (float)vec.Z, (float)vec.W);

        #endregion
    }
}

#endif
