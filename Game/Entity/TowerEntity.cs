using PaeezanAssignment_Server.Common.Game.Physics;

namespace PaeezanAssignment_Server.Common.Game.Entity
{
    public sealed class TowerEntity : GameEntity
    {
        public TowerEntity(int owner, Fix64 maxHp)
        {
            Owner = owner;
            Type = EntityType.Tower;
            MaxHP = maxHp;
            HP = maxHp;
        }
    }
}