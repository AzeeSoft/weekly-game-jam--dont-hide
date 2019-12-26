using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    public GameObject WinScreen;
    public GameObject LoseScreen;

    // Start is called before the first frame update
    void Start()
    {
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        GameManager.Instance.playerModel.health.OnHealthDepleted.AddListener(OnGameLost);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnGameLost()
    {
        this.WaitAndExecute(() =>
        {
            LoseScreen.SetActive(true);
            Time.timeScale = 0;
            HelperUtilities.UpdateCursorLock(false);
        }, GameManager.Instance.playerModel.deathAnimationDuration);
    }

    public void RestartLevel()
    {
        ScreenFader.Instance.FadeOutAndExecute(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
    }
}