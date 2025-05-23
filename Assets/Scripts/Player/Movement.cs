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

        [SerializeField] private float slideRate;


        private Vector2 _movInput;

        private void OnEnable()
        {
            SpeedLimit = walkingSpeed;

            movement.action.started += ctx => _movInput = ctx.ReadValue<Vector2>();
            movement.action.performed += ctx => _movInput = ctx.ReadValue<Vector2>();
            movement.action.canceled += ctx => StartCoroutine(Stop(ctx));

            jump.action.started += _ => StartCoroutine(Jump());

            run.action.started += _ => SpeedLimit = runSpeed;
            run.action.canceled += _ => SpeedLimit = walkingSpeed;

            Service = new Service("playerData");
            ServiceProvider.TryAddService(Service);
            ServiceProvider.ChangeAccess(Service, AccessType.Set, GetType());
            ServiceProvider.ChangeAccess(Service, AccessType.Get, typeof(Enemy.Movement));
        }

        private void FixedUpdate()
        {
            HorVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
            Move(transform.forward * _movInput.y + transform.right * _movInput.x);
            ServiceProvider.Put(Service, "position", GetType(), transform.position);
        }

        private IEnumerator Stop(InputAction.CallbackContext ctx)
        {
            _movInput = ctx.ReadValue<Vector2>();

            yield return new WaitForFixedUpdate();

            if (onGround)
            {
                rb.AddForce(
                    new Vector3(HorVelocity.normalized.x, 0f, HorVelocity.normalized.y) *
                    -(HorVelocity.magnitude - slideRate), ForceMode.Impulse);
            }
        }


        private IEnumerator Jump()
        {
            if (!onGround) yield break;

            yield return new WaitForFixedUpdate();
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        }
    }
}