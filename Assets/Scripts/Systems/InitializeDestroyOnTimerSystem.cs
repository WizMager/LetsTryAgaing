using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Systems
{
    public partial struct InitializeDestroyOnTimerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var serverTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (destroyOnTimer, entity) in SystemAPI
                .Query<RefRO<DestroyOnTimerComponent>>()
                .WithNone<DestroyAtTickComponent>()
                .WithEntityAccess())
            {
                var lifetimeInTicks = (uint)(destroyOnTimer.ValueRO.Value * serverTickRate);
                var targetTick = currentTick;
                targetTick.Add(lifetimeInTicks);
                ecb.AddComponent(entity, new DestroyAtTickComponent
                {
                    Value = targetTick
                });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}