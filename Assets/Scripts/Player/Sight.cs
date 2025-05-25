using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
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
        [SerializeField] private GameObject main;
        [CanBeNull] private GameObject _scope;

        private Vector2 _look;
        private float _sensitivity;

        private void OnEnable()
        {
            _sensitivity = sensitivity;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            look.action.started += _ => _look = look.action.ReadValue<Vector2>();
            look.action.performed += _ => _look = look.action.ReadValue<Vector2>();
            look.action.canceled += _ => _look = look.action.ReadValue<Vector2>();
            
            if(_scope)
            {
                scopeIn.action.started += _ =>
                {
                    _scope.SetActive(true);
                    main.SetActive(false);
                    _sensitivity = aimSensitivity;
                };
                scopeIn.action.canceled += _ =>
                {
                    _scope.SetActive(false);
                    main.SetActive(true);
                    _sensitivity = sensitivity;
                };
            }
        }

        private void FixedUpdate()
        {
            var addedAngle = _look * (_sensitivity * Time.fixedDeltaTime);
            var angle = head.rotation.eulerAngles.x + -addedAngle.y;
            
            transform.Rotate(Vector3.up, addedAngle.x);

            if (angle > lowLimitAngle && angle < 180)
            {
                head.transform.eulerAngles = new Vector3(lowLimitAngle, head.transform.eulerAngles.y, head.transform.eulerAngles.z);
            }
            else if (angle < highLimitAngle && angle > 180)
            {
                head.transform.eulerAngles = new Vector3(highLimitAngle, head.transform.eulerAngles.y, head.transform.eulerAngles.z);
            }
            else
            {
                head.transform.Rotate(Vector3.right, -addedAngle.y);
            }
            
        }

        public void SetScope([NotNull] GameObject scope)
        {
            if(!scope) return;
            
            _scope = scope;

            scopeIn.action.started += _ =>
            {
                _scope.SetActive(true);
                main.SetActive(false);
                _sensitivity = aimSensitivity;
            };
            scopeIn.action.canceled += _ =>
            {
                _scope.SetActive(false);
                main.SetActive(true);
                _sensitivity = sensitivity;
            };
        }
    }
}