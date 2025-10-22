using Components;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Authoring
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public float PlayerSpeed;
        
        private class PlayerAuthoringBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlayerTag>(entity);
                AddComponent<NewPlayerTag>(entity);
                AddComponent<PlayerColorComponent>(entity);
                AddComponent<URPMaterialPropertyBaseColor>(entity);
                AddComponent(entity, new MoveSpeedComponent
                {
                    Value = authoring.PlayerSpeed
                });
                AddComponent<InputDataComponent>(entity);
            }
        }
    }
}