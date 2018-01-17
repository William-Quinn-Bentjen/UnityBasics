using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimerUI : MonoBehaviour {

    public Text timerLabel;
    public float duration;
    public bool isRunning;
    public bool LoadSceneWhenTimeIsUp = false;
    public string SceneToLoad;
    void Update()
    {
        if (isRunning == true)
        {
            duration -= Time.deltaTime;
            int temp = (int)duration;
            int hours = temp / 60 / 60;
            int mins = temp / 60;
            int secs = temp % 60;
            timerLabel.text = hours.ToString("D2") + ":" + mins.ToString("D2") + ":" + secs.ToString("D2");
            if (duration <= 0)
            {
                isRunning = false;
                if (LoadSceneWhenTimeIsUp)
                {
                    SceneManager.LoadScene(SceneToLoad);
                }
            }
        }
    }
    public void ToggleTimer()
    {
        isRunning = !isRunning;
    }
}
