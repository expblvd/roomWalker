using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.2f;

    private Vector3 originalPos;
    private float shakeTimeRemaining = 0f;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeTimeRemaining > 0)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = originalPos + randomOffset;

            shakeTimeRemaining -= Time.unscaledDeltaTime;

            if (shakeTimeRemaining <= 0f)
            {
                transform.localPosition = originalPos;
            }
        }
    }

    public void Shake(float duration = -1f, float magnitude = -1f)
    {
        shakeTimeRemaining = duration > 0 ? duration : shakeDuration;
        shakeMagnitude = magnitude > 0 ? magnitude : shakeMagnitude;
    }
}