using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform camTransform;
    private Vector3 originalPos;
    private float shakeTimeRemaining = 0f;
    private float shakePower = 0f;

    private void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent<Transform>();
        }
    }

    private void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    private void Update()
    {
        if (shakeTimeRemaining > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakePower;
            shakeTimeRemaining -= Time.deltaTime;
        }
        else
        {
            shakeTimeRemaining = 0f;
            camTransform.localPosition = originalPos;
        }
    }

    public void ShakeCamera(float duration, float power)
    {
        shakeTimeRemaining = duration;
        shakePower = power;
    }
}