using Components;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace Authoring
{
    public class AbilityPrefabsAuthoring : MonoBehaviour
    {
        public GameObject SuperAbilityPrefab;
        public float SuperAbilityCooldown;
        public NetCodeConfig NetCodeConfig;

        private int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        private class AbilityPrefabsAuthoringBaker : Baker<AbilityPrefabsAuthoring>
        {
            public override void Bake(AbilityPrefabsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new AbilityPrefabs
                {
                    SuperAbilityPrefab = GetEntity(authoring.SuperAbilityPrefab, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity, new AbilityCooldownTicksComponent
                {
                    SuperAbility = (uint)(authoring.SuperAbilityCooldown * authoring.SimulationTickRate)
                });
                AddBuffer<AbilityCooldownTargetTicks>(entity);
            }
        }
    }
}