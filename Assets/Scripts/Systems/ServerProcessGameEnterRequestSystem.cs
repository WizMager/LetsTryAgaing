using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEnterRequestSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<ClientSpawnRequest, ReceiveRpcCommandRequest>();
            state.RequireAnyForUpdate(state.GetEntityQuery(builder));
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (requestSource, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<ClientSpawnRequest>().WithEntityAccess())
            {
                
                
                ecb.DestroyEntity(entity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.ValueRO.SourceConnection);

                var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.ValueRO.SourceConnection).Value;
                
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}