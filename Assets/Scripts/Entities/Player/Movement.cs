using System.Collections;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Entities.Player
{
    public class Movement : Entity
    {
        [SerializeField] private InputActionReference movement;
        [SerializeField] private InputActionReference jump;
        [SerializeField] private InputActionReference run;
        [SerializeField] private InputActionReference crouch;
        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpForce;

        [FormerlySerializedAs("maxSpeed")] [FormerlySerializedAs("maxVelocity")] [SerializeField]
        private float walkingSpeed;
        
        private Vector2 _movInput;

        private void OnEnable()
        {
            SpeedLimit = walkingSpeed;

            movement.action.started += ReadValue;
            movement.action.performed += ReadValue;
            movement.action.canceled += ReadValue;

            jump.action.started += ActionJump;

            run.action.started += _ => SpeedLimit = runSpeed;
            run.action.canceled += _ => SpeedLimit = walkingSpeed;
            
            Service = ServiceProvider.TryAddService("playerData");
            ServiceProvider.ChangeAccess(Service, AccessType.Put, GetType());
            ServiceProvider.ChangeAccess(Service, AccessType.Get, typeof(Enemy.Movement));
        }

        private void ActionJump(InputAction.CallbackContext _) => StartCoroutine(Jump());
        

        private void FixedUpdate()
        {
            HorVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
            Move(transform.forward * _movInput.y + transform.right * _movInput.x);
            ServiceProvider.Put(Service, "position", GetType(), transform.position);
        }


        private IEnumerator Jump()
        {
            if (!onGround) yield break;

            yield return new WaitForFixedUpdate();
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        }

        public void MakeFlash(out float oldSpeed)
        {
            oldSpeed = walkingSpeed;
            walkingSpeed *= 3;
            SpeedLimit = walkingSpeed;
        }
        public void NotTheFastestManAlive(float oldSpeed)
        {
            walkingSpeed = oldSpeed;
            SpeedLimit = walkingSpeed;
        }

        private void OnDisable()
        {
            movement.action.started -= ReadValue;
            movement.action.performed -= ReadValue;
            movement.action.canceled -= ReadValue;

            jump.action.started -= ActionJump;

            run.action.started -= _ => SpeedLimit = runSpeed;
            run.action.canceled -= _ => SpeedLimit = walkingSpeed;
        }

        private void ReadValue(InputAction.CallbackContext ctx)
        {
            _movInput = ctx.ReadValue<Vector2>();
        }
    }
}