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
        [SerializeField] private Transform hand;
        [FormerlySerializedAs("minDistanceToGrab")] [SerializeField] private float maxDistanceToGrab;
        [CanBeNull] private Weapon.Weapon _weaponScript;
        private Sight _sight;
        private GameObject _weapon;

        private void OnEnable()
        {
            grabWeapon.action.started += _ => GrabWeapon();

            _weaponScript = gameObject.GetComponent<Weapon.Weapon>();
            
            _sight = gameObject.GetComponentInParent<Sight>();

            if (_weaponScript)
            {
                fire.action.started += _ => _weaponScript.Fire();
            }
        }

        private void GrabWeapon()
        {
            
            if (Physics.Raycast(transform.position, transform.forward, out var lookAt, maxDistanceToGrab) &&
                lookAt.collider.gameObject.CompareTag("Weapon"))
            {
                if (_weapon)
                {
                    Destroy(_weapon);
                }
                
                _weapon = Instantiate(lookAt.collider.transform.GetChild(0).gameObject);

                if (_weapon)
                {
                    Destroy(_weapon.GetComponent<Rigidbody>());
                    Destroy(_weapon.GetComponent<BoxCollider>());
                    
                    _weapon.transform.SetParent(hand,false);
                    
                    _weaponScript = _weapon.GetComponent<Weapon.Weapon>();

                    if (_weaponScript)
                    {
                        fire.action.started += _ => _weaponScript.Fire();
                        _sight.SetScope(_weapon.transform.GetChild(0).gameObject);
                    }
                    else
                    {
                        Debug.Log("Weapon script not attached to this weapon.");
                    }
                }
            }
        }
    }
}