using Components;
using Unity.Entities;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct SuperAbilityAspect : IAspect
    {
        private readonly RefRO<InputAbilityComponent> _abilityComponent;
        private readonly RefRO<AbilityPrefabs> _abilityPrefabs;
        private readonly RefRO<LocalTransform> _localTransform;
    }
}