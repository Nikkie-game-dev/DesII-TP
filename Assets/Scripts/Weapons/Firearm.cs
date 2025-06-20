using Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Weapons
{
    public abstract class Firearm : MonoBehaviour, IWeapon
    {
        [SerializeField] protected Transform tip;
        [SerializeField] protected float damage;
        [SerializeField] protected int defAmmo;
        [SerializeField] protected Animator controller;
        [HideInInspector] public int ammo;
        protected bool CanFire = true;
        private Service _weaponData;

        private void OnEnable() => ammo = defAmmo;

        public void StartData()
        {
            _weaponData = ServiceProvider.TryAddService("weaponData");
            ServiceProvider.ChangeAccess(_weaponData, AccessType.Put, GetType());
            ServiceProvider.Put(_weaponData, "ammo", GetType(), ammo);
        }
        

        float IWeapon.Damage
        {
            get => damage;
            set => damage = value;
        }

        public void Attack(InputAction.CallbackContext context)
        {
            Fire();
        }

        public bool CanAttack() => Cursor.lockState == CursorLockMode.Locked && ammo > 0 && CanFire;

        protected abstract void Fire();
        public abstract void Reload(InputAction.CallbackContext _);

        protected void TriggerShooting()
        {
            ammo--;

            ServiceProvider.Put(_weaponData, "ammo", GetType(), ammo);

            controller?.SetTrigger(Animator.StringToHash("Shoot"));
        }
        
        protected void ReloadDefault()
        {
            ammo = defAmmo;
            ServiceProvider.Put(_weaponData, "ammo", GetType(), ammo);
            UI.HudController.OnReload.Invoke();
        }
    }
}