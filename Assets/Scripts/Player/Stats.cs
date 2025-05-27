using UnityEngine;

namespace Player
{
    public class Stats : MonoBehaviour
    {
        public void ReceiveDamage()
        {
            print("Ouch!");
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