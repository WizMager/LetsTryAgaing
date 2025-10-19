using System;
using R3;
using TMPro;
using Unity.Entities;
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
                
        }

        private void StartClient()
        {
                
        }
        
        private void OnDisable()
        {
              _disposables?.Dispose();  
        }
}