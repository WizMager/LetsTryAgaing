using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    public partial struct DestroyEntitySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<NetworkTime>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();

            if (!networkTime.IsFirstTimeFullyPredictingTick)
                return;

            var currentTick = networkTime.ServerTick;
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (localTransform, entity) in SystemAPI
                .Query<RefRW<LocalTransform>>()
                .WithAll<DestroyEntityTag, Simulate>()
                .WithEntityAccess())
            {
                if (state.World.IsServer())
                {
                    ecb.DestroyEntity(entity);
                }
                else
                {
                    localTransform.ValueRW.Position = new float3(1000f, 1000f, 1000f);
                }
            }
        }
    }
}