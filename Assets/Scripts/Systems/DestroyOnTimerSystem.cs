using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct DestroyOnTimerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (destroyAtTick, entity) in SystemAPI
                .Query<RefRW<DestroyAtTickComponent>>()
                .WithAll<Simulate>()
                .WithNone<DestroyEntityTag>()
                .WithEntityAccess()
            )
            {
                if (currentTick.Equals(destroyAtTick.ValueRO.Value) || currentTick.IsNewerThan(destroyAtTick.ValueRO.Value))
                {
                    ecb.AddComponent<DestroyEntityTag>(entity);
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}