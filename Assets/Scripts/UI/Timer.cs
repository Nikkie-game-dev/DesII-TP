using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class Script : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerDisplay;
        [SerializeField] private float timeToBeat;
        private float _timer;

        private void FixedUpdate()
        {
            _timer += Time.fixedDeltaTime;

            if (_timer >= timeToBeat)
            {
                StartCoroutine(Lose());
            }
            else
            {
                timerDisplay.text = Math.Round(_timer, 2).ToString();
            }
        }

        private IEnumerator Lose()
        {
            timerDisplay.text = "YOU LOST! RESTARTING IN 3";
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
