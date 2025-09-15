namespace PaeezanAssignment_Server.Common.Game.DTO
{
    public struct Snapshot
    {
        public uint Frame;
        public int Winner; // -1 = ongoing, 0 = left wins, 1 = right wins
        public SnapshotEntity[] Entities; // sorted by Id for determinism
    }
}