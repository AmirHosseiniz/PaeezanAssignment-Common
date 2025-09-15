// Physics/CollisionDetection.cs  (minor additions: AABB build; unchanged narrowphase)
using System;

public struct CollisionInfo
{
    public PhysicsBody BodyA;
    public PhysicsBody BodyB;
    public FixedVector3 ContactPoint;
    public FixedVector3 Normal;
    public Fix64 Penetration;
    public bool IsValid;
    public bool IsTriggerPair;
}

public static class CollisionDetection
{
    public static void BuildAabb(PhysicsBody b)
    {
        switch (b.ColliderType)
        {
            case ColliderType.Sphere:
            {
                var r = b.ColliderSize.x;
                var p = b.WorldPosition;
                var ext = new FixedVector3(r, r, r);
                b.AabbMin = p - ext;
                b.AabbMax = p + ext;
                break;
            }
            case ColliderType.Box:
            default:
            {
                var half = b.ColliderSize * Fix64.Half;
                var p = b.WorldPosition;
                b.AabbMin = p - half;
                b.AabbMax = p + half;
                break;
            }
        }
    }

    public static bool CheckAabbOverlap(PhysicsBody a, PhysicsBody b)
    {
        return !(a.AabbMax.x < b.AabbMin.x || a.AabbMin.x > b.AabbMax.x ||
                 a.AabbMax.y < b.AabbMin.y || a.AabbMin.y > b.AabbMax.y ||
                 a.AabbMax.z < b.AabbMin.z || a.AabbMin.z > b.AabbMax.z);
    }

    public static bool CheckCollision(PhysicsBody a, PhysicsBody b, out CollisionInfo info)
    {
        info = new CollisionInfo { BodyA = a, BodyB = b, IsTriggerPair = a.IsTrigger || b.IsTrigger };

        var posA = a.WorldPosition;
        var posB = b.WorldPosition;

        if (a.ColliderType == ColliderType.Box && b.ColliderType == ColliderType.Box)
            return BoxBoxCollision(a, b, posA, posB, out info);
        if (a.ColliderType == ColliderType.Sphere && b.ColliderType == ColliderType.Sphere)
            return SphereSphereCollision(a, b, posA, posB, out info);
        if (a.ColliderType == ColliderType.Box && b.ColliderType == ColliderType.Sphere)
            return BoxSphereCollision(a, b, posA, posB, out info);
        if (a.ColliderType == ColliderType.Sphere && b.ColliderType == ColliderType.Box)
        {
            var result = BoxSphereCollision(b, a, posB, posA, out info);
            if (result)
            {
                // swap back to keep A/B order as passed in
                var tmpA = info.BodyA; info.BodyA = a; info.BodyB = b; 
                info.Normal = -info.Normal;
            }
            return result;
        }
        return false;
    }

    private static bool BoxBoxCollision(PhysicsBody a, PhysicsBody b, FixedVector3 posA, FixedVector3 posB, out CollisionInfo info)
    {
        info = new CollisionInfo { BodyA = a, BodyB = b, IsTriggerPair = a.IsTrigger || b.IsTrigger };
        var halfA = a.ColliderSize * Fix64.Half;
        var halfB = b.ColliderSize * Fix64.Half;

        var delta = posB - posA;
        var overlap = halfA + halfB - new FixedVector3(Fix64.Abs(delta.x), Fix64.Abs(delta.y), Fix64.Abs(delta.z));

        if (overlap.x <= Fix64.Zero || overlap.y <= Fix64.Zero || overlap.z <= Fix64.Zero)
            return false;

        Fix64 minOverlap = overlap.x;
        FixedVector3 normal = delta.x > Fix64.Zero ? -FixedVector3.Right : FixedVector3.Right;

        if (overlap.y < minOverlap) { minOverlap = overlap.y; normal = delta.y > Fix64.Zero ? -FixedVector3.Up : FixedVector3.Up; }
        if (overlap.z < minOverlap) { minOverlap = overlap.z; normal = delta.z > Fix64.Zero ? -FixedVector3.Forward : FixedVector3.Forward; }

        info.Normal = normal;
        info.Penetration = minOverlap;
        info.ContactPoint = posA + delta * Fix64.Half;
        info.IsValid = true;
        return true;
    }

    private static bool SphereSphereCollision(PhysicsBody a, PhysicsBody b, FixedVector3 posA, FixedVector3 posB, out CollisionInfo info)
    {
        info = new CollisionInfo { BodyA = a, BodyB = b, IsTriggerPair = a.IsTrigger || b.IsTrigger };

        var ra = a.ColliderSize.x;
        var rb = b.ColliderSize.x;
        var rsum = ra + rb;

        var delta = posB - posA;
        var dist2 = delta.SqrMagnitude;
        var r2 = rsum * rsum;

        if (dist2 >= r2) return false;

        var dist = Fix64.Sqrt(dist2);
        info.Normal = dist > Fix64.Zero ? delta / dist : FixedVector3.Right;
        info.Penetration = rsum - dist;
        info.ContactPoint = posA + info.Normal * ra;
        info.IsValid = true;
        return true;
    }

    private static bool BoxSphereCollision(PhysicsBody box, PhysicsBody sphere, FixedVector3 boxPos, FixedVector3 spherePos, out CollisionInfo info)
    {
        info = new CollisionInfo { BodyA = box, BodyB = sphere, IsTriggerPair = box.IsTrigger || sphere.IsTrigger };

        var half = box.ColliderSize * Fix64.Half;
        var r = sphere.ColliderSize.x;

        var closest = new FixedVector3(
            Clamp(spherePos.x, boxPos.x - half.x, boxPos.x + half.x),
            Clamp(spherePos.y, boxPos.y - half.y, boxPos.y + half.y),
            Clamp(spherePos.z, boxPos.z - half.z, boxPos.z + half.z)
        );

        var delta = spherePos - closest;
        var dist2 = delta.SqrMagnitude;
        var r2 = r * r;

        if (dist2 >= r2) return false;

        var dist = Fix64.Sqrt(dist2);
        info.Normal = dist > Fix64.Zero ? delta / dist : FixedVector3.Up;
        info.Penetration = r - dist;
        info.ContactPoint = closest;
        info.IsValid = true;
        return true;
    }

    private static Fix64 Clamp(Fix64 v, Fix64 min, Fix64 max)
    {
        if (v < min) return min;
        if (v > max) return max;
        return v;
    }
}
