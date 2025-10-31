using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(CalculateFrameDamageSystem))]
    public partial struct ApplyDamageSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (currentHitPoints, damageThisTicks, entity) in SystemAPI
                .Query<RefRW<CurrentHitPointsComponent>, DynamicBuffer<DamageThisTick>>()
                .WithAll<Simulate>()
                .WithEntityAccess()
            )
            {
                if (!damageThisTicks.GetDataAtTick(currentTick, out var damageThisTick))
                    continue;

                if (damageThisTick.Tick != currentTick)
                    continue;

                currentHitPoints.ValueRW.Value -= damageThisTick.Value;

                if (currentHitPoints.ValueRO.Value <= 0)
                {
                    ecb.AddComponent<DestroyEntityTag>(entity);
                }
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}