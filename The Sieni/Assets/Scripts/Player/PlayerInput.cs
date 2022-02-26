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

    private Vector2 input;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        var up = Camera.main.transform.up.normalized;
        var right = Camera.main.transform.right.normalized;
        rb.velocity = (input.x * right + input.y * up) * moveSpeed;
        
        Debug.Log(rb.velocity);
    }

    // Update is called once per frame
    void Update()
    {
        var horizontalInput = RemappableInput.Main.GetHorizontal();
        var verticalInput = RemappableInput.Main.GetVertical();
        input = new Vector2(horizontalInput, verticalInput);
        if (input.magnitude >= 1.0f) input.Normalize();

        debugStuff();
    }

    private void debugStuff()
    {
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
