using PaeezanAssignment_Server.Common.Game.DTO;
using PaeezanAssignment_Server.Common.Game.Entity;
using PaeezanAssignment_Server.Common.Game.Simulation;

public sealed class BattleSim
{
    public readonly PhysicsWorld World;
    public readonly SimConfig Config;

    private readonly Dictionary<int, GameEntity> _entities = new Dictionary<int, GameEntity>(256); // key: bodyId
    private readonly List<GameEntity> _updateList = new List<GameEntity>(256);
    private readonly List<GameEntity> _despawnBuffer = new List<GameEntity>(64);

    private TowerEntity _leftTower;
    private TowerEntity _rightTower;

    public BattleSim(SimConfig config)
    {
        Config = config;
        World = new PhysicsWorld();
        World.OnCollisionEvent += HandleCollisionEvent;

        SolveCollideMatrix();
        InitMapAndTowers();
        InitialSpawnsFromConfig();
    }

    public GameEntity TryGetEntityByBody(int bodyId)
        => _entities.TryGetValue(bodyId, out var e) ? e : null;

    public void MarkForDespawn(GameEntity e)
    {
        if (!_despawnBuffer.Contains(e)) _despawnBuffer.Add(e);
    }

    private void HandleCollisionEvent(CollisionEvent evt)
    {
        if (!evt.IsTriggerEvent) return;
        var a = evt.Info.BodyA;
        var b = evt.Info.BodyB;
        if (a == null || b == null) return;
        var aIsProj = a.Layer == CollisionLayer.Projectile || a.Tag == "Projectile";
        var bIsProj = b.Layer == CollisionLayer.Projectile || b.Tag == "Projectile";
        if (!aIsProj && !bIsProj) return;
        if (aIsProj && _entities.TryGetValue(a.Id, out var aEnt) && aEnt is ProjectileEntity pa)
            pa.OnTriggerHit(this, b);
        else if (bIsProj && _entities.TryGetValue(b.Id, out var bEnt) && bEnt is ProjectileEntity pb)
            pb.OnTriggerHit(this, a);
    }

    private void SolveCollideMatrix()
    {
        World.CollisionMatrix
            .Allow(CollisionLayer.Unit, CollisionLayer.Unit)
            .Allow(CollisionLayer.Unit, CollisionLayer.Tower)
            .Allow(CollisionLayer.Projectile, CollisionLayer.Unit)
            .Allow(CollisionLayer.Projectile, CollisionLayer.Tower);
    }

    private void InitMapAndTowers()
    {
        var width = Config.map.width;
        var laneY = Config.map.laneY;
        var half = width * Fix64.Half;
        var gap = Config.towers.halfGapToEdge;
        var towerSize = Config.towers.size.ToFixed();

        // Left Tower
        _leftTower = new TowerEntity(owner: 0, maxHp: Config.towers.hp)
        {
            Body = new PhysicsBody
            {
                Type = BodyType.Static,
                Tag = "Tower",
                Layer = CollisionLayer.Tower,
                Position = new FixedVector3(-half + gap, laneY, Fix64.Zero),
                ColliderType = ColliderType.Box,
                ColliderSize = towerSize,
                UseGravity = false,
                IsTrigger = false
            }
        };
        _leftTower.BindId(World.AddBody(_leftTower.Body));
        CollisionDetection.BuildAabb(_leftTower.Body); // ensure static AABB built once
        _entities[_leftTower.Body.Id] = _leftTower;
        _leftTower.OnAdded(this);

        // Right Tower
        _rightTower = new TowerEntity(owner: 1, maxHp: Config.towers.hp)
        {
            Body = new PhysicsBody
            {
                Type = BodyType.Static,
                Tag = "Tower",
                Layer = CollisionLayer.Tower,
                Position = new FixedVector3(half - gap, laneY, Fix64.Zero),
                ColliderType = ColliderType.Box,
                ColliderSize = towerSize,
                UseGravity = false,
                IsTrigger = false
            }
        };
        _rightTower.BindId(World.AddBody(_rightTower.Body));
        CollisionDetection.BuildAabb(_rightTower.Body);
        _entities[_rightTower.Body.Id] = _rightTower;
        _rightTower.OnAdded(this);
    }

    private void InitialSpawnsFromConfig()
    {
        for (int i = 0; i < Config.melee.countPerSide; i++)
        {
            SpawnUnit(isArcher: false, owner: 0, laneOffsetZ: Fix64.Zero,
                extraOffsetX: Fix64.FromFloat(1.5f) * Fix64.FromInt(i));
            SpawnUnit(isArcher: false, owner: 1, laneOffsetZ: Fix64.Zero,
                extraOffsetX: Fix64.FromFloat(1.5f) * Fix64.FromInt(i));
        }

        for (int i = 0; i < Config.archer.countPerSide; i++)
        {
            SpawnUnit(isArcher: true, owner: 0, laneOffsetZ: Fix64.Zero,
                extraOffsetX: Fix64.FromFloat(1.5f) * Fix64.FromInt(i));
            SpawnUnit(isArcher: true, owner: 1, laneOffsetZ: Fix64.Zero,
                extraOffsetX: Fix64.FromFloat(1.5f) * Fix64.FromInt(i));
        }
    }

    public UnitEntity SpawnUnit(bool isArcher, int owner, Fix64 laneOffsetZ, Fix64 extraOffsetX)
    {
        var laneY = Config.map.laneY;
        var width = Config.map.width;
        var gap = Config.towers.halfGapToEdge;
        var startX = owner == 0
            ? (-width * Fix64.Half + gap + extraOffsetX)
            : (width * Fix64.Half - gap - extraOffsetX);

        UnitEntity unit = isArcher
            ? new ArcherUnitEntity(
                owner,
                hp: Config.archer.hp,
                speed: Config.archer.speed,
                dmg: Config.archer.damage,
                range: Config.archer.range,
                cooldown: Config.archer.cooldown,
                projectileSpeed: Config.archer.projectileSpeed)
            : new MeleeUnitEntity(
                owner,
                hp: Config.melee.hp,
                speed: Config.melee.speed,
                dmg: Config.melee.damage,
                range: Config.melee.range,
                cooldown: Config.melee.cooldown);

        var radius = isArcher ? Config.archer.radius : Config.melee.radius;
        unit.Body = new PhysicsBody
        {
            Type = BodyType.Dynamic,
            Tag = isArcher ? "Archer" : "Melee",
            Layer = CollisionLayer.Unit,
            Position = new FixedVector3(startX, laneY, laneOffsetZ),
            ColliderType = ColliderType.Sphere,
            ColliderSize = new FixedVector3(radius, Fix64.Zero, Fix64.Zero),
            Mass = Fix64.FromInt(1),
            UseGravity = false,
            IsTrigger = false
        };
        unit.BindId(World.AddBody(unit.Body));
        CollisionDetection.BuildAabb(unit.Body);
        unit.MoveDir = owner == 0 ? FixedVector3.Right : FixedVector3.Left;

        _entities[unit.Body.Id] = unit;
        unit.OnAdded(this);
        return unit;
    }

    public void SpawnProjectile(UnitEntity ownerUnit, GameEntity target, FixedVector3 origin, FixedVector3 dir,
        Fix64 speed, Fix64 damage)
    {
        var proj = new ProjectileEntity(ownerUnit.Owner, damage, lifeSeconds: Fix64.FromInt(5),
            targetOwner: 1 - ownerUnit.Owner)
        {
            Body = new PhysicsBody
            {
                Type = BodyType.Dynamic,
                Tag = "Projectile",
                Layer = CollisionLayer.Projectile,
                Position = origin,
                ColliderType = ColliderType.Sphere,
                ColliderSize = new FixedVector3(Fix64.FromFloat(0.2f), Fix64.Zero, Fix64.Zero),
                Mass = Fix64.FromFloat(0.1f),
                UseGravity = false,
                IsTrigger = true,
                Velocity = dir * speed
            }
        };
        proj.BindId(World.AddBody(proj.Body));
        CollisionDetection.BuildAabb(proj.Body);
        _entities[proj.Body.Id] = proj;
        proj.OnAdded(this);
    }

    public void Tick()
    {
        // Deterministic update order: sorted by entity Id (body id)
        _updateList.Clear();
        foreach (var e in _entities.Values) _updateList.Add(e);
        _updateList.Sort(static (a, b) => a.Id.CompareTo(b.Id));

        foreach (var e in _updateList) e.Tick(this);

        World.Step();

        if (_despawnBuffer.Count > 0)
        {
            _despawnBuffer.Sort(static (a, b) => a.Id.CompareTo(b.Id));
            foreach (var e in _despawnBuffer)
            {
                if (e.Body != null)
                {
                    World.RemoveBody(e.Body.Id);
                    _entities.Remove(e.Body.Id);
                    e.OnRemoved(this);
                    e.Body = null;
                }
            }

            _despawnBuffer.Clear();
        }
    }

    public Snapshot BuildSnapshot()
    {
        var list = new List<SnapshotEntity>(_entities.Count);
        foreach (var e in _entities.Values)
        {
            if (!e.IsAlive) continue;
            var b = e.Body;
            list.Add(new SnapshotEntity
            {
                Id = e.Id,
                Type = e.Type,
                Owner = e.Owner,
                Px = b.WorldPosition.x.ToRaw(),
                Py = b.WorldPosition.y.ToRaw(),
                Pz = b.WorldPosition.z.ToRaw(),
                Vx = b.Velocity.x.ToRaw(),
                Vy = b.Velocity.y.ToRaw(),
                Vz = b.Velocity.z.ToRaw(),
                HP = e.HP.ToRaw(),
            });
        }

        list.Sort(static (a, b) => a.Id.CompareTo(b.Id));

        int winner = -1;
        if (_leftTower.HP == Fix64.Zero || _rightTower.HP == Fix64.Zero)
            winner = _leftTower.HP == Fix64.Zero ? 1 : 0;

        return new Snapshot { Frame = World.Frame, Winner = winner, Entities = list.ToArray() };
    }

    public GameEntity FindNearestEnemy(GameEntity self, Fix64 withinRange)
    {
        GameEntity best = null;
        var bestDist2 = Fix64.FromInt(int.MaxValue);
        var pos = self.Body.WorldPosition;
        var range2 = withinRange * withinRange;

        foreach (var e in _entities.Values)
        {
            if (e.Owner == self.Owner) continue;
            if (e.Type == EntityType.Projectile) continue;

            var d2 = FixedVector3.DistanceSqr(pos, e.Body.WorldPosition);
            if (d2 <= range2)
            {
                if (best == null || d2 < FixedVector3.DistanceSqr(pos, best.Body.WorldPosition) ||
                    (d2 == FixedVector3.DistanceSqr(pos, best.Body.WorldPosition) && e.Id < best.Id))
                {
                    best = e;
                }
            }
        }

        return best;
    }
}