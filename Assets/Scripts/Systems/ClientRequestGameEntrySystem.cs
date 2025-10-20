using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct ClientRequestGameEntrySystem : ISystem
    {
        private EntityQuery _pendingNetworkIdQuery;

        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
            _pendingNetworkIdQuery = state.GetEntityQuery(builder);
            state.RequireForUpdate(_pendingNetworkIdQuery);
            state.RequireForUpdate<ClientSpawn>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var pendingNetworkIds = _pendingNetworkIdQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in pendingNetworkIds)
            {
                ecb.AddComponent<NetworkStreamInGame>(entity);
                var requestSpawnEntity = ecb.CreateEntity();
                ecb.AddComponent<ClientSpawnRequest>(requestSpawnEntity);
                ecb.AddComponent(requestSpawnEntity, new SendRpcCommandRequest
                {
                    TargetConnection = entity
                });
                
                ecb.Playback(state.EntityManager);
            }
        }
    }
}