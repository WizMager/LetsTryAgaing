using Unity.Entities;
using Unity.NetCode;

namespace Components
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct InputAbilityComponent : IInputComponentData
    {
        [GhostField] public InputEvent SuperAbility;
        [GhostField] public InputEvent ShootAbility;
    }
}