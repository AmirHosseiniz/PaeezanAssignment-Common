using PaeezanAssignment_Server.Common.Game.Physics;

namespace PaeezanAssignment_Server.Common.Game.Entity
{
    public abstract class UnitEntity : GameEntity
    {
        protected Fix64 Speed; // units per second
        protected Fix64 AttackRange; // world units
        protected Fix64 AttackDamage; // damage per hit
        protected Fix64 AttackCooldown; // seconds between hits
        protected Fix64 CooldownTimer; // countdown timer

        // Movement target — always towards enemy tower X
        public FixedVector3 MoveDir;

        protected UnitEntity(int owner, Fix64 hp, Fix64 speed, Fix64 dmg, Fix64 range, Fix64 cooldown)
        {
            Owner = owner;
            MaxHP = hp;
            HP = hp;
            Speed = speed;
            AttackDamage = dmg;
            AttackRange = range;
            AttackCooldown = cooldown;
            Type = EntityType.Melee; // overridden by subclass
        }

        public override void Tick(BattleSim sim)
        {
            var dt = sim.World.FixedTimeStep;
            if (CooldownTimer > Fix64.Zero) CooldownTimer -= dt;

            // Find best target (nearest enemy within range)
            var target = sim.FindNearestEnemy(this, AttackRange);
            if (target != null)
            {
                var dist = FixedVector3.Distance(Body.WorldPosition, target.Body.WorldPosition);
                if (dist <= AttackRange)
                {
                    TryAttack(sim, target);
                    Stop(sim);
                    return;
                }
            }

            // otherwise march forward in lane
            Move(sim, MoveDir * Speed);
        }

        protected virtual void TryAttack(BattleSim sim, GameEntity target)
        {
            if (CooldownTimer > Fix64.Zero) return;
            target.ApplyDamage(sim, AttackDamage);
            CooldownTimer = AttackCooldown;
        }

        protected void Move(BattleSim sim, FixedVector3 vel)
        {
            Body.Velocity = vel;
        }

        protected void Stop(BattleSim sim)
        {
            Body.Velocity = FixedVector3.Zero;
        }
    }
}