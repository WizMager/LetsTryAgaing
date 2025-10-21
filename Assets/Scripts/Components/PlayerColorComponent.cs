using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Components
{
    public struct PlayerColorComponent : IComponentData
    {
        [GhostField] public float4 Color;
    }
}