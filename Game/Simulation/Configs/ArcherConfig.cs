using System;
using PaeezanAssignment_Server.Common.Game.Physics;

namespace PaeezanAssignment_Server.Common.Game.Simulation
{
    [Serializable]
    public sealed class ArcherConfig
    {
        public Fix64 hp { get; set; } = Fix64.FromInt(80);
        public Fix64 speed { get; set; } = Fix64.FromFloat(2.5f);
        public Fix64 damage { get; set; } = Fix64.FromInt(15);
        public Fix64 range { get; set; } = Fix64.FromInt(6);
        public Fix64 cooldown { get; set; } = Fix64.FromFloat(1.2f);
        public Fix64 radius { get; set; } = Fix64.FromFloat(0.5f);
        public Fix64 projectileSpeed { get; set; } = Fix64.FromInt(12);
        public int countPerSide { get; set; } = 0;
    }
}