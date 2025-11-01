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
        public GameObject ShootAbilityPrefab;
        
        public float SuperAbilityCooldown;
        public float ShootAbilityCooldown;
        
        public NetCodeConfig NetCodeConfig;

        private int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        private class AbilityPrefabsAuthoringBaker : Baker<AbilityPrefabsAuthoring>
        {
            public override void Bake(AbilityPrefabsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new AbilityPrefabs
                {
                    SuperAbilityPrefab = GetEntity(authoring.SuperAbilityPrefab, TransformUsageFlags.Dynamic),
                    ShootAbility = GetEntity(authoring.ShootAbilityPrefab, TransformUsageFlags.Dynamic),

                });
                AddComponent(entity, new AbilityCooldownTicksComponent
                {
                    SuperAbility = (uint)(authoring.SuperAbilityCooldown * authoring.SimulationTickRate),
                    ShootAbility = (uint)(authoring.ShootAbilityCooldown * authoring.SimulationTickRate)
                });
                AddBuffer<AbilityCooldownTargetTicks>(entity);
            }
        }
    }
}