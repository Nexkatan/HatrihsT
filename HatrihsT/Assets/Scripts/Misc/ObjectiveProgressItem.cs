using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveProgressItem : MonoBehaviour
{
    public Slider progress;
    public TextMeshProUGUI progressText;

    public float fillSpeed = 0.5f;
    public float targetProgress = 0f;

    private void Update()
    {
        if (progress.value < targetProgress)
        {
            progress.value += fillSpeed * Time.deltaTime;
        }
    }

    public void IncrementProgress(float newProgress)
    {
        targetProgress = progress.value + newProgress;
    }
}
