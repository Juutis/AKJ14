using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShroomEffects : MonoBehaviour
{
    public static ShroomEffects Main;

    private AcidEffect acid;
    private DizzyCamera dizzyCamera;

    void Awake() {
        Main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        acid = GetComponent<AcidEffect>();
        dizzyCamera = GetComponent<DizzyCamera>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetDizzyCamera(bool active) {
        dizzyCamera.SetDizzy(active);
    }

    public void SetOnAcid(bool onAcid) {
        acid.SetOnAcid(onAcid);
    }
}
