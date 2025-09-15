namespace PaeezanAssignment_Server.Common.Game.Physics
{
    public enum CollisionEventType
    {
        CollisionEnter,
        CollisionStay,
        CollisionExit,
        TriggerEnter,
        TriggerStay,
        TriggerExit
    }

    public struct CollisionEvent
    {
        public CollisionEventType Type;
        public CollisionInfo Info; // includes A/B, normal, penetration, contacts
        public bool IsTriggerEvent; // convenience
        public uint Frame; // simulation frame index (deterministic ordering)
    }
}