using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    public partial struct CalculateFrameDamageSystem : ISystem
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

            foreach (var (damageBuffer, damageThisTicks) in SystemAPI
                .Query<DynamicBuffer<DamageBufferElement>, DynamicBuffer<DamageThisTick>>()
                .WithAll<Simulate>())
            {
                if (damageBuffer.IsEmpty)
                {
                    damageThisTicks.AddCommandData(new DamageThisTick
                    {
                        Tick = currentTick,
                        Value = 0
                    });
                }
                else
                {
                    var totalDamage = 0;
                    if (damageThisTicks.GetDataAtTick(currentTick, out var damageThisTick))
                    {
                        totalDamage = damageThisTick.Value;
                    }

                    foreach (var damageBufferElement in damageBuffer)
                    {
                        totalDamage += damageBufferElement.Value;
                    }
                    
                    damageThisTicks.AddCommandData(new DamageThisTick
                    {
                        Tick = currentTick,
                        Value = totalDamage
                    });
                    
                    damageBuffer.Clear();
                }
            }
        }
    }
}