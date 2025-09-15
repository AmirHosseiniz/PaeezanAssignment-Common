using System;
using PaeezanAssignment_Server.Common.Game.Physics;

namespace PaeezanAssignment_Server.Common.Game.Simulation
{
    [Serializable]
    public sealed class TowerConfig
    {
        public Fix64 hp { get; set; } = Fix64.FromInt(500);

        public FVec3 size { get; set; } =
            new FVec3 { x = Fix64.FromInt(2), y = Fix64.FromInt(5), z = Fix64.FromInt(2) };

        public Fix64 halfGapToEdge { get; set; } = Fix64.FromInt(3); // distance from map edge
    }
}