using Unity.Entities;

namespace Components
{
    public struct AlreadyDamagedEntitiesBuffer : IBufferElementData
    {
        public Entity Value;
    }
}