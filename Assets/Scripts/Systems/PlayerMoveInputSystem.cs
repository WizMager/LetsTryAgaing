using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class PlayerMoveInputSystem : SystemBase
    {
        private InputSystem_Actions _inputSystemActions;
        private float2 _moveDirection = float2.zero;
        private bool _isChangeDirection;
        private bool _isMove;
        
        protected override void OnCreate()
        {
            _inputSystemActions = new InputSystem_Actions();
            _inputSystemActions.Enable();
            
            _inputSystemActions.Player.Move.performed += OnMovePerformed;
            _inputSystemActions.Player.Move.canceled += OnMoveEnd;
        }

        private void OnMoveEnd(InputAction.CallbackContext obj)
        {
            _isMove = false;
        }

        private void OnMovePerformed(InputAction.CallbackContext obj)
        {
            var newInput = obj.ReadValue<Vector2>();
            _isMove = true;
            
            if (!Mathf.Approximately(_moveDirection.x, newInput.x) || !Mathf.Approximately(_moveDirection.y, newInput.y))
            {
                _isChangeDirection = true;
                _moveDirection = newInput;
            }
        }

        protected override void OnUpdate()
        {
            if (!_isMove)
                return;

            if (_isChangeDirection)
            {
                _isChangeDirection = false;
                
                var entity = SystemAPI.GetSingletonEntity<PlayerOwnerTag>();
                EntityManager.SetComponentData(entity, new InputDataComponent
                {
                    MoveDirection = _moveDirection
                });
            }
        }

        protected override void OnDestroy()
        {
            _inputSystemActions.Disable();
            _inputSystemActions.Dispose();
        }
    }
}