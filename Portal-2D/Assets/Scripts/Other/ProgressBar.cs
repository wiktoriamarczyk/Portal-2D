using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Image progressBar;
    [SerializeField, Range(0f, 1f)] float initialValue;
    RectTransform rectTransform;
    float progressBarValue;
    float progressBarMaxValue;

    public void SetProgressBarValue(float value)
    {
        UpdateProgressBar(value);
    }

    void Awake()
    {
        rectTransform = progressBar.GetComponent<RectTransform>();
        progressBarMaxValue = rectTransform.sizeDelta.x;
        UpdateProgressBar(initialValue);
    }

    void UpdateProgressBar(float value)
    {
        progressBarValue = Mathf.Clamp01(value);
        rectTransform.sizeDelta = new Vector2(progressBarMaxValue * progressBarValue, rectTransform.sizeDelta.y);
    }

}
