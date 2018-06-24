using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;
	Quaternion originalRot;

    void OnEnable()
    {
        originalPos = Camera.main.transform.localPosition;
		originalRot = Camera.main.transform.rotation;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            Camera.main.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
			Camera.main.transform.localPosition = originalPos;
			Camera.main.transform.rotation = originalRot;
			this.enabled = false;
        }
    }
}