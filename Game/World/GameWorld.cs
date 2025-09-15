// Game/GameWorld.cs  (clean separation: owns systems, not physics)

using Newtonsoft.Json;
using PaeezanAssignment_Server.Common.Game;
using PaeezanAssignment_Server.Common.Game.Simulation;

public sealed class GameWorld
{
    public BattleSim Simulation { get; private set; }

    public GameWorld(string config)
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new Fix64JsonConverter());
        var cfg = JsonConvert.DeserializeObject<SimConfig>(config, settings);


        Simulation = new BattleSim(cfg);
    }

    public void Tick() => Simulation.Tick();
}