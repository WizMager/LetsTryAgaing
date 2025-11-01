using Unity.Mathematics;
using Unity.NetCode;

namespace Components
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct AimInputComponent : IInputComponentData
    {
        [GhostField(Quantization = 0)] public float3 Value;
    }
}