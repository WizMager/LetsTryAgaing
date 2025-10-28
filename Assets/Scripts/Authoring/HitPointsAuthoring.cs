using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class HitPointsAuthoring : MonoBehaviour
    {
        public int MaxHitPoints;
        
        private class HitPointsAuthoringBaker : Baker<HitPointsAuthoring>
        {
            public override void Bake(HitPointsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MaxHitPointsComponent
                {
                    Value = authoring.MaxHitPoints
                });
                AddComponent(entity, new CurrentHitPointsComponent
                {
                    Value = authoring.MaxHitPoints
                });
                AddBuffer<DamageBufferElement>(entity);
                AddBuffer<DamageThisTick>(entity);
            }
        }
    }
}