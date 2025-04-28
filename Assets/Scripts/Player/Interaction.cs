using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Interaction : MonoBehaviour
    {
        [SerializeField] private Weapon.Weapon weapon;
        [SerializeField] private InputActionReference fire;

        private void OnEnable()
        {
            fire.action.started += _ => weapon.Fire();
        }
    }
}
