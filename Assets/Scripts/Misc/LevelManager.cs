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
        LoseScreen.SetActive(true);
        Time.timeScale = 0;
        HelperUtilities.UpdateCursorLock(false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AudioSource asrc;
    }
}
