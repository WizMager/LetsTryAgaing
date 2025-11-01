using Unity.Entities;

namespace Components
{
    public struct AbilityCooldownTicksComponent : IComponentData
    {
        public uint SuperAbility;
        public uint ShootAbility;
    }
}