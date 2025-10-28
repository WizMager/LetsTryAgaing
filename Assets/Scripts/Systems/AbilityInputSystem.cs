using Components;
using Unity.Entities;

namespace Systems
{
    public partial class AbilityInputSystem : SystemBase
    {
        private InputSystem_Actions _inputSystemActions;
        
        protected override void OnCreate()
        {
            _inputSystemActions = new InputSystem_Actions();
            
        }

        protected override void OnStartRunning()
        {
            _inputSystemActions.Enable();
        }

        protected override void OnStopRunning()
        {
            _inputSystemActions.Disable();
        }

        protected override void OnUpdate()
        {
            var abilityInputComponent = new InputAbilityComponent();

            if (_inputSystemActions.Player.Jump.WasPressedThisFrame())
            {
                abilityInputComponent.SuperAbility.Set();
            }

            foreach (var inputAbilityComponent in SystemAPI.Query<RefRW<InputAbilityComponent>>())
            {
                inputAbilityComponent.ValueRW = abilityInputComponent;
            }
        }
    }
}