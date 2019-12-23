using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : SingletonMonoBehaviour<PauseScreen>
{
    public bool isPaused => pauseScreenRoot.activeSelf;

    public GameObject pauseScreenRoot;

    // Start is called before the first frame update
    void Start()
    {
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        pauseScreenRoot.SetActive(true);
        Time.timeScale = 0;
        HelperUtilities.UpdateCursorLock(false);
    }

    public void Resume()
    {
        pauseScreenRoot.SetActive(false);
        Time.timeScale = 1;
        HelperUtilities.UpdateCursorLock(true);
    }
}
