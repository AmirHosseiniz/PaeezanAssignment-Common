using System;

namespace PaeezanAssignment_Server.Common.Game.Physics
{
    public struct FixedVector3
    {
        public Fix64 x;
        public Fix64 y;
        public Fix64 z;

        public FixedVector3(Fix64 x, Fix64 y, Fix64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public FixedVector3(float x, float y, float z)
        {
            this.x = Fix64.FromFloat(x);
            this.y = Fix64.FromFloat(y);
            this.z = Fix64.FromFloat(z);
        }

        // Static constants
        public static FixedVector3 Zero => new FixedVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
        public static FixedVector3 One => new FixedVector3(Fix64.One, Fix64.One, Fix64.One);
        public static FixedVector3 Up => new FixedVector3(Fix64.Zero, Fix64.One, Fix64.Zero);
        public static FixedVector3 Down => new FixedVector3(Fix64.Zero, -Fix64.One, Fix64.Zero);
        public static FixedVector3 Left => new FixedVector3(-Fix64.One, Fix64.Zero, Fix64.Zero);
        public static FixedVector3 Right => new FixedVector3(Fix64.One, Fix64.Zero, Fix64.Zero);
        public static FixedVector3 Forward => new FixedVector3(Fix64.Zero, Fix64.Zero, Fix64.One);
        public static FixedVector3 Back => new FixedVector3(Fix64.Zero, Fix64.Zero, -Fix64.One);

        // Arithmetic operators
        public static FixedVector3 operator +(FixedVector3 a, FixedVector3 b)
        {
            return new FixedVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static FixedVector3 operator -(FixedVector3 a, FixedVector3 b)
        {
            return new FixedVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static FixedVector3 operator -(FixedVector3 a)
        {
            return new FixedVector3(-a.x, -a.y, -a.z);
        }

        public static FixedVector3 operator *(FixedVector3 a, Fix64 scalar)
        {
            return new FixedVector3(a.x * scalar, a.y * scalar, a.z * scalar);
        }

        public static FixedVector3 operator *(Fix64 scalar, FixedVector3 a)
        {
            return new FixedVector3(scalar * a.x, scalar * a.y, scalar * a.z);
        }

        public static FixedVector3 operator /(FixedVector3 a, Fix64 scalar)
        {
            if (scalar == Fix64.Zero)
                throw new DivideByZeroException("Cannot divide vector by zero");
            return new FixedVector3(a.x / scalar, a.y / scalar, a.z / scalar);
        }

        // Comparison operators
        public static bool operator ==(FixedVector3 a, FixedVector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(FixedVector3 a, FixedVector3 b)
        {
            return !(a == b);
        }

        // Vector operations
        public Fix64 Magnitude => Fix64.Sqrt(x * x + y * y + z * z);

        public Fix64 SqrMagnitude => x * x + y * y + z * z;

        public FixedVector3 Normalized
        {
            get
            {
                Fix64 mag = Magnitude;
                if (mag == Fix64.Zero)
                    return Zero;
                return this / mag;
            }
        }

        public void Normalize()
        {
            Fix64 mag = Magnitude;
            if (mag != Fix64.Zero)
            {
                x /= mag;
                y /= mag;
                z /= mag;
            }
        }

        public static Fix64 Dot(FixedVector3 a, FixedVector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static FixedVector3 Cross(FixedVector3 a, FixedVector3 b)
        {
            return new FixedVector3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }

        public static Fix64 Distance(FixedVector3 a, FixedVector3 b)
        {
            return (a - b).Magnitude;
        }

        public static Fix64 DistanceSqr(FixedVector3 a, FixedVector3 b)
        {
            return (a - b).SqrMagnitude;
        }

        public static FixedVector3 Lerp(FixedVector3 a, FixedVector3 b, Fix64 t)
        {
            t = Fix64.Clamp(t, Fix64.Zero, Fix64.One);
            return a + (b - a) * t;
        }

        public static FixedVector3 MoveTowards(FixedVector3 current, FixedVector3 target, Fix64 maxDistanceDelta)
        {
            FixedVector3 direction = target - current;
            Fix64 distance = direction.Magnitude;

            if (distance <= maxDistanceDelta || distance == Fix64.Zero)
                return target;

            return current + direction / distance * maxDistanceDelta;
        }

        public static FixedVector3 Reflect(FixedVector3 inDirection, FixedVector3 inNormal)
        {
            return inDirection - Fix64.FromInt(2) * Dot(inDirection, inNormal) * inNormal;
        }

        // Utility methods
        public FixedVector3 WithX(Fix64 newX) => new FixedVector3(newX, y, z);
        public FixedVector3 WithY(Fix64 newY) => new FixedVector3(x, newY, z);
        public FixedVector3 WithZ(Fix64 newZ) => new FixedVector3(x, y, newZ);

        // Override methods
        public override bool Equals(object obj)
        {
            return obj is FixedVector3 other && this == other;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x.GetHashCode(), y.GetHashCode(), z.GetHashCode());
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public string ToString(string format)
        {
            return
                $"({x.ToDouble().ToString(format)}, {y.ToDouble().ToString(format)}, {z.ToDouble().ToString(format)})";
        }
    }
}