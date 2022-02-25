using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RemappableInput : MonoBehaviour
{
    public static RemappableInput Main { get; private set; }

    private static string UP = "Up", DOWN = "Down", RIGHT = "Right", LEFT = "Left";

    private Dictionary<Direction, MappedDirection> directions = new Dictionary<Direction, MappedDirection> {
        { Direction.RIGHT, new MappedDirection(Direction.RIGHT, RIGHT) },
        { Direction.LEFT, new MappedDirection(Direction.LEFT, LEFT) },
        { Direction.UP, new MappedDirection(Direction.UP, UP) },
        { Direction.DOWN, new MappedDirection(Direction.DOWN, DOWN) }
    };

    void Awake() {
        Main = this;
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
        var leftInput = directions[Direction.LEFT].GetAbsoluteValue();
        var rightInput = directions[Direction.RIGHT].GetAbsoluteValue();
        return rightInput - leftInput;
    }

    public float GetVertical() {
        var upInput = directions[Direction.UP].GetAbsoluteValue();
        var downInput = directions[Direction.DOWN].GetAbsoluteValue();
        return upInput - downInput;
    }

    public void RandomizeDirections() {
        var values = directions.Values.ToList().Shuffled();
        directions = new Dictionary<Direction, MappedDirection> {
            { Direction.RIGHT, values.Pop() },
            { Direction.LEFT, values.Pop() },
            { Direction.UP, values.Pop() },
            { Direction.DOWN, values.Pop() }
        };

        Debug.Log("Inputs are now:");
        Debug.Log("UP: " + directions[Direction.UP].Direction);
        Debug.Log("DOWN: " + directions[Direction.DOWN].Direction);
        Debug.Log("LEFT: " + directions[Direction.LEFT].Direction);
        Debug.Log("RIGHT: " + directions[Direction.RIGHT].Direction);
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

