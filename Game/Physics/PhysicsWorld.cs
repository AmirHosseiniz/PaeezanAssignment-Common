using System;
using System.Collections.Generic;

namespace PaeezanAssignment_Server.Common.Game.Physics
{
    public sealed class PhysicsWorld
    {
        // —— Configuration ——————————————————————————
        public FixedVector3 Gravity { get; set; } = new FixedVector3(Fix64.Zero, Fix64.FromFloat(-9.81f), Fix64.Zero);
        public Fix64 FixedTimeStep { get; set; } = Fix64.FromFloat(1f / 60f);
        public CollisionMatrix CollisionMatrix { get; } = CollisionMatrix.CreateDefault();
        public Fix64 BroadphaseCellSize { get; set; } = Fix64.FromInt(4); // 🔧 tweak

        // —— Deterministic storage ——————————————————
        private readonly List<PhysicsBody> _bodies = new List<PhysicsBody>(128);
        private readonly Dictionary<int, int> _idToIndex = new Dictionary<int, int>(128);
        private int _nextId = 1;
        private uint _frame;

        // —— Broadphase / narrowphase temp ——————————
        private readonly SpatialHash3D _hash;
        private readonly List<(int, int)> _candidatePairs = new List<(int, int)>(512);
        private readonly List<CollisionInfo> _contacts = new List<CollisionInfo>(256);

        // —— Event bookkeeping ————————————————
        private readonly HashSet<long> _prevPairs = new HashSet<long>();
        private readonly HashSet<long> _currPairs = new HashSet<long>();

        // —— Events ————————————————————————————————
        public event Action<CollisionEvent> OnCollisionEvent;

        public PhysicsWorld()
        {
            _hash = new SpatialHash3D(BroadphaseCellSize);
        }

        public uint Frame => _frame;

        public int AddBody(PhysicsBody body)
        {
            body.Id = _nextId++;
            _idToIndex[body.Id] = _bodies.Count;
            _bodies.Add(body);
            return body.Id;
        }

        public bool RemoveBody(int id)
        {
            if (!_idToIndex.TryGetValue(id, out var idx)) return false;
            var lastIdx = _bodies.Count - 1;
            var last = _bodies[lastIdx];

            _bodies[idx] = last;
            _idToIndex[last.Id] = idx;

            _bodies.RemoveAt(lastIdx);
            _idToIndex.Remove(id);
            return true;
        }

        public PhysicsBody GetBody(int id)
        {
            return _idToIndex.TryGetValue(id, out var idx) ? _bodies[idx] : null;
        }

        public IReadOnlyList<PhysicsBody> Bodies => _bodies;

        // —— Main tick —————————————————————————————
        public void Step()
        {
            _frame++;

            Integrate();
            BuildBroadphase();
            FindContacts();
            FireEnterStayExitEvents(); // includes trigger + collision flavors
            SolveCollisions(); // skip triggers here
        }

        // —— Integration (deterministic, no LINQ) —————
        private void Integrate()
        {
            var dt = FixedTimeStep;
            for (int i = 0; i < _bodies.Count; i++)
            {
                var b = _bodies[i];
                if (b.Type == BodyType.Static) continue;

                if (b.UseGravity && b.Type == BodyType.Dynamic)
                {
                    var accel = Gravity; // since F = m*g, a = g
                    b.Velocity += accel * dt;
                }

                // linear drag (semi-implicit Euler)
                if (b.Drag > Fix64.Zero)
                    b.Velocity -= b.Velocity * b.Drag * dt;

                b.Position += b.Velocity * dt;

                // angular (kept simple; your model already has AngularDrag)
                if (b.AngularDrag > Fix64.Zero)
                    b.AngularVelocity -= b.AngularVelocity * b.AngularDrag * dt;

                b.Rotation += b.AngularVelocity * dt;

                // update cached AABB
                CollisionDetection.BuildAabb(b);

                _bodies[i] = b; // struct-like discipline for determinism even with class
            }
        }

        // —— Broadphase ————————————————————————————
        private void BuildBroadphase()
        {
            _hash.Clear();
            for (int i = 0; i < _bodies.Count; i++)
            {
                var b = _bodies[i];
                _hash.InsertAabb(b.Id, b.AabbMin, b.AabbMax);
            }
        }

        // —— Narrowphase ————————————————————————————
        private void FindContacts()
        {
            _contacts.Clear();
            _hash.QueryPairs(_candidatePairs);

            for (int k = 0; k < _candidatePairs.Count; k++)
            {
                var (idA, idB) = _candidatePairs[k];
                var a = GetBody(idA);
                var b = GetBody(idB);
                if (a == null || b == null) continue;

                // layer filtering
                if (!CollisionMatrix.ShouldCollide(a.Layer, b.Layer)) continue;

                // static-static skip
                if (a.Type == BodyType.Static && b.Type == BodyType.Static) continue;

                // AABB early out
                if (!CollisionDetection.CheckAabbOverlap(a, b)) continue;

                if (CollisionDetection.CheckCollision(a, b, out var info) && info.IsValid)
                {
                    _contacts.Add(info);
                }
            }
        }

        // —— Event generation (enter/stay/exit) —————————
        private static long PairKey(int a, int b)
        {
            // stable order (min,max) into 64-bit key
            if (a > b)
            {
                var t = a;
                a = b;
                b = t;
            }

            return ((long)a << 32) | (uint)b;
        }

        private void FireEnterStayExitEvents()
        {
            _currPairs.Clear();

            // Colliding this frame: fire Enter/Stay
            for (int i = 0; i < _contacts.Count; i++)
            {
                var info = _contacts[i];
                var key = PairKey(info.BodyA.Id, info.BodyB.Id);
                _currPairs.Add(key);

                bool wasColliding = _prevPairs.Contains(key);
                var isTrigger = info.IsTriggerPair;

                var evt = new CollisionEvent
                {
                    Type = isTrigger
                        ? (wasColliding ? CollisionEventType.TriggerStay : CollisionEventType.TriggerEnter)
                        : (wasColliding ? CollisionEventType.CollisionStay : CollisionEventType.CollisionEnter),
                    Info = info,
                    IsTriggerEvent = isTrigger,
                    Frame = _frame
                };
                OnCollisionEvent?.Invoke(evt);
            }

            // No longer colliding: fire Exit
            foreach (var key in _prevPairs)
            {
                if (_currPairs.Contains(key)) continue;

                // we need lightweight info to say who exited; rebuild minimal info
                int aId = (int)(key >> 32);
                int bId = (int)(key & 0xFFFFFFFF);

                var a = GetBody(aId);
                var b = GetBody(bId);
                if (a == null || b == null) continue;

                var isTrigger = a.IsTrigger || b.IsTrigger;
                var evt = new CollisionEvent
                {
                    Type = isTrigger ? CollisionEventType.TriggerExit : CollisionEventType.CollisionExit,
                    Info = new CollisionInfo { BodyA = a, BodyB = b, IsValid = false, IsTriggerPair = isTrigger },
                    IsTriggerEvent = isTrigger,
                    Frame = _frame
                };
                OnCollisionEvent?.Invoke(evt);
            }

            // swap sets
            _prevPairs.Clear();
            foreach (var k in _currPairs) _prevPairs.Add(k);
        }

        // —— Solver (impulse; no triggers) ————————————
        private void SolveCollisions()
        {
            // NO COLLISIONS NOW 

            // var restitution = Fix64.FromFloat(0.3f); // 🔧 tweak
            // for (int i = 0; i < _contacts.Count; i++)
            // {
            //     var c = _contacts[i];
            //     if (c.IsTriggerPair) continue; // triggers don't resolve
            //
            //     var a = c.BodyA;
            //     var b = c.BodyB;
            //
            //     var totalMass = a.Mass + b.Mass;
            //     if (totalMass <= Fix64.Zero) continue;
            //
            //     // positional correction (split by mass)
            //     var moveA = c.Penetration * (b.Mass / totalMass);
            //     var moveB = c.Penetration * (a.Mass / totalMass);
            //
            //     if (a.Type != BodyType.Static) a.Position -= c.Normal * moveA;
            //     if (b.Type != BodyType.Static) b.Position += c.Normal * moveB;
            //
            //     // velocity impulse
            //     var relVel = b.Velocity - a.Velocity;
            //     var sepVel = FixedVector3.Dot(relVel, c.Normal);
            //     if (sepVel > Fix64.Zero)
            //     {
            //         _contacts[i] = c;
            //         continue;
            //     }
            //
            //     var impulse = -(Fix64.One + restitution) * sepVel / totalMass;
            //
            //     if (a.Type == BodyType.Dynamic) a.Velocity -= c.Normal * impulse * b.Mass;
            //     if (b.Type == BodyType.Dynamic) b.Velocity += c.Normal * impulse * a.Mass;
            //
            //     // write back
            //     _bodies[_idToIndex[a.Id]] = a;
            //     _bodies[_idToIndex[b.Id]] = b;
            // }
        }
    }
}