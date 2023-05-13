using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    private GameManager gm;

    [SerializeField]
    private Text timerText;

    private float timeRemaining;
    private string timeAsString;

    private bool counting;

    private void Start()
    {
        gm = GameManager.Instance;
    }

    public void SetTimer(float t)
    {
        StopAllCoroutines();
        timeRemaining = t;
        UpdateText();
    }

    private void UpdateText()
    {
        timeAsString = (int) timeRemaining / 60 + " : ";
        timeAsString += timeRemaining % 60 < 10 ? "0" : "";
        timerText.text = timeAsString + (int) timeRemaining % 60;
    }

    public IEnumerator Countdown()
    {
        counting = true;
        do
        {
            timeRemaining -= Time.deltaTime;
            UpdateText();
            yield return null;
        }
        while(timeRemaining > 0);

        counting = false;

        gm.GameOver();
    }
}
