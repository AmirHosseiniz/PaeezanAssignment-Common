namespace PaeezanAssignment_Server.Common.Game.Entity;

public abstract class GameEntity
{
    public int Id { get; private set; }
    public EntityType Type { get; protected set; }
    public int Owner { get; protected set; } // 0 (left) or 1 (right)
    public PhysicsBody Body { get; set; }
    public Fix64 HP { get; protected set; }
    public Fix64 MaxHP { get; protected set; }
    public bool IsAlive => HP > Fix64.Zero && Body != null;


    public void BindId(int id) => Id = id;

    public virtual void OnAdded(BattleSim sim)
    {
    }

    public virtual void OnRemoved(BattleSim sim)
    {
    }

    public virtual void Tick(BattleSim sim)
    {
    }

    public virtual void ApplyDamage(BattleSim sim, Fix64 dmg)
    {
        if (HP <= Fix64.Zero) return;
        HP -= dmg;
        if (HP < Fix64.Zero) HP = Fix64.Zero;
        if (HP == Fix64.Zero)
            sim.MarkForDespawn(this);
    }
}