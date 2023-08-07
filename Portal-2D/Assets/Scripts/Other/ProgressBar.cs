using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for managing progress bar
/// </summary>
public class ProgressBar : MonoBehaviour
{
    [SerializeField] Image progressBar;
    [SerializeField, Range(0f, 1f)] float initialValue;
    RectTransform rectTransform;
    float progressBarValue;
    float progressBarMaxValue;

    /// <summary>
    /// Sets the value of the progress bar
    /// </summary>
    /// <param name="value">input value</param>
    public void SetProgressBarValue(float value)
    {
        UpdateProgressBar(value);
    }
    /// <summary>
    /// Awake is called when the script instance is being loaded - here we initialize the progress bar
    /// </summary>
    void Awake()
    {
        rectTransform = progressBar.GetComponent<RectTransform>();
        progressBarMaxValue = rectTransform.sizeDelta.x;
        UpdateProgressBar(initialValue);
    }
    /// <summary>
    /// Updates progress bar with given value
    /// </summary>
    /// <param name="value">input value</param>
    void UpdateProgressBar(float value)
    {
        progressBarValue = Mathf.Clamp01(value);
        rectTransform.sizeDelta = new Vector2(progressBarMaxValue * progressBarValue, rectTransform.sizeDelta.y);
    }

}
