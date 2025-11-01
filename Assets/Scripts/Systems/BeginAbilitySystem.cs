using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginAbilitySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();

            if (!networkTime.IsFirstTimeFullyPredictingTick)
                return;
            
            var currentTick = networkTime.ServerTick;

            foreach (var superAbilityAspect in SystemAPI.Query<SuperAbilityAspect>().WithAll<Simulate>())
            {
                var isOnCooldown = true;
                var currentTargetTick = new AbilityCooldownTargetTicks();

                for (uint i = 0; i < networkTime.SimulationStepBatchSize; i++)
                {
                    var testTick = currentTick;
                    testTick.Subtract(i);

                    if (!superAbilityAspect.CooldownTargetTicks.GetDataAtTick(testTick, out currentTargetTick))
                    {
                        currentTargetTick.SuperAbility = NetworkTick.Invalid;
                    }

                    if (currentTargetTick.SuperAbility == NetworkTick.Invalid || !currentTargetTick.SuperAbility.IsNewerThan(currentTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }

                if (isOnCooldown)
                    continue;
                
                if (!superAbilityAspect.ShouldAttack)
                    continue;

                var ability = ecb.Instantiate(superAbilityAspect.AbilityPrefab);
                var localTransform = LocalTransform.FromPosition(superAbilityAspect.AttackPosition);
                ecb.SetComponent(ability, localTransform);
                
                if(state.WorldUnmanaged.IsServer())
                    continue;

                var newCooldownTargetTick = currentTick;
                newCooldownTargetTick.Add(superAbilityAspect.CooldownTicks);
                currentTargetTick.SuperAbility = newCooldownTargetTick;

                var nextTick = currentTick;
                nextTick.Add(1u);
                currentTargetTick.Tick = nextTick;
                superAbilityAspect.CooldownTargetTicks.AddCommandData(currentTargetTick);
            }
        }
    }
}