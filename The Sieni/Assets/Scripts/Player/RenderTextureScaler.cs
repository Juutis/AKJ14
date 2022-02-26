using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureScaler : MonoBehaviour
{
    [SerializeField]
    private List<RenderTexture> renderTextures;

    [SerializeField]
    private List<Camera> cameras;

    private int lastWidth = 0;
    private int lastHeight = 0;
    // Start is called before the first frame update
    void Start()
    {
        scaleToCameraSize();
    }

    // Update is called once per frame
    void Update()
    {
        scaleToCameraSize();
    }

    private void scaleToCameraSize()
    {
        if (lastHeight != Camera.main.pixelHeight || lastWidth != Camera.main.pixelWidth) {
            var max = Mathf.Max(Camera.main.pixelWidth, Camera.main.pixelHeight);

            renderTextures.ForEach( it => {
                it.Release();
                it.width = Camera.main.pixelWidth;
                it.height = Camera.main.pixelHeight;
                it.Create();
            });

            cameras.ForEach(it => {
                it.ResetAspect();
            });

            lastWidth = Camera.main.pixelWidth;
            lastHeight = Camera.main.pixelHeight;
        }
    }
}
