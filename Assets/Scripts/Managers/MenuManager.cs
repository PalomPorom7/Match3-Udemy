using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Fader loadingScreen;

    private void Start()
    {
        loadingScreen.Hide(false);
        StartCoroutine(loadingScreen.Fade(0));
    }

    private IEnumerator Quit()
    {
        yield return StartCoroutine(loadingScreen.Fade(1));
        Application.Quit();
    }

    private IEnumerator StartSurvivalGame()
    {
        yield return StartCoroutine(loadingScreen.Fade(1));
        SceneManager.LoadScene("Survival");
    }

    private IEnumerator StartTimeRushGame()
    {
        yield return StartCoroutine(loadingScreen.Fade(1));
        SceneManager.LoadScene("Time Rush");
    }

    public void QuitButtonPressed()
    {
        StartCoroutine(Quit());
    }

    public void SurvivalButtonPressed()
    {
        StartCoroutine(StartSurvivalGame());
    }

    public void TimeRushButtonPressed()
    {
        StartCoroutine(StartTimeRushGame());
    }
}
