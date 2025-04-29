using System;
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

        [FormerlySerializedAs("runSpeed")] [SerializeField]
        private float runAccel;

        [SerializeField] private float jumpForce;

        [FormerlySerializedAs("maxVelocity")] [SerializeField]
        private float maxSpeed;

        [SerializeField] private float slideRate;


        private Vector2 _movInput;
        private Vector2 _horVelocity;
        private float _accel;
        private bool _onGround;

        private void OnEnable()
        {
            _accel = acceleration;
            movement.action.started += ctx => _movInput = ctx.ReadValue<Vector2>();
            movement.action.performed += ctx => _movInput = ctx.ReadValue<Vector2>();
            movement.action.canceled += ctx => StartCoroutine(Stop(ctx));

            jump.action.started += _ => StartCoroutine(Jump());

            run.action.started += _ => _accel = runAccel;
            run.action.canceled += _ => _accel = acceleration;
        }

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

        private void FixedUpdate()
        {
            _horVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
            if (_onGround)
            {
                if (_horVelocity.magnitude <= maxSpeed)
                {
                    rb.AddForce(new Vector3(_movInput.x, 0f, _movInput.y) * _accel, ForceMode.Force);
                }
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            _onGround = other.collider.CompareTag("Ground");
        }

        private void OnCollisionExit(Collision other)
        {
            _onGround = false;
        }


        private IEnumerator Jump()
        {
            if (!_onGround) yield break;

            yield return new WaitForFixedUpdate();
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        }
    }
}