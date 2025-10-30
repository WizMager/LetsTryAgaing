using Aspects;
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
                if (!superAbilityAspect.ShouldAttack)
                    continue;

                var ability = ecb.Instantiate(superAbilityAspect.AbilityPrefab);
                var localTransform = LocalTransform.FromPosition(superAbilityAspect.AttackPosition);
                ecb.SetComponent(ability, localTransform);
            }
        }
    }
}