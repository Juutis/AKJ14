using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    [SerializeField]
    private List<EffectDuration> durations;

    private int moveShroomEffectCount = 0;
    private int visionShroomEffectCount = 0;

    private int totalEffectCount {
        get {
            return moveShroomEffectCount + visionShroomEffectCount;
        }
    }

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
            moveShroomEffectCount++;
            Invoke("EndMoveShroomEffect", getDurationForType(objectType));
            UIManager.main.RemapButtons();
        }
        if (objectType == MoveObjectType.VisionShroom)
        {
            ShroomEffects.Main.SetOnAcid(true);
            ShroomEffects.Main.SetDizzyCamera(true);
            visionShroomEffectCount++;
            Invoke("EndVisionShroomEffect", getDurationForType(objectType));
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

    public void EndMoveShroomEffect() {
        moveShroomEffectCount--;
        if (moveShroomEffectCount <= 0) {
            RemappableInput.Main.ResetDirections();
            if (totalEffectCount <= 0) {
                ShroomEffects.Main.SetOnAcid(false);
            }
        }
    }

    public void EndVisionShroomEffect() {
        visionShroomEffectCount--;
        if (visionShroomEffectCount <= 0) {
            ShroomEffects.Main.SetDizzyCamera(false);
            if (totalEffectCount <= 0) {
                ShroomEffects.Main.SetOnAcid(false);
            }
        }
    }

    private float getDurationForType(MoveObjectType type) {
        return durations.Find(it => it.Type == type).Duration;
    }
}

[Serializable]
public class EffectDuration
{
    public MoveObjectType Type;
    public float Duration;
}