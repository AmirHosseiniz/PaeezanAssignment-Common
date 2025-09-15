namespace PaeezanAssignment_Server.Common.Game.Simulation;

[Serializable]
public sealed class MapConfig
{
    public Fix64 width { get; set; } = Fix64.FromInt(30); // world units along X
    public Fix64 height { get; set; } = Fix64.FromInt(10); // world units along Z (or Y if 2D)
    public Fix64 laneY { get; set; } = Fix64.Zero; // keep everything on Y=0 plane
}