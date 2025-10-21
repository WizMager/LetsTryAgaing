using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEnterRequestSystem : ISystem
    {
        private int _playerIterator;
        
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<ClientSpawnRequest, ReceiveRpcCommandRequest>();
            state.RequireAnyForUpdate(state.GetEntityQuery(builder));
            state.RequireForUpdate<PrefabsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var playerPrefab = SystemAPI.GetSingleton<PrefabsComponent>().Player;
            
            foreach (var (requestSource, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<ClientSpawnRequest>().WithEntityAccess())
            {
                _playerIterator++;
                
                ecb.DestroyEntity(entity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.ValueRO.SourceConnection);

                var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.ValueRO.SourceConnection).Value;
                var newPlayer = ecb.Instantiate(playerPrefab);
                ecb.SetName(newPlayer, "Player " + clientId);
                
                var spawnPosition = new float3(0, 1, 0);
                var localTransform = LocalTransform.FromPosition(spawnPosition);
                ecb.SetComponent(newPlayer, localTransform);
                ecb.AddComponent(newPlayer, new GhostOwner
                {
                    NetworkId = clientId
                });
                ecb.SetComponent(newPlayer, new PlayerColorComponent
                {
                    Color = new float4(_playerIterator * 50 % 255f, _playerIterator * 50 % 255f, _playerIterator * 50 % 255f, 1)
                });
                ecb.AppendToBuffer(requestSource.ValueRO.SourceConnection, new LinkedEntityGroup
                {
                    Value = newPlayer
                });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}