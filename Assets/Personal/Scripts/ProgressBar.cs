using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] float progress = 0;
    float maxProgress = 100;

    private void Start()
    {
        GetComponent<Image>().fillAmount = 0;
    }

    public void IncreaseProgress(float value)
    {
        progress += value;
        progress = Mathf.Clamp(progress, 0, maxProgress);
        GetComponent<Image>().fillAmount = progress / 100;
    }
}
