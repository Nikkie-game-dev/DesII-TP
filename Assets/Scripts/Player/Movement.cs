using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private InputActionReference movement;
        [SerializeField] private InputActionReference jump;
        [SerializeField] private InputActionReference run;
        [SerializeField] private InputActionReference crouch;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float maxVelocity;
        
        
        private Vector2 _movement;
        private float _speed;
        private bool _onGround;


        private void OnEnable()
        {
            _speed = movementSpeed;
                
            movement.action.started += ctx => _movement = ctx.ReadValue<Vector2>();
            movement.action.performed += ctx => _movement = ctx.ReadValue<Vector2>();
            movement.action.canceled += ctx => _movement = ctx.ReadValue<Vector2>();

            jump.action.started += _ => StartCoroutine(Jump());
            
            run.action.started += _ => _speed = runSpeed;
            run.action.canceled += _ => _speed = movementSpeed;
            

        }

        private void FixedUpdate()
        {
            if (_onGround)
            {
                if (rb.linearVelocity.magnitude <= maxVelocity)
                {
                    rb.AddForce(new Vector3(_movement.x, 0f, _movement.y) * _speed, ForceMode.Force);
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
