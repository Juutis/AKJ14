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

    private Dictionary<Direction, MappedDirection> originalDirections = new Dictionary<Direction, MappedDirection> {
        { Direction.RIGHT, DIR_RIGHT },
        { Direction.LEFT, DIR_LEFT },
        { Direction.UP, DIR_UP },
        { Direction.DOWN, DIR_DOWN }
    };

    private Dictionary<Direction, MappedDirection> mappedDirections;

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
        var leftInput = mappedDirections[Direction.LEFT].GetAbsoluteValue();
        var rightInput = mappedDirections[Direction.RIGHT].GetAbsoluteValue();
        return rightInput - leftInput;
    }

    public float GetVertical() {
        var upInput = mappedDirections[Direction.UP].GetAbsoluteValue();
        var downInput = mappedDirections[Direction.DOWN].GetAbsoluteValue();
        return upInput - downInput;
    }

    public void InvertControls() {
        mappedDirections = new Dictionary<Direction, MappedDirection> {
            { Direction.RIGHT, DIR_LEFT },
            { Direction.LEFT, DIR_RIGHT },
            { Direction.UP, DIR_DOWN },
            { Direction.DOWN, DIR_UP }
        };

        debugInputs();
    }

    public void InvertHorizontalControls() {
        mappedDirections = new Dictionary<Direction, MappedDirection> {
            { Direction.RIGHT, DIR_LEFT },
            { Direction.LEFT, DIR_RIGHT },
            { Direction.UP, DIR_UP },
            { Direction.DOWN, DIR_DOWN }
        };
    }

    public void InvertVerticalControls() {
        mappedDirections = new Dictionary<Direction, MappedDirection> {
            { Direction.RIGHT, DIR_RIGHT },
            { Direction.LEFT, DIR_LEFT },
            { Direction.UP, DIR_DOWN },
            { Direction.DOWN, DIR_UP }
        };
    }

    public void RandomizeDirections() {
        var values = originalDirections.Values.ToList().Shuffled();
        mappedDirections = new Dictionary<Direction, MappedDirection> {
            { Direction.RIGHT, values.Pop() },
            { Direction.LEFT, values.Pop() },
            { Direction.UP, values.Pop() },
            { Direction.DOWN, values.Pop() }
        };

        debugInputs();
    }

    public void ResetDirections() {
        mappedDirections = originalDirections;

        debugInputs();
    }

    /// Returns current Input -> Movedirection mappings.
    /// The key is the input (eg. 'up' = 'W') and the value is the direction that key will move the player.
    public Dictionary<Direction, Direction> GetInputMappings() {
        return mappedDirections.ToDictionary(it => it.Value.Direction, it => it.Key);
    }

    private void debugInputs() {
        var inputs = GetInputMappings();
        Debug.Log("Inputs are now:");
        Debug.Log("UP / W: " + inputs[Direction.UP]);
        Debug.Log("DOWN / S: " + inputs[Direction.DOWN]);
        Debug.Log("LEFT / A : " + inputs[Direction.LEFT]);
        Debug.Log("RIGHT / D: " + inputs[Direction.RIGHT]);
    }
}

public struct MappedDirection {

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

public enum Direction {
    LEFT,
    RIGHT,
    UP,
    DOWN
}

