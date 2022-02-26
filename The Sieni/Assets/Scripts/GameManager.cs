using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    private void Awake()
    {
        main = this;
    }

    public void CollectWorldObject(WorldMoveObject moveObject)
    {
        MoveObjectType objectType = moveObject.ObjectType;
        PlayerParticles.main.PlayParticles(objectType); // plays particle effects if type has one defined

        if (objectType == MoveObjectType.Tree)
        {
            PlayerInput playerInput = PlayerInput.main;
            playerInput.IsEnabled = false;
            playerInput.Stop();
            Debug.Log("You hit a tree!");
            WorldMover.main.IsMoving = false;
            GameOver();
        }
        else
        {
            moveObject.Kill();
        }
        if (objectType == MoveObjectType.RegularShroom)
        {
            GainScore(1);
        }
        if (objectType == MoveObjectType.MoveShroom)
        {
            ShroomEffects.Main.SetOnAcid(true);
            RemappableInput.Main.RandomizeDirections();
        }
        if (objectType == MoveObjectType.VisionShroom)
        {
            ShroomEffects.Main.SetOnAcid(true);
            ShroomEffects.Main.SetDizzyCamera(true);
        }
    }

    public void GainScore(int score)
    {
        Debug.Log($"Gained {score} score!");
    }

    public void GameOver()
    {
        Debug.Log("Game over!");
    }
}
