using UnityEngine;
using UnityEngine.InputSystem;

namespace Weapons
{
    public class Ar15 : Firearm
    {
        [SerializeField] private Bullet prefabBullet;
        [SerializeField] private float power;
        [SerializeField] private ParticleSystem muzzleFlash;

        protected override void Fire()
        {
            if (CanAttack())
            {
                var newBullet = Instantiate(prefabBullet, tip.position, tip.rotation);
                newBullet.gameObject.SetActive(true);
                newBullet.force = power;
                newBullet.damage = damage;
                newBullet.Fire();

                TriggerShooting();
                muzzleFlash?.Play();
            }
        }

        public override void Reload(InputAction.CallbackContext _)
        {
            ReloadDefault();
        }
    }
}