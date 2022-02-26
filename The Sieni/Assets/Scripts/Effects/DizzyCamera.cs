using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DizzyCamera : MonoBehaviour
{
    [SerializeField]
    private Transform cameraRoot;

    [SerializeField]
    private float fadeSpeed = 1.0f;

    [SerializeField]
    private float strength = 360.0f;

    [SerializeField]
    private float interval = 1.0f;

    [SerializeField]
    private bool debugActivate;

    [SerializeField]
    private bool debugDeactivate;

    private float intensity = 0.0f;
    private float targetIntensity = 0.0f;

    private Quaternion origRotation;

    // Start is called before the first frame update
    void Start()
    {
        origRotation = cameraRoot.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (debugActivate) {
            SetDizzy(true);
            debugActivate = false;
        }
        if (debugDeactivate) {
            SetDizzy(false);
            debugDeactivate = false;
        }

        if (intensity < targetIntensity) {
            intensity += fadeSpeed * Time.deltaTime;
            if (intensity > targetIntensity) {
                intensity = targetIntensity;
            }
        }
        if (intensity > targetIntensity) {
            intensity -= fadeSpeed * Time.deltaTime;
            if (intensity < targetIntensity) {
                intensity = targetIntensity;
            }
        }

        HandleCameraRotation();
    }

    public void SetDizzy(bool dizzy) {
        if (dizzy) {
            targetIntensity = 1.0f;
        } else {
            targetIntensity = 0.0f;
        }
    }

    private void HandleCameraRotation() {
        var angle = strength * Mathf.Sin(Time.time * 2 * Mathf.PI * interval);
        var rotation = origRotation * Quaternion.AngleAxis(angle * intensity, Vector3.forward);
        cameraRoot.rotation = rotation;
    }
}
