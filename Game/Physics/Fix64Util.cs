namespace PaeezanAssignment_Server.Common.Game.Physics
{
    /// <summary>
    /// Helpers to convert your fixed-point values to floats for rendering.
    /// </summary>
    public static class Fix64Util
    {
        // Adjust if your Fix64 scaling factor differs.
        private const long ONE = 1L << 32; // based on your 32-bit fractional design


        public static float ToFloat(long raw)
        {
            return (float)((double)raw / ONE);
        }
        

        public static float ToFloat(PaeezanAssignment_Server.Common.Game.Physics.Fix64 v)
        {
            return ToFloat(v.RawValue); // ensure Fix64 exposes RawValue long
        }
    }
}