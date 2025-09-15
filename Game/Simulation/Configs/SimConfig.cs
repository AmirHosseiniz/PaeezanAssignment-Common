namespace PaeezanAssignment_Server.Common.Game.Simulation;

[Serializable]
public sealed class SimConfig
{
    public MapConfig map { get; set; } = new MapConfig();
    public TowerConfig towers { get; set; } = new TowerConfig();
    public MeleeConfig melee { get; set; } = new MeleeConfig();
    public ArcherConfig archer { get; set; } = new ArcherConfig();
}