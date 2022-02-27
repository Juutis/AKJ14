using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RemappableInput : MonoBehaviour
{
    public static RemappableInput Main { get; private set; }

    private static string UP = "Up", DOWN = "Down", RIGHT = "Right", LEFT = "Left";

    private static MappedDirection
        DIR_UP = new MappedDirection(Direction.UP, UP),
        DIR_DOWN = new MappedDirection(Direction.DOWN, DOWN),
        DIR_RIGHT = new MappedDirection(Direction.RIGHT, RIGHT),
        DIR_LEFT = new MappedDirection(Direction.LEFT, LEFT);

    private static InputDirection
        INPUT_UP = new InputDirection(Direction.UP),
        INPUT_DOWN = new InputDirection(Direction.DOWN),
        INPUT_RIGHT = new InputDirection(Direction.RIGHT),
        INPUT_LEFT = new InputDirection(Direction.LEFT);

    private Dictionary<InputDirection, MappedDirection> originalDirections = new Dictionary<InputDirection, MappedDirection> {
        { INPUT_RIGHT, DIR_RIGHT },
        { INPUT_LEFT, DIR_LEFT },
        { INPUT_UP, DIR_UP },
        { INPUT_DOWN, DIR_DOWN }
    };

    private Dictionary<InputDirection, MappedDirection> mappedDirections;

    void Awake() {
        Main = this;
        mappedDirections = originalDirections;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public float GetHorizontal() {
        var leftInput = INPUT_LEFT.Enabled ? mappedDirections[INPUT_LEFT].GetAbsoluteValue() : 0.0f;
        var rightInput = INPUT_RIGHT.Enabled ? mappedDirections[INPUT_RIGHT].GetAbsoluteValue() : 0.0f;
        return rightInput - leftInput;
    }

    public float GetVertical() {
        var upInput = INPUT_UP.Enabled ? mappedDirections[INPUT_UP].GetAbsoluteValue() : 0.0f;
        var downInput = INPUT_DOWN.Enabled ? mappedDirections[INPUT_DOWN].GetAbsoluteValue() : 0.0f;
        return upInput - downInput;
    }

    public void InvertControls() {
        mappedDirections = new Dictionary<InputDirection, MappedDirection> {
            { INPUT_RIGHT, DIR_LEFT },
            { INPUT_LEFT, DIR_RIGHT },
            { INPUT_UP, DIR_DOWN },
            { INPUT_DOWN, DIR_UP }
        };

        debugInputs();
    }

    public void InvertHorizontalControls() {
        mappedDirections = new Dictionary<InputDirection, MappedDirection> {
            { INPUT_RIGHT, DIR_LEFT },
            { INPUT_LEFT, DIR_RIGHT },
            { INPUT_UP, DIR_UP },
            { INPUT_DOWN, DIR_DOWN }
        };

        debugInputs();
    }

    public void InvertVerticalControls() {
        mappedDirections = new Dictionary<InputDirection, MappedDirection> {
            { INPUT_RIGHT, DIR_RIGHT },
            { INPUT_LEFT, DIR_LEFT },
            { INPUT_UP, DIR_DOWN },
            { INPUT_DOWN, DIR_UP }
        };

        debugInputs();
    }

    public void RandomizeDirections() {
        var values = originalDirections.Values.ToList().Shuffled();
        mappedDirections = new Dictionary<InputDirection, MappedDirection> {
            { INPUT_RIGHT, values.Pop() },
            { INPUT_LEFT, values.Pop() },
            { INPUT_UP, values.Pop() },
            { INPUT_DOWN, values.Pop() }
        };

        debugInputs();
    }

    public void SwapHorizontalAndVerticalControls() {
        var leftAndRight = new List<MappedDirection> { DIR_RIGHT, DIR_LEFT }.Shuffled();
        var upAndDown = new List<MappedDirection> { DIR_DOWN, DIR_UP }.Shuffled();
        mappedDirections = new Dictionary<InputDirection, MappedDirection> {
            { INPUT_RIGHT, upAndDown.Pop() },
            { INPUT_LEFT, upAndDown.Pop() },
            { INPUT_UP, leftAndRight.Pop() },
            { INPUT_DOWN, leftAndRight.Pop() }
        };

        debugInputs();
    }

    public void DisableHorizontalControls() {
        INPUT_LEFT.Enabled = false;
        INPUT_RIGHT.Enabled = false;
    }

    public void EnableHorizontalControls() {
        INPUT_LEFT.Enabled = true;
        INPUT_RIGHT.Enabled = true;
    }

    public void ResetDirections() {
        mappedDirections = originalDirections;

        debugInputs();
    }

    /// Returns current Input -> Movedirection mappings.
    /// The key is the input (eg. 'up' = 'W') and the value is the direction that key will move the player.
    public Dictionary<Direction, InputDirection> GetInputMappings() {
        return mappedDirections.ToDictionary(it => it.Value.Direction, it => it.Key);
    }

    private void debugInputs() {
        var inputs = GetInputMappings();
        Debug.Log("Inputs are now:");
        Debug.Log("UP / W: " + inputs[Direction.UP].Direction);
        Debug.Log("DOWN / S: " + inputs[Direction.DOWN].Direction);
        Debug.Log("LEFT / A : " + inputs[Direction.LEFT].Direction);
        Debug.Log("RIGHT / D: " + inputs[Direction.RIGHT].Direction);
    }
}

public class MappedDirection {

    public Direction Direction;
    string axis;

    public MappedDirection(Direction direction, string axis) {
        this.Direction = direction;
        this.axis = axis;
    }

    public float GetAbsoluteValue() {
        return Input.GetAxis(axis);
    }
}

public class InputDirection {
    
    public Direction Direction;
    public bool Enabled;

    public InputDirection(Direction direction) {
        this.Direction = direction;
        Enabled = true;
    }
}

public enum Direction {
    LEFT,
    RIGHT,
    UP,
    DOWN
}

