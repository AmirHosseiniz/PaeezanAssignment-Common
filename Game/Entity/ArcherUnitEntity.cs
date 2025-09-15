namespace PaeezanAssignment_Server.Common.Game.Entity;

public sealed class ArcherUnitEntity : UnitEntity
{
    private readonly Fix64 _projectileSpeed;


    public ArcherUnitEntity(int owner, Fix64 hp, Fix64 speed, Fix64 dmg, Fix64 range, Fix64 cooldown,
        Fix64 projectileSpeed)
        : base(owner, hp, speed, dmg, range, cooldown)
    {
        _projectileSpeed = projectileSpeed;
        Type = EntityType.Archer;
    }


    protected override void TryAttack(BattleSim sim, GameEntity target)
    {
        if (CooldownTimer > Fix64.Zero) return;
        var origin = Body.WorldPosition;
        var aim = target.Body.WorldPosition - origin;
        var dist = aim.Magnitude;
        var dir = dist > Fix64.Zero ? aim / dist : FixedVector3.Right;
        sim.SpawnProjectile(this, target, origin, dir, _projectileSpeed, AttackDamage);
        CooldownTimer = AttackCooldown;
    }
}