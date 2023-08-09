using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnGameOverObj : MonoBehaviour
{
    float timer = 5f;
    [SerializeField] private GameOverObj gameOverObj;
    [SerializeField] private Text timerText;
    bool timerDone;


    void Update()
    {
        timerText.text = string.Format("00:{0:00}", timer.ToString("00"));
        
        if (timer > 0)
            timer -= Time.deltaTime;

        if(timer <= 0 && !timerDone)
        {
            gameOverObj.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
            gameOverObj.Box.enabled = true;
            timerDone = true;
        }
    }

    
}
