using PaeezanAssignment_Server.Common.Game.Physics;

namespace PaeezanAssignment_Server.Common.Game.Entity
{
    public sealed class MeleeUnitEntity : UnitEntity
    {
        public MeleeUnitEntity(int owner, Fix64 hp, Fix64 speed, Fix64 dmg, Fix64 range, Fix64 cooldown)
            : base(owner, hp, speed, dmg, range, cooldown)
        {
            Type = EntityType.Melee;
        }
    }
}