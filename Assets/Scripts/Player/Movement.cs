using System.Collections;
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
        }

        private void FixedUpdate()
        {
            Move(transform.forward * _movInput.y + transform.right * _movInput.x);
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