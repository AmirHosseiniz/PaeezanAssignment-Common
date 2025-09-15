// Physics/CollisionLayers.cs
using System;

[Flags]
public enum CollisionLayer : uint
{
    None        = 0,
    Default     = 1 << 0,
    Unit        = 1 << 1,
    Tower       = 1 << 2,
    Projectile  = 1 << 3,
    Terrain     = 1 << 4,
    // add more as needed
}

public sealed class CollisionMatrix
{
    // 32-bit mask lets you toggle which layers can interact
    private readonly uint[] _matrix = new uint[32];

    public CollisionMatrix Allow(CollisionLayer a, CollisionLayer b)
    {
        int ia = BitIndex(a); int ib = BitIndex(b);
        _matrix[ia] |= (uint)(1u << ib);
        _matrix[ib] |= (uint)(1u << ia);
        return this;
    }

    public CollisionMatrix Deny(CollisionLayer a, CollisionLayer b)
    {
        int ia = BitIndex(a); int ib = BitIndex(b);
        _matrix[ia] &= ~(uint)(1u << ib);
        _matrix[ib] &= ~(uint)(1u << ia);
        return this;
    }

    public bool ShouldCollide(CollisionLayer a, CollisionLayer b)
    {
        int ia = BitIndex(a); int ib = BitIndex(b);
        return ((_matrix[ia] >> ib) & 1u) != 0u;
    }

    private static int BitIndex(CollisionLayer l)
    {
        uint v = (uint)l;
        if (v == 0) return 0;
        int idx = 0;
        while ((v & 1u) == 0u) { v >>= 1; idx++; }
        return idx;
    }

    public static CollisionMatrix CreateDefault()
    {
        var m = new CollisionMatrix();
        // default sensible rules for lane battler:
        m.Allow(CollisionLayer.Unit, CollisionLayer.Unit);
        m.Allow(CollisionLayer.Unit, CollisionLayer.Tower);
        m.Allow(CollisionLayer.Projectile, CollisionLayer.Unit);
        m.Allow(CollisionLayer.Projectile, CollisionLayer.Tower);
        m.Allow(CollisionLayer.Terrain, CollisionLayer.Unit);
        m.Allow(CollisionLayer.Terrain, CollisionLayer.Projectile);
        return m;
    }
}