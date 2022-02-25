using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10.0f;

    [SerializeField]
    private bool triggerInputRemappingForDebugPurposes = false;

    [SerializeField]
    private bool triggerInputResetForDebugPurposes = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var horizontalInput = RemappableInput.Main.GetHorizontal();
        var verticalInput = RemappableInput.Main.GetVertical();
        var input = new Vector2(horizontalInput, verticalInput);
        if (input.magnitude >= 1.0f) input.Normalize();

        transform.position += (Vector3)input * Time.deltaTime * moveSpeed;

        debugStuff();
    }

    private void debugStuff() {
        if (triggerInputResetForDebugPurposes)
        {
            triggerInputResetForDebugPurposes = false;
            RemappableInput.Main.ResetDirections();
        }
        if (triggerInputRemappingForDebugPurposes)
        {
            triggerInputRemappingForDebugPurposes = false;
            RemappableInput.Main.RandomizeDirections();
        }
    }
}
