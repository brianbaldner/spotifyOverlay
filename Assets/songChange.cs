using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class songChange : MonoBehaviour
{
    public float waitTime;
    float currentWaitTime;
    public CanvasGroup canvas;
    bool hasFinished;
    // Start is called before the first frame update
    void Start()
    {
        changeSong();
    }
    public void changeSong()
    {
        currentWaitTime = waitTime;
        canvas.alpha = 1f;
        hasFinished = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaitTime > 0)
        {
            currentWaitTime -= Time.deltaTime;
        }
        if(currentWaitTime <= 1)
        {
            canvas.alpha -= Time.deltaTime;
        }
        if (currentWaitTime <= 0 && !hasFinished)
        {
            canvas.alpha = 0f;
            hasFinished = true;
        }
    }
}

