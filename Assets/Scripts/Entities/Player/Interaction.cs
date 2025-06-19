using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Weapons;

namespace Entities.Player
{
    public class Interaction : MonoBehaviour
    {
        [SerializeField] private InputActionReference fire;
        [SerializeField] private InputActionReference grabWeapon;
        [SerializeField] private InputActionReference reload;
        [SerializeField] private InputActionReference throwInHand;

        [SerializeField] private Transform gunHolder;
        
        [SerializeField] private GameObject centerFrame;
        
        [FormerlySerializedAs("controller")] [SerializeField] private Animator animator;
        [SerializeField] private string animParam;
        
        [FormerlySerializedAs("minDistanceToGrab")] [SerializeField]
        private float maxDistanceToGrab;

        [SerializeField] private float throwingForce;
        [SerializeField] private float correction;

        [CanBeNull] private Firearm _weaponScript;
        [CanBeNull] private Firearm _weaponGrabScript;
        [CanBeNull] private GameObject _weapon;
        [CanBeNull] private GameObject _weaponGrab;

        private Sight _sight;

        private void OnEnable()
        {
            grabWeapon.action.started += GrabWeapon;

            throwInHand.action.started += ThrowWeaponAction;

            _sight = gameObject.GetComponentInParent<Sight>();
        }

        private void GrabWeapon(InputAction.CallbackContext _)
        {
            if (!Physics.Raycast(transform.position + transform.forward * correction, transform.forward, out var lookAt,
                    maxDistanceToGrab) || !lookAt.collider.gameObject.CompareTag("Weapon")) return;

            centerFrame.SetActive(false);
            if (_weapon)
            {
                ThrowOldWeapon();
            }

            _weapon = Instantiate(lookAt.collider.transform.GetChild(0).gameObject);
            SetWeapon(lookAt.collider.gameObject);
            IkController.OnGunGrab.Invoke();
        }

        private void ThrowWeaponAction(InputAction.CallbackContext _)
        {
            centerFrame.SetActive(true);
            ThrowOldWeapon();
            UI.HudController.OnThrowGun.Invoke();
        }

        private void SetWeapon([NotNull] GameObject weaponOnGround)
        {
            if (!_weapon) return;

            _weapon.transform.SetParent(gunHolder, false);

            _weaponScript = _weapon.GetComponent<Firearm>();

            _weaponGrab = weaponOnGround;
            _weaponGrab.SetActive(false);

            if (_weaponScript)
            {
                fire.action.started += FireAction;
                reload.action.started += _weaponScript.Reload;
                
                
                _sight.SetScope(_weapon.transform.GetChild(0).gameObject);
                

                _weaponScript.StartData();
                _weaponGrabScript = _weaponGrab.transform.GetChild(0).GetComponent<Firearm>();
                if (_weaponGrabScript) _weaponScript.ammo = _weaponGrabScript.ammo;
                
                UI.HudController.OnTakeGun.Invoke();

                animator?.SetBool(Animator.StringToHash(animParam), true);
            }
            else
            {
                Debug.Log("Weapon script not attached to this weapon.");
            }
        }

        private void FireAction(InputAction.CallbackContext ctx)
        {
            if(!_weaponScript || !_weaponScript.CanAttack()) return;
            
            _weaponScript.Attack(ctx);
            UI.HudController.OnFire.Invoke();
            animator?.SetTrigger(Animator.StringToHash("Shoot"));
        }

        private void ThrowOldWeapon()
        {
            if (!_weaponGrab || !_weaponScript || !_weaponGrabScript) return;
            
            IkController.OnGunDrop.Invoke();
            
            fire.action.started -= FireAction;
            reload.action.started -= _weaponScript.Reload;

            Destroy(_weapon);

            _weaponGrab.SetActive(true);

            _weaponGrab.transform.localPosition = gunHolder.position + gunHolder.forward;
            _weaponGrab.transform.rotation = gunHolder.rotation;

            _weaponGrabScript.ammo = _weaponScript.ammo;
            _weaponGrab.GetComponent<Rigidbody>()
                .AddForce(
                    new Vector3(transform.forward.x, transform.forward.y + 1, transform.forward.z).normalized *
                    throwingForce, ForceMode.Impulse);
            animator?.SetBool(Animator.StringToHash(animParam), false);
        }

        private void OnDisable()
        {
            grabWeapon.action.started -= GrabWeapon;

            if (_weaponScript)
            {
                fire.action.started -= FireAction;
                reload.action.started -= _weaponScript.Reload;
            }


            throwInHand.action.started -= ThrowWeaponAction;
        }
    }
}