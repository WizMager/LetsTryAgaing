using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class DestroyOnTimerAuthoring : MonoBehaviour
    {
        public float DestroyOnTimer;
        
        private class DestroyOnTimerAuthoringBaker : Baker<DestroyOnTimerAuthoring>
        {
            public override void Bake(DestroyOnTimerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DestroyOnTimerComponent
                {
                    Value = authoring.DestroyOnTimer
                });
            }
        }
    }
}