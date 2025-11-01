using Unity.Entities;

namespace Components
{
    public struct AbilityPrefabs : IComponentData
    {
        public Entity SuperAbilityPrefab;
        public Entity ShootAbility;
    }
}