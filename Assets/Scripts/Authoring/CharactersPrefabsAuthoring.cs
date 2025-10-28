using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class CharactersPrefabsAuthoring : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        
        private class PrefabAuthoringBaker : Baker<CharactersPrefabsAuthoring>
        {
            public override void Bake(CharactersPrefabsAuthoring authoring)
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