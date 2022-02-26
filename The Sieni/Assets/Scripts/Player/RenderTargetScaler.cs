using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTargetScaler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        scaleToCameraSize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void scaleToCameraSize()
    {
        var cam = Camera.main;
        var ySize = cam.orthographicSize * 2.0f;
        var xSize = cam.pixelWidth / cam.pixelHeight * ySize;
        transform.localScale = new Vector3(xSize, ySize, 1.0f);
    }
}
