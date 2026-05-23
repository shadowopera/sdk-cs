#nullable enable

#if UNITY_5_3_OR_NEWER

using UnityEngine;

namespace Shadop.Archmage.Sdk
{
    public static class VecExtensions
    {
        #region Vec2 Extensions

        public static Vector2Int ToVector2Int(this Vec2<byte> vec) => new (vec.X, vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<sbyte> vec) => new (vec.X, vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<short> vec) => new (vec.X, vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<ushort> vec) => new (vec.X, vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<int> vec) => new (vec.X, vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<uint> vec) => new ((int)vec.X, (int)vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<long> vec) => new ((int)vec.X, (int)vec.Y);
        public static Vector2Int ToVector2Int(this Vec2<ulong> vec) => new ((int)vec.X, (int)vec.Y);

        public static Vector2 ToVector2(this Vec2<float> vec) => new (vec.X, vec.Y);
        public static Vector2 ToVector2(this Vec2<double> vec) => new ((float)vec.X, (float)vec.Y);

        #endregion

        #region Vec3 Extensions

        public static Vector3Int ToVector3Int(this Vec3<byte> vec) => new (vec.X, vec.Y, vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<sbyte> vec) => new (vec.X, vec.Y, vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<short> vec) => new (vec.X, vec.Y, vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<ushort> vec) => new (vec.X, vec.Y, vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<int> vec) => new (vec.X, vec.Y, vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<uint> vec) => new ((int)vec.X, (int)vec.Y, (int)vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<long> vec) => new ((int)vec.X, (int)vec.Y, (int)vec.Z);
        public static Vector3Int ToVector3Int(this Vec3<ulong> vec) => new ((int)vec.X, (int)vec.Y, (int)vec.Z);

        public static Vector3 ToVector3(this Vec3<float> vec) => new (vec.X, vec.Y, vec.Z);
        public static Vector3 ToVector3(this Vec3<double> vec) => new ((float)vec.X, (float)vec.Y, (float)vec.Z);

        #endregion

        #region Vec4 Extensions

        public static Vector4 ToVector4(this Vec4<float> vec) => new (vec.X, vec.Y, vec.Z, vec.W);
        public static Vector4 ToVector4(this Vec4<double> vec) => new ((float)vec.X, (float)vec.Y, (float)vec.Z, (float)vec.W);

        #endregion
    }
}

#endif
