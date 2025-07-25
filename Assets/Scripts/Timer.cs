using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]float timeForContinuing;
    public bool isTimerStarted = false;
    float timerValue;
    float fillFraction;

    private void Update()
    {
        if (isTimerStarted)
        { 
            RunSwitchBaseBLockOptionTimer();
        }
    }

    private void Awake()
    {
        timerValue = timeForContinuing;
    }

    public void ResetTimerValue()
    {
        timerValue = timeForContinuing;
    }
    public void CancelTimer()
    {
        timerValue = 0;
    }
    public void RunSwitchBaseBLockOptionTimer()
    {
        timerValue-= Time.deltaTime;
        //Debug.Log(timerValue);
        if (timerValue > 0)
        {
            fillFraction = timerValue / timeForContinuing;
            this.gameObject.GetComponent<Image>().fillAmount = fillFraction;
        }
        else
        {
            isTimerStarted = false;
            UIManager.Instance.CustomBaseBlockSwitchButtonWithTimer.SetActive(false);
            UIManager.Instance.OnGameOverWithNOBaseBlockRemaining(true);
            ResetTimerValue();
        }
    }
}
