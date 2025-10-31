using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct DamageOnTriggerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            
            var damageOnTriggerJob = new DamageOnTriggerJob
            {
                DamageOnTriggerLookup = SystemAPI.GetComponentLookup<DamageOnTriggerComponent>(true),
                Enemies = SystemAPI.GetComponentLookup<EnemyTag>(true),
                AlreadyDamagedEntitiesBufferLookup = SystemAPI.GetBufferLookup<AlreadyDamagedEntitiesBuffer>(true),
                DamageBufferLookup = SystemAPI.GetBufferLookup<DamageBufferElement>(true),
                Ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            };

            var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = damageOnTriggerJob.Schedule(simulationSingleton, state.Dependency);
        }
    }
    
    public struct DamageOnTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<DamageOnTriggerComponent> DamageOnTriggerLookup;
        [ReadOnly] public ComponentLookup<EnemyTag> Enemies;
        [ReadOnly] public BufferLookup<AlreadyDamagedEntitiesBuffer> AlreadyDamagedEntitiesBufferLookup;
        [ReadOnly] public BufferLookup<DamageBufferElement> DamageBufferLookup;
        
        public EntityCommandBuffer Ecb;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity damageDealingEntity;
            Entity damageReceivingEntity;

            if (DamageBufferLookup.HasBuffer(triggerEvent.EntityA) && DamageOnTriggerLookup.HasComponent(triggerEvent.EntityB))
            {
                damageReceivingEntity = triggerEvent.EntityA;
                damageDealingEntity = triggerEvent.EntityB;
            }else if (DamageBufferLookup.HasBuffer(triggerEvent.EntityB) && DamageOnTriggerLookup.HasComponent(triggerEvent.EntityA))
            {
                damageDealingEntity = triggerEvent.EntityA;
                damageReceivingEntity = triggerEvent.EntityB;
            }
            else
            {
                return;
            }
            
            var alreadyDamageBuffer = AlreadyDamagedEntitiesBufferLookup[damageDealingEntity];
            foreach (var alreadyDamagedEntitiesBuffer in alreadyDamageBuffer)
            {
                if(alreadyDamagedEntitiesBuffer.Value.Equals(damageReceivingEntity))
                    return;
            }

            if (!Enemies.HasComponent(damageReceivingEntity))
                return;
            
            var damageOnTrigger = DamageOnTriggerLookup[damageDealingEntity];
            Ecb.AppendToBuffer(damageReceivingEntity, new DamageBufferElement
            {
                Value = damageOnTrigger.Value
            });
            Ecb.AppendToBuffer(damageDealingEntity, new AlreadyDamagedEntitiesBuffer
            {
                Value = damageReceivingEntity
            });
        }
    }
}