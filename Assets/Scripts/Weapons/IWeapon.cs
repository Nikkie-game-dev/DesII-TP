
using UnityEngine.InputSystem;

namespace Weapons
{
    public interface IWeapon
    {
        protected float Damage { get; set; }
        public void Attack(InputAction.CallbackContext context);

        public bool CanAttack();

    }
}
