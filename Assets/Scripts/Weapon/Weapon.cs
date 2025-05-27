using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Weapon
{
    public class Weapon : MonoBehaviour
    {
        [FormerlySerializedAs("prefabricatedBullet")] [SerializeField]
        private Bullet prefabBullet;

        [SerializeField] private Transform tip;
        [SerializeField] private float power;
        [SerializeField] private float damage;
        [SerializeField] private int defAmmo;
        [SerializeField] private bool isHitScan;

        [HideInInspector] public int ammo;

        private Service _weaponData;

        private void OnEnable()
        {
            ammo = defAmmo;
        }

        public void StartData()
        {
            _weaponData = ServiceProvider.TryAddService("weaponData");
            ServiceProvider.ChangeAccess(_weaponData, AccessType.Put, GetType());
            ServiceProvider.Put(_weaponData, "ammo", GetType(), ammo);
        }


        public void Fire(InputAction.CallbackContext _)
        {
            if (ammo > 0)
            {
                if (!isHitScan)
                {
                    var newBullet = Instantiate(prefabBullet, tip.position, tip.rotation);
                    newBullet.gameObject.SetActive(true);
                    newBullet.force = power;
                    newBullet.damage = damage;
                    newBullet.Fire();
                }
                else
                {
                    Debug.DrawRay(tip.position, tip.forward * power, Color.red, 3);

                    if (Physics.Raycast(tip.position, tip.forward, out var hit))
                    {
                        var objective = hit.transform.gameObject.GetComponent<Enemy.Stats>();
                        if (objective && objective.CompareTag("Enemy"))
                        {
                            objective.ReceiveDamage(damage);
                        }
                    }
                }
                ammo--;
            }

            ServiceProvider.Put(_weaponData, "ammo", GetType(), ammo);
        }

        public void Reload()
        {
            ammo = defAmmo;
        }
    }
}