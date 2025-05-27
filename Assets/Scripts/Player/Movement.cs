using System.Collections;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
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

            movement.action.started += ctx => _movInput = ctx.ReadValue<Vector2>();
            movement.action.performed += ctx => _movInput = ctx.ReadValue<Vector2>();
            
            movement.action.canceled += ctx =>
            {
                _movInput = ctx.ReadValue<Vector2>();
            };

            jump.action.started += _ => StartCoroutine(Jump());

            run.action.started += _ => SpeedLimit = runSpeed;
            run.action.canceled += _ => SpeedLimit = walkingSpeed;
            
            Service = ServiceProvider.TryAddService("playerData");
            ServiceProvider.ChangeAccess(Service, AccessType.Set, GetType());
            ServiceProvider.ChangeAccess(Service, AccessType.Get, typeof(Enemy.Movement));
        }

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
    }
}