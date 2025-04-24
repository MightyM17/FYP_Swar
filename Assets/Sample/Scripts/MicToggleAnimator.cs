using UnityEngine;
using UnityEngine.UI;

public class MicToggleAnimator : MonoBehaviour
{
    public Toggle micToggle;
    public float pulseSpeed = 2f;
    public float scaleAmount = 1.1f;

    private bool isPulsing = false;
    private Vector3 originalScale;

    public VoiceProcessor VoiceProcessor;

    void Awake()
    {
        if (micToggle == null)
        {
            micToggle = GetComponent<Toggle>();
        }

        originalScale = micToggle.transform.localScale;
    }

    public void StartPulsing()
    {
        if (!isPulsing)
        {
            isPulsing = true;
            StartCoroutine(Pulse());
        }
    }

    public void StopPulsing()
    {
        isPulsing = false;
        micToggle.transform.localScale = originalScale;
    }

    private System.Collections.IEnumerator Pulse()
    {
        while (isPulsing)
        {
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            float scale = Mathf.Lerp(1f, scaleAmount, t);
            micToggle.transform.localScale = originalScale * scale;
            yield return null;
        }
    }
}
