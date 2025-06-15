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
        [HideInInspector] public int ammo;
        protected Service WeaponData;
        [SerializeField] protected Animator controller;

        private void OnEnable() => ammo = defAmmo;

        public void StartData()
        {
            WeaponData = ServiceProvider.TryAddService("weaponData");
            ServiceProvider.ChangeAccess(WeaponData, AccessType.Put, GetType());
            ServiceProvider.Put(WeaponData, "ammo", GetType(), ammo);
        }
        

        float IWeapon.Damage
        {
            get => damage;
            set => damage = value;
        }

        public void Attack(InputAction.CallbackContext  context)
        {
            Fire();
            controller.SetTrigger(Animator.StringToHash("Fire"));
        }

        public bool CanAttack() => Cursor.lockState == CursorLockMode.None && ammo > 0;

        protected abstract void Fire();
        public abstract void Reload(InputAction.CallbackContext _);

        protected void ReloadDefault()
        {
            ammo = defAmmo;
            ServiceProvider.Put(WeaponData, "ammo", GetType(), ammo);
            UI.HudController.OnReload.Invoke();
        }
    }
}