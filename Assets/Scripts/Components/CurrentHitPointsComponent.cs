using Unity.Entities;
using Unity.NetCode;

namespace Components
{
    public struct CurrentHitPointsComponent : IComponentData
    {
        [GhostField] public int Value;
    }
}