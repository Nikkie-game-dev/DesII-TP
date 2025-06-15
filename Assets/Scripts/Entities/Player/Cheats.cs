using System;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Entities.Player
{
    public class Cheats : MonoBehaviour
    {
        [SerializeField] private InputActionReference godMode;
        [SerializeField] private InputActionReference flash;
        [SerializeField] private InputActionReference nextLevel;
        [SerializeField] private InputActionReference dump;

        private float _oldHealth;
        private float _oldSpeed;

        private bool _isGod;
        private bool _isFlash;
        private bool _nextLevel;

        private Stats _stats;
        private Movement _movement;

        private enum Cheat
        {
            God,
            Flash,
            NextLevel,
        }

        private void OnEnable()
        {
            _stats = gameObject.GetComponent<Stats>();
            _movement = gameObject.GetComponent<Movement>();

            godMode.action.started += _ => ToggleCheat(Cheat.God);
            flash.action.started += _ => ToggleCheat(Cheat.Flash);
            nextLevel.action.started += _ => ToggleCheat(Cheat.NextLevel);
            dump.action.started += _ => ServiceProvider.Dump();
        }

        private void ToggleCheat(Cheat cheat)
        {
            switch (cheat)
            {
                case Cheat.God:
                    if (!_isGod)
                    {
                        _stats.MakeGod(out _oldHealth);
                    }
                    else
                    {
                        _stats.MakeMortal(_oldHealth);
                    }

                    _isGod = !_isGod;

                    break;
                case Cheat.Flash:
                    if (!_isFlash)
                    {
                        _movement.MakeFlash(out _oldSpeed);
                    }
                    else
                    {
                        _movement.NotTheFastestManAlive(_oldSpeed);
                    }

                    _isFlash = !_isFlash;

                    break;
                case Cheat.NextLevel:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cheat), cheat, null);
            }
        }

        private void OnDisable()
        {
            godMode.action.started -= _ => ToggleCheat(Cheat.God);
            flash.action.started -= _ => ToggleCheat(Cheat.Flash);
            nextLevel.action.started -= _ => ToggleCheat(Cheat.NextLevel);
            dump.action.started -= _ => ServiceProvider.Dump();
        }
    }
}