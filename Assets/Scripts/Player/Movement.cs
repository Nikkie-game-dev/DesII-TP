using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private InputActionReference movement;
        [SerializeField] private InputActionReference jump;
        [SerializeField] private InputActionReference run;
        [SerializeField] private InputActionReference crouch;
        [SerializeField] private Rigidbody rb;


        [FormerlySerializedAs("movementSpeed")] [SerializeField]
        private float acceleration;


        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpForce;

        [FormerlySerializedAs("maxSpeed")] [FormerlySerializedAs("maxVelocity")] [SerializeField]
        private float walkingSpeed;

        [SerializeField] private float slideRate;


        private Vector2 _movInput;
        private Vector2 _horVelocity;
        private float _speedLimit;
        private bool _onGround;

        private void OnEnable()
        {
            _speedLimit = walkingSpeed;
            
            movement.action.started += ctx => _movInput = ctx.ReadValue<Vector2>();
            movement.action.performed += ctx => _movInput = ctx.ReadValue<Vector2>();
            movement.action.canceled += ctx => StartCoroutine(Stop(ctx));

            jump.action.started += _ => StartCoroutine(Jump());

            run.action.started += _ => _speedLimit = runSpeed;
            run.action.canceled += _ => _speedLimit = walkingSpeed;
        }

        private void FixedUpdate()
        {
            _horVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
            
            if (_onGround)
            {
                //todo: this is not what we should be using
                var move = transform.forward * _movInput.y + transform.right * _movInput.x;
                rb.AddForce(new Vector3(move.x, 0f, move.z) * acceleration, ForceMode.Force);
                
                if (_horVelocity.magnitude > _speedLimit)
                {
                    rb.linearVelocity = new Vector3(move.x, 0f, move.z) * _speedLimit;
                }
                //todo: this is wrong, it limits the times the direction can change, rather than the speed
                //if (_horVelocity.magnitude <= _speedLimit)
                //{
                //    var move = transform.forward * _movInput.y + transform.right * _movInput.x;
                //    rb.AddForce(new Vector3(move.x, 0f, move.z) * acceleration, ForceMode.Force);
                //}
            }
            
        }

        // --------------------- Events ---------------
        private void OnCollisionEnter(Collision other)
        {
            if(other.collider.CompareTag("Ground"))
            {
                _onGround = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if(other.collider.CompareTag("Ground"))
            {
                _onGround = false;
            }
        }

        // --------------------- Coroutines ---------------

        private IEnumerator Stop(InputAction.CallbackContext ctx)
        {
            _movInput = ctx.ReadValue<Vector2>();

            yield return new WaitForFixedUpdate();

            if (_onGround)
            {
                rb.AddForce(
                    new Vector3(_horVelocity.normalized.x, 0f, _horVelocity.normalized.y) *
                    -(_horVelocity.magnitude - slideRate), ForceMode.Impulse);
            }
        }


        private IEnumerator Jump()
        {
            if (!_onGround) yield break;

            yield return new WaitForFixedUpdate();
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        }
    }
}