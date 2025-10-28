using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class AbilityPrefabsAuthoring : MonoBehaviour
    {
        public GameObject SuperAbilityPrefab;
        
        private class AbilityPrefabsAuthoringBaker : Baker<AbilityPrefabsAuthoring>
        {
            public override void Bake(AbilityPrefabsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new AbilityPrefabs
                {
                    SuperAbilityPrefab = GetEntity(authoring.SuperAbilityPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}