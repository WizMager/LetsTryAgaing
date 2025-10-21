using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class PrefabsAuthoring : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        
        private class PrefabAuthoringBaker : Baker<PrefabsAuthoring>
        {
            public override void Bake(PrefabsAuthoring authoring)
            {
                var prefabContainerEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(prefabContainerEntity, new PrefabsComponent
                {
                    Player = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}