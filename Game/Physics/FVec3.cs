namespace PaeezanAssignment_Server.Common.Game.Physics;

[Serializable]
public struct FVec3
{
    public Fix64 x, y, z;
    public FixedVector3 ToFixed() => new FixedVector3(x, y, z);
}