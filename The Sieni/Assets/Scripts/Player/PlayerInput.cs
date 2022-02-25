using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private bool TriggerInputRemappingForDebugPurposes = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TriggerInputRemappingForDebugPurposes)
        {
            TriggerInputRemappingForDebugPurposes = false;
            RemappableInput.Main.RandomizeDirections();
        }

        var horizontalInput = RemappableInput.Main.GetHorizontal();
        var verticalInput = RemappableInput.Main.GetVertical();
        var input = new Vector2(horizontalInput, verticalInput);
        if (input.magnitude >= 1.0f) input.Normalize();

        transform.position += (Vector3)input * Time.deltaTime;
    }
}
