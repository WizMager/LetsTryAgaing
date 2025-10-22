using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct LocalPlayerInitializationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (localTransform, entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<GhostOwnerIsLocal>().WithNone<PlayerOwnerTag>().WithEntityAccess())
            {
                ecb.AddComponent<PlayerOwnerTag>(entity);
                ecb.SetComponent(entity, new InputDataComponent
                {
                    MoveDirection = float2.zero
                });
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}