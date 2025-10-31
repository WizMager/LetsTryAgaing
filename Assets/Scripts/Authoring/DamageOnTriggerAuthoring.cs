using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class DamageOnTriggerAuthoring : MonoBehaviour
    {
        public int DamageOnTrigger;
        
        private class DamageOnTriggerAuthoringBaker : Baker<DamageOnTriggerAuthoring>
        {
            public override void Bake(DamageOnTriggerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageOnTriggerComponent
                {
                    Value = authoring.DamageOnTrigger
                });
                AddBuffer<AlreadyDamagedEntitiesBuffer>(entity);
            }
        }
    }
}