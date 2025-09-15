// Physics/PhysicsBody.cs  (updated)
using System;

public enum BodyType { Static, Kinematic, Dynamic }
public enum ColliderType { Box, Sphere, Capsule }

// Keep API surface similar but add Layer/IsTrigger and Cached AABB for broadphase
public class PhysicsBody
{
    public int Id { get; internal set; }
    public BodyType Type { get; set; }
    public string Tag { get; set; }

    public FixedVector3 Position { get; set; }
    public FixedVector3 Rotation { get; set; }
    public FixedVector3 Scale { get; set; } = FixedVector3.One;

    public FixedVector3 Velocity { get; set; }
    public FixedVector3 AngularVelocity { get; set; }
    public Fix64 Mass { get; set; } = Fix64.One;
    public Fix64 Drag { get; set; } = Fix64.FromFloat(0.1f);
    public Fix64 AngularDrag { get; set; } = Fix64.FromFloat(0.1f);
    public bool UseGravity { get; set; }

    public ColliderType ColliderType { get; set; }
    public FixedVector3 ColliderSize { get; set; }  // for Box: size; Sphere: radius in x; Capsule: radiusX,heightY
    public FixedVector3 ColliderCenter { get; set; }
    public bool IsTrigger { get; set; }             // triggers raise events but skip physical resolution
    public CollisionLayer Layer { get; set; } = CollisionLayer.Default;

    public FixedVector3 WorldPosition => Position + ColliderCenter;

    // Cached bounds for broadphase (updated per step)
    internal FixedVector3 AabbMin, AabbMax;
}