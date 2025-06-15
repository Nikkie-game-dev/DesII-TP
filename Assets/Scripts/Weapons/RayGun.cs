using System.Collections;
using Entities.Enemy;
using Services;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Weapons
{
    public class RayGun : Firearm
    {
        [SerializeField] private GameObject laser;
        [SerializeField] private float laserLifeTime;

        private IEnumerator DestroyLaser(GameObject instance)
        {
            yield return new WaitForSeconds(laserLifeTime);
            Destroy(instance);
        }

        protected override void Fire()
        {
            if (!CanAttack()) return;

            if (Physics.Raycast(tip.position, tip.forward, out var hit))
            {
                var objective = hit.transform.gameObject.GetComponent<Stats>();
                if (objective && objective.CompareTag("Enemy"))
                {
                    objective.ReceiveDamage(damage);
                }
            }

            var laserInstance = Instantiate(laser);
            laserInstance.SetActive(true);
            laserInstance.transform.position = tip.position;
            laserInstance.transform.rotation = tip.rotation;
            StartCoroutine(DestroyLaser(laserInstance));
            
            ammo--;

            ServiceProvider.Put(WeaponData, "ammo", GetType(), ammo);
        }

        public override void Reload(InputAction.CallbackContext _)
        {
            ReloadDefault();
            // TODO
        }
    }
}