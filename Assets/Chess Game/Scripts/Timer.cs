using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    public static Timer Instance;

    [SerializeField]
    private Image timerFill,TimerImage;

    [SerializeField]
    private Text timerText;

    private float timer = 0;
    private bool isRunning = false;
    private float timeLeft = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void Show()
    {
        TimerImage.gameObject.SetActive(true);
    }

    public void Hide()
    {
        TimerImage.gameObject.SetActive(false);
    }

    public void StartTimer(int timer)
    {
        this.timer = timer;
        timeLeft = timer;
        isRunning = true;
        timerText.text = timer.ToString();
        StartCoroutine(StartTimerCoroutine());
        StartCoroutine(LoseTime());
    }

    IEnumerator LoseTime()
    {
        while(timeLeft>0f)
        {
            yield return new WaitForSecondsRealtime(1);
            timeLeft--;
            timerText.text = timeLeft.ToString();
        }
    }

    IEnumerator StartTimerCoroutine()
    {
        float NormalisedTime = 0f;
        while(NormalisedTime<=1f)
        {
            timerFill.fillAmount = NormalisedTime;
            NormalisedTime += Time.deltaTime / timer;

            yield return null;
        }
        isRunning = false;
    }

    public void StopTimer()
    {
        StopAllCoroutines();
        isRunning = false;
        timerFill.fillAmount = 0f;
    }
    
    public bool IsRunning()
    {
        return isRunning;
    }

}
