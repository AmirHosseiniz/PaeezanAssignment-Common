namespace PaeezanAssignment_Server.Common.Game.Simulation;

[Serializable]
public sealed class MeleeConfig
{
    public Fix64 hp { get; set; } = Fix64.FromInt(100);
    public Fix64 speed { get; set; } = Fix64.FromInt(3); // units / sec
    public Fix64 damage { get; set; } = Fix64.FromInt(20);
    public Fix64 range { get; set; } = Fix64.FromFloat(1.5f);
    public Fix64 cooldown { get; set; } = Fix64.FromFloat(0.8f); // seconds
    public Fix64 radius { get; set; } = Fix64.FromFloat(0.5f);
    public int countPerSide { get; set; } = 0; // integral count kept as int
}