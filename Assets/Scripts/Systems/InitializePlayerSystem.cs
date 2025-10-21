using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct InitializePlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (color, entity) in SystemAPI.Query<RefRW<PlayerColorComponent>>().WithAny<NewPlayerTag>().WithEntityAccess())
            {
                ecb.SetComponent(entity, new URPMaterialPropertyBaseColor
                {
                    Value = color.ValueRW.Color
                });
                
                ecb.RemoveComponent<PlayerColorComponent>(entity);
                ecb.RemoveComponent<NewPlayerTag>(entity);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}