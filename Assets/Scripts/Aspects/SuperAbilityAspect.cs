using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct SuperAbilityAspect : IAspect
    {
        private readonly RefRO<InputAbilityComponent> _abilityComponent;
        private readonly RefRO<AbilityPrefabs> _abilityPrefabs;
        private readonly RefRO<LocalTransform> _localTransform;

        public bool ShouldAttack => _abilityComponent.ValueRO.SuperAbility.IsSet;
        public Entity AbilityPrefab => _abilityPrefabs.ValueRO.SuperAbilityPrefab;
        public float3 AttackPosition => _localTransform.ValueRO.Position;
    }
}