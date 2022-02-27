using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AcidEffect : MonoBehaviour
{

    [SerializeField]
    private float acidFadeSpeed = 1.0f;

    [SerializeField]
    private List<Volume> volumes;

    [SerializeField]
    private Volume additionalEffects;

    [SerializeField]
    private bool debugActivateAcid;

    [SerializeField]
    private bool debugDeactivateAcid;

    private float acidIntensity = 0.0f;
    private float targetAcidIntensity = 0.0f;

    private List<ColorLookup> colorLookups = new List<ColorLookup>();
    private List<float> originalContributions = new List<float>();

    private LensDistortion lensDistortion;
    private float origLensDistortionIntensity;
    private float throbbingIntensity = 0.25f;
    private float throbbingFrequency = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        volumes.ForEach(it => {
            var profile = it.profile;
            if (!profile) throw new System.NullReferenceException(nameof(VolumeProfile));
            ColorLookup colorLookup;
            if (!profile.TryGet(out colorLookup)) throw new System.NullReferenceException(nameof(colorLookup));
            colorLookups.Add(colorLookup);
            originalContributions.Add(colorLookup.contribution.value);
        });

        if (!additionalEffects.profile.TryGet(out lensDistortion)) throw new System.NullReferenceException(nameof(lensDistortion));

        setAcidIntensity(acidIntensity);
    }

    // Update is called once per frame
    void Update()
    {
        if (debugActivateAcid) {
            SetOnAcid(true);
            debugActivateAcid = false;
        }
        if (debugDeactivateAcid) {
            SetOnAcid(false);
            debugDeactivateAcid = false;
        }

        if (acidIntensity < targetAcidIntensity) {
            acidIntensity += acidFadeSpeed * Time.deltaTime;
            if (acidIntensity > targetAcidIntensity) {
                acidIntensity = targetAcidIntensity;
            }
        }
        if (acidIntensity > targetAcidIntensity) {
            acidIntensity -= acidFadeSpeed * Time.deltaTime;
            if (acidIntensity < targetAcidIntensity) {
                acidIntensity = targetAcidIntensity;
            }
        }
        setAcidIntensity(acidIntensity);

        handleThrobbing();
    }

    public void SetOnAcid(bool onAcid) {
        if (onAcid) {
            targetAcidIntensity = 1.0f;
        } else {
            targetAcidIntensity = 0.0f;
        }
    }

    private void setAcidIntensity(float intensity) {
        for (int i = 0; i < colorLookups.Count; i++) {
            var lookup = colorLookups[i];
            var originalContribution = originalContributions[i];
            var t = Mathf.Clamp(intensity, 0.0f, 1.0f);
            var contribution = Mathf.Lerp(0.0f, originalContribution, t);
            lookup.contribution.Override(contribution);
        }
        additionalEffects.weight = intensity;
    }

    private void handleThrobbing() {
        var intensity = origLensDistortionIntensity + throbbingIntensity * Mathf.Sin(Time.time * 2 * Mathf.PI * throbbingFrequency);
        lensDistortion.intensity.Override(intensity);
    }
}
