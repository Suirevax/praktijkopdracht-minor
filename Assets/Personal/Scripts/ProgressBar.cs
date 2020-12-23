using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Events;


public class ProgressBar : MonoBehaviour
{
    //[SyncVar(hook = nameof(UpdateProgress))]
    float progress = 0;

    float maxProgress = 100;

    public static event UnityAction OnProgressFull;

    private void Start()
    {
        GetComponent<Image>().fillAmount = 0;
    }

    public void IncreaseProgress(float value)
    {
        progress = Mathf.Clamp(progress + value, 0, maxProgress);
        GetComponent<Image>().fillAmount = progress / 100;
        if (progress >= maxProgress)
        {
            OnProgressFull?.Invoke();
        }
    }

    //public void UpdateProgress(float oldValue, float newValue)
    //{
    //    GetComponent<Image>().fillAmount = progress / 100;
    //    if (progress >= maxProgress)
    //    {
    //        OnProgressFull?.Invoke();
    //    }
    //}
}
