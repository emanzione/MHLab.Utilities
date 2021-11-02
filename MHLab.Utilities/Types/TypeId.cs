using System;

namespace MHLab.Utilities.Types
{
    public readonly struct TypeId : IEquatable<TypeId>
    {
        private readonly int _id;

        internal TypeId(int id)
        {
            _id = id;
        }
        
        public bool Equals(TypeId other)
        {
            return _id == other._id;
        }

        public override bool Equals(object obj)
        {
            return obj is TypeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static bool operator ==(TypeId left, TypeId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TypeId left, TypeId right)
        {
            return !left.Equals(right);
        }
    }
}