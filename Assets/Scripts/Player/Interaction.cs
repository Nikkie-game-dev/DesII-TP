using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


namespace Player
{
    public class Interaction : MonoBehaviour
    {
        [SerializeField] private InputActionReference fire;
        [SerializeField] private InputActionReference grabWeapon;
        [SerializeField] private InputActionReference reload;
        [SerializeField] private InputActionReference throwInHand;
        [SerializeField] private Transform hand;
        [SerializeField] private GameObject centerFrame;
        [SerializeField] private Animator controller;
        [SerializeField] private string animParam;

        [FormerlySerializedAs("minDistanceToGrab")] [SerializeField]
        private float maxDistanceToGrab;

        [SerializeField] private float throwingForce;

        [CanBeNull] private Weapon.Weapon _weaponScript;
        [CanBeNull] private Weapon.Weapon _weaponGrabScript;
        [CanBeNull] private GameObject _weapon;
        [CanBeNull] private GameObject _weaponGrab;

        private Sight _sight;

        private void OnEnable()
        {
            grabWeapon.action.started += GrabWeapon;

            throwInHand.action.started += _ =>
            {
                centerFrame.SetActive(true);
                ThrowOldWeapon();
                UI.HudController.OnThrowGun.Invoke();
            };

            _sight = gameObject.GetComponentInParent<Sight>();
        }

        private void GrabWeapon(InputAction.CallbackContext _)
        {
            if (!Physics.Raycast(transform.position, transform.forward, out var lookAt, maxDistanceToGrab) ||
                !lookAt.collider.gameObject.CompareTag("Weapon")) return;

            centerFrame.SetActive(false);
            if (_weapon)
            {
                ThrowOldWeapon();
            }

            _weapon = Instantiate(lookAt.collider.transform.GetChild(0).gameObject);
            SetWeapon(lookAt.collider.gameObject);
        }

        private void SetWeapon([NotNull] GameObject weaponOnGround)
        {
            if (!_weapon) return;

            _weapon.transform.SetParent(hand, false);
            
            _weaponScript = _weapon.GetComponent<Weapon.Weapon>();

            _weaponGrab = weaponOnGround;
            _weaponGrab.SetActive(false);

            if (_weaponScript)
            {
                fire.action.started += FireAction;

                _sight.SetScope(_weapon.transform.GetChild(0).gameObject);

                _weaponScript.StartData();

                UI.HudController.OnTakeGun.Invoke();

                reload.action.started += _weaponScript.Reload;

                _weaponGrabScript = _weaponGrab.transform.GetChild(0).GetComponent<Weapon.Weapon>();

                if (_weaponGrabScript) _weaponScript.ammo = _weaponGrabScript.ammo;

                controller?.SetBool(Animator.StringToHash(animParam), true);
            }
            else
            {
                Debug.Log("Weapon script not attached to this weapon.");
            }
        }

        private void FireAction(InputAction.CallbackContext ctx)
        {
            _weaponScript?.Fire();
            UI.HudController.OnFire.Invoke();
        }

        private void ThrowOldWeapon()
        {
            if (!_weaponGrab || !_weaponScript || !_weaponGrabScript) return;

            fire.action.started -= FireAction;
            reload.action.started -= _weaponScript.Reload;

            Destroy(_weapon);

            _weaponGrab.SetActive(true);

            _weaponGrab.transform.localPosition = hand.position;
            _weaponGrab.transform.rotation = hand.rotation;

            _weaponGrabScript.ammo = _weaponScript.ammo;
            _weaponGrab.GetComponent<Rigidbody>()
                .AddForce(
                    new Vector3(transform.forward.x, transform.forward.y + 1, transform.forward.z).normalized *
                    throwingForce, ForceMode.Impulse);
            controller?.SetBool(Animator.StringToHash(animParam), false);
        }

        private void OnDisable()
        {
            grabWeapon.action.started -= GrabWeapon;

            if (_weaponScript)
            {
                fire.action.started -= FireAction;
                reload.action.started -= _weaponScript.Reload;
            }


            throwInHand.action.started -= _ =>
            {
                centerFrame.SetActive(true);
                ThrowOldWeapon();
                UI.HudController.OnThrowGun.Invoke();
            };
        }
    }
}