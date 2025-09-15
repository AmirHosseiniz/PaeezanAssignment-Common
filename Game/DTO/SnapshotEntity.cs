using PaeezanAssignment_Server.Common.Game.Entity;

namespace PaeezanAssignment_Server.Common.Game.DTO;

public struct SnapshotEntity
{
    public int Id;
    public EntityType Type;
    public int Owner;
    public long Px, Py, Pz; // Fix64 raw
    public long Vx, Vy, Vz; // Fix64 raw
    public long HP; // Fix64 raw
}   