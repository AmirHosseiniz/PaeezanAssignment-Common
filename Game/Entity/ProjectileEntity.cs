using PaeezanAssignment_Server.Common.Game.Physics;
using PaeezanAssignment_Server.Common.Game.Simulation;

namespace PaeezanAssignment_Server.Common.Game.Entity
{
    public sealed class ProjectileEntity : GameEntity
    {
        public Fix64 Damage { get; private set; }
        private Fix64 _life;
        private readonly int _targetOwner;


        public ProjectileEntity(int owner, Fix64 damage, Fix64 lifeSeconds, int targetOwner)
        {
            Owner = owner;
            Type = EntityType.Projectile;
            Damage = damage;
            _life = lifeSeconds;
            _targetOwner = targetOwner;
            MaxHP = Fix64.One;
            HP = Fix64.One;
        }


        public override void Tick(BattleSim sim)
        {
            var dt = sim.World.FixedTimeStep;
            _life -= dt;
            if (_life <= Fix64.Zero) sim.MarkForDespawn(this);
        }


        public void OnTriggerHit(BattleSim sim, PhysicsBody otherBody)
        {
            var other = sim.TryGetEntityByBody(otherBody.Id);
            if (other == null) return;
            if (other.Owner == Owner) return;
            other.ApplyDamage(sim, Damage);
            sim.MarkForDespawn(this);
        }
    }
}