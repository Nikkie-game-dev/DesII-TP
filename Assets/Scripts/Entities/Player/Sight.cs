using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Entities.Player
{
    public class Sight : MonoBehaviour
    {
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
            _scope?.SetActive(!_scope.activeSelf);
            firstPersonView.SetActive(true);
            _sensitivity = sensitivity;
            animator?.SetBool(Animator.StringToHash(animParam), _scope != null && _scope.activeSelf);
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
            scopeIn.action.canceled -= ScopeIn;
        }

        private void OnDisable()
        {
            look.action.started -= ReadValue;
            look.action.performed -= ReadValue;
            look.action.canceled -= ReadValue;

        }
    }
}