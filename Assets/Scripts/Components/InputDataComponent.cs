using Unity.Mathematics;
using Unity.NetCode;

namespace Components
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct InputDataComponent : IInputComponentData
    {
        [GhostField(Quantization = 10)] public float2 MoveDirection;
    }
}