using Unity.Entities;
using Unity.NetCode;

namespace Components
{
    public struct DestroyAtTickComponent : IComponentData
    {
        [GhostField] public NetworkTick Value;
    }
}