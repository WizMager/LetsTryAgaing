using Components;
using R3;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientConnectionManager : MonoBehaviour
{
        [SerializeField] private TMP_InputField _addressField;
        [SerializeField] private TMP_InputField _portField;
        [SerializeField] private TMP_Dropdown _connectionModeDropdown;
        [SerializeField] private Button _connectButton;
        [SerializeField] private TMP_Text _buttonText;

        private CompositeDisposable _disposables;
        
        private string Address => _addressField.text;
        private ushort Port => ushort.Parse(_portField.text);

        private void OnEnable()
        {
                if (_buttonText == null)
                {
                        _buttonText = _connectButton.GetComponentInChildren<TMP_Text>();
                }

                //default value for tests
                _addressField.text = "127.0.0.1";
                _portField.text = "7979";
                
                _disposables = new CompositeDisposable();
                _connectionModeDropdown.OnValueChangedAsObservable().Subscribe(OnConnectionModeChange).AddTo(_disposables);
                _connectButton.OnClickAsObservable().Subscribe(_ => OnConnectionClicked()).AddTo(_disposables);
                OnConnectionModeChange(_connectionModeDropdown.value);
        }

        private void OnConnectionModeChange(int connectionMode)
        {
                string buttonLabel;
                _connectButton.enabled = true;
                
                switch (connectionMode)
                {
                      case  0:
                              buttonLabel = "Start Host";
                          break;
                      case 1:
                              buttonLabel = "Start Server";
                              break;
                      case 2: buttonLabel = "Start Client";
                              break;
                      default:
                              buttonLabel = "ERROR";
                              _connectButton.enabled = false;
                              break;
                }

                _buttonText.text = buttonLabel;
        }

        private void OnConnectionClicked()
        {
                DestroyLocalSimulationWorld();
                SceneManager.LoadScene(1);

                switch (_connectionModeDropdown.value)
                {
                        case 0:
                                StartServer();
                                StartClient();
                                break;
                        case 1:
                                StartServer();
                                break;
                        case 2:
                                StartClient();
                                break;
                        default:
                                Debug.Log(
                                        $"[{nameof(ClientConnectionManager)}]: Unknown connection mode: {_connectionModeDropdown.value}");
                                break;
                }
        }

        private static void DestroyLocalSimulationWorld()
        {
                foreach (var world in World.All)
                {
                        if (world.Flags != WorldFlags.Game)
                                continue;

                        world.Dispose();
                        break;
                }
        }

        private void StartServer()
        {
                var serverWorld = ClientServerBootstrap.CreateServerWorld("SomeServerWorld");
                var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(Port);

                using var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
        }

        private void StartClient()
        {
                var clientWorld = ClientServerBootstrap.CreateClientWorld("SomeClientWorld");
                var connectionEndpoint = NetworkEndpoint.Parse(Address, Port);
                
                using var networkDriverQuery = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);

                World.DefaultGameObjectInjectionWorld = clientWorld;
                
                var requestEntity = clientWorld.EntityManager.CreateEntity();
                clientWorld.EntityManager.AddComponentData(requestEntity, new ClientSpawn());
        }
        
        private void OnDisable()
        {
              _disposables?.Dispose();  
        }
}