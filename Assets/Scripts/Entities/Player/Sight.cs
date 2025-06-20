using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Entities.Player
{
    public class Sight : MonoBehaviour
    {
        public static event Action OnScopeIn;
        public static event Action OnScopeOut;
        
        [SerializeField] private InputActionReference look;
        [SerializeField] private InputActionReference scopeIn;
        
        [SerializeField] private Transform head;
        
        [SerializeField] private float sensitivity;
        [SerializeField] private float aimSensitivity;
        [SerializeField] private float highLimitAngle;
        [SerializeField] private float lowLimitAngle;
        
        [FormerlySerializedAs("controller")] [SerializeField] private Animator animator;
        
        [SerializeField] private string animParam;



        [FormerlySerializedAs("FirstPersonView")] [FormerlySerializedAs("main")] [SerializeField]
        private GameObject firstPersonView;

        [CanBeNull] private GameObject _scope;

        private Vector2 _look;
        private float _sensitivity;

        private void OnEnable()
        {
            _sensitivity = sensitivity;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            look.action.started += ReadValue;
            look.action.performed += ReadValue;
            look.action.canceled += ReadValue;
            
            
        }

        private void ScopeIn(InputAction.CallbackContext _)
        {
            _sensitivity = sensitivity;
            _scope?.SetActive(true);
            firstPersonView.SetActive(false);
            animator?.SetBool(Animator.StringToHash(animParam), true);
            OnScopeIn?.Invoke();
        }
        
        private void ScopeOut(InputAction.CallbackContext _)
        {
            _scope?.SetActive(false);
            firstPersonView.SetActive(true);
            animator?.SetBool(Animator.StringToHash(animParam), false);
            OnScopeOut?.Invoke();
        }

        private void ReadValue(InputAction.CallbackContext ctx)
        {
            _look = ctx.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            var addedAngle = _look * (_sensitivity * Time.fixedDeltaTime);
            var angle = head.rotation.eulerAngles.x + -addedAngle.y;

            transform.Rotate(Vector3.up, addedAngle.x);

            if (angle > lowLimitAngle && angle < 180)
            {
                head.transform.eulerAngles = new Vector3(lowLimitAngle, head.transform.eulerAngles.y,
                    head.transform.eulerAngles.z);
            }
            else if (angle < highLimitAngle && angle > 180)
            {
                head.transform.eulerAngles = new Vector3(highLimitAngle, head.transform.eulerAngles.y,
                    head.transform.eulerAngles.z);
            }
            else
            {
                head.transform.Rotate(Vector3.right, -addedAngle.y);
            }
        }

        public void SetScope([NotNull] GameObject scope)
        {
            if (!scope) return;

            _scope = scope;

            scopeIn.action.started += ScopeIn;
            scopeIn.action.canceled -= ScopeOut;
        }

        private void OnDisable()
        {
            look.action.started -= ReadValue;
            look.action.performed -= ReadValue;
            look.action.canceled -= ReadValue;

        }
    }
}