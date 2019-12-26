using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSequence : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEndingSequenceEnded()
    {
        ScreenFader.Instance.FadeOutAndExecute(() => { SceneManager.LoadScene("MainMenu"); }, 5f);
    }
}
