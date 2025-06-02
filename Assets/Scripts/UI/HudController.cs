using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Services;
using UnityEngine.Events;

namespace UI
{
    public class HudController : MonoBehaviour
    {
        public static UnityEvent OnTakeDamage;
        public static UnityEvent OnFire;
        public static UnityEvent OnTakeGun;
        public static UnityEvent OnThrowGun;
        public static UnityEvent OnReload;

        [SerializeField] private TMP_Text ammo;
        [SerializeField] private Image health;

        private float _health;
        private float _maxHealth;
        private int _ammo;
        private Service _playerData;
        private Service _weaponData;

        private void OnEnable()
        {
            OnTakeDamage = new UnityEvent();
            OnFire = new UnityEvent();
            OnTakeGun = new UnityEvent();
            OnReload = new UnityEvent();
            OnThrowGun = new UnityEvent();

            _playerData = ServiceProvider.TryAddService("playerData");
            _weaponData = ServiceProvider.TryAddService("weaponData");

            ServiceProvider.ChangeAccess(_playerData, AccessType.Get, GetType());
            ServiceProvider.ChangeAccess(_weaponData, AccessType.Get, GetType());

            OnFire.AddListener(UpdateAmmo);
            OnTakeGun.AddListener(UpdateAmmo);
            OnReload.AddListener(UpdateAmmo);
            OnTakeDamage.AddListener(UpdateHealth);
            OnThrowGun.AddListener(RemoveAmmo);
        }

        private void UpdateAmmo()
        {
            _ammo = (int) ServiceProvider.Get(_weaponData, "ammo", GetType())!;
            ammo.text = _ammo.ToString();
        }

        private void UpdateHealth()
        {
            _health = (float)ServiceProvider.Get(_playerData, "health", GetType())!;
            health.color = new Color(255, 0, 0, (255 - 255 * (_health / _maxHealth)));
        }

        private void RemoveAmmo()
        {
            ammo.text = "";
        }
    }
}