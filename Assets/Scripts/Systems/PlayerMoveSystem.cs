using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PlayerMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (localTransform, inputData, moveSpeed) in SystemAPI
                .Query<RefRW<LocalTransform>, RefRW<InputDataComponent>, RefRO<MoveSpeedComponent>>()
                .WithAll<Simulate>())
            {

                if (inputData.ValueRO.MoveDirection.x == 0 && inputData.ValueRO.MoveDirection.y == 0)
                    continue;
                
                var inputVector = inputData.ValueRO.MoveDirection * moveSpeed.ValueRO.Value * deltaTime;
                var moveVector = new float3(inputVector.x, 0, inputVector.y);
                
                localTransform.ValueRW.Position += moveVector;
                localTransform.ValueRW.Rotation = quaternion.LookRotationSafe(moveVector, math.up());
                inputData.ValueRW.MoveDirection = float2.zero;
            }
        }
    }
}