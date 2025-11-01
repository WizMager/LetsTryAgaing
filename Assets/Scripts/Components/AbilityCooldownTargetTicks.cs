using Unity.NetCode;

namespace Components
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct AbilityCooldownTargetTicks : ICommandData
    {
        public NetworkTick Tick { get; set; }
        public NetworkTick SuperAbility;
        public NetworkTick ShootAbility;
    }
}