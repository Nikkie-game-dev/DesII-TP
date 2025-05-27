using Services;
using UnityEngine;

namespace Player
{
    public class Stats : MonoBehaviour
    {
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;

        private Service _playerData;
        private void OnEnable()
        {
            _playerData = ServiceProvider.TryAddService("playerData");
            ServiceProvider.ChangeAccess(_playerData, AccessType.Put, GetType());
        }

        public void ReceiveDamage(float damage)
        {
            health -= damage;
            ServiceProvider.Put(_playerData, "health", GetType(), health);
            UI.HudController.OnTakeDamage.Invoke();
        }

        public void MakeGod(out float oldHealth)
        {
            oldHealth = health;
            health = float.PositiveInfinity;
        }
        public void MakeMortal(float oldHealth)
        {
            health = oldHealth;
        }
    }
}