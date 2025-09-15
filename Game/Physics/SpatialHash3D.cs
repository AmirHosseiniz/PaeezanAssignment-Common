// Physics/SpatialHash3D.cs  (simple uniform grid broadphase)

namespace PaeezanAssignment_Server.Common.Game.Physics
{
    internal sealed class SpatialHash3D
    {
        private readonly Fix64 _cellSize;
        private readonly Dictionary<(int, int, int), List<int>> _cells = new Dictionary<(int, int, int), List<int>>();

        public SpatialHash3D(Fix64 cellSize)
        {
            _cellSize = cellSize;
        }

        public void Clear() => _cells.Clear();

        private static int FloorToInt(Fix64 v, Fix64 invCell)
        {
            // deterministic floor: (int)Math.Floor(v / cell)
            var raw = v.ToDouble() * invCell.ToDouble();
            return (int)Math.Floor(raw);
        }

        public void InsertAabb(int id, FixedVector3 min, FixedVector3 max)
        {
            var inv = Fix64.One / _cellSize;
            int x0 = FloorToInt(min.x, inv), x1 = FloorToInt(max.x, inv);
            int y0 = FloorToInt(min.y, inv), y1 = FloorToInt(max.y, inv);
            int z0 = FloorToInt(min.z, inv), z1 = FloorToInt(max.z, inv);

            for (int x = x0; x <= x1; x++)
            for (int y = y0; y <= y1; y++)
            for (int z = z0; z <= z1; z++)
            {
                var key = (x, y, z);
                if (!_cells.TryGetValue(key, out var list))
                {
                    list = new List<int>(4);
                    _cells[key] = list;
                }

                list.Add(id);
            }
        }

        public void QueryPairs(List<(int, int)> outPairs)
        {
            outPairs.Clear();
            foreach (var kv in _cells)
            {
                var list = kv.Value;
                // stable pair order
                for (int i = 0; i < list.Count; i++)
                for (int j = i + 1; j < list.Count; j++)
                    outPairs.Add((list[i], list[j]));
            }
        }
    }
}