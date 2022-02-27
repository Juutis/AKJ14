using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    [SerializeField]
    private List<EffectDuration> durations;

    [SerializeField]
    private List<ShroomMultiplierIncrement> multipliers;

    private int moveShroomEffectCount = 0;
    private int visionShroomEffectCount = 0;

    private int moveShroomsEaten = 0;

    private int scoreMultiplier = 1;
    private int totalScore = 0;

    private int totalEffectCount
    {
        get
        {
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
        GainMultiplier(objectType); // Gain a multiplier if it's defined

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
            int score = 1;
            GainScore(score);
            Vector3 offset = new Vector3(0, 1f, 0f);
            UIManager.main.ShowPoppingMessage(PlayerInput.main.transform.position + offset, $"+{score}");
        }
        if (objectType == MoveObjectType.MoveShroom)
        {
            ShroomEffects.Main.SetOnAcid(true);
            changeControls();
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
        Debug.Log($"Gained {score * scoreMultiplier} score! Now you have {totalScore}!");
        totalScore += score * scoreMultiplier;
        UIManager.main.UpdateScore(scoreMultiplier, totalScore);
    }

    public void GainMultiplier(MoveObjectType objectType)
    {
        scoreMultiplier += multipliers.FirstOrDefault(x => x.Type == objectType)?.Multiplier ?? 0;
        UIManager.main.UpdateScore(scoreMultiplier, totalScore);
    }

    public void GameOver()
    {
        Debug.Log("Game over!");
    }

    public void EndMoveShroomEffect()
    {
        moveShroomEffectCount--;
        if (moveShroomEffectCount <= 0)
        {
            RemappableInput.Main.ResetDirections();
            if (totalEffectCount <= 0)
            {
                ShroomEffects.Main.SetOnAcid(false);
            }
            UIManager.main.RemapButtons();
        }
    }

    public void EndVisionShroomEffect()
    {
        visionShroomEffectCount--;
        if (visionShroomEffectCount <= 0)
        {
            ShroomEffects.Main.SetDizzyCamera(false);
            if (totalEffectCount <= 0)
            {
                ShroomEffects.Main.SetOnAcid(false);
            }
        }
    }

    private float getDurationForType(MoveObjectType type)
    {
        return durations.Find(it => it.Type == type).Duration;
    }

    private void changeControls()
    {
        if (moveShroomsEaten < 5)
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.50f)
            {
                RemappableInput.Main.InvertHorizontalControls();
            }
            else
            {
                RemappableInput.Main.InvertVerticalControls();
            }
        }
        else if (moveShroomsEaten < 10)
        {
            RemappableInput.Main.InvertControls();
        }
        else
        {
            RemappableInput.Main.RandomizeDirections();
        }
        moveShroomsEaten++;
    }
}

[Serializable]
public class EffectDuration
{
    public MoveObjectType Type;
    public float Duration;
}

[Serializable]
public class ShroomMultiplierIncrement
{
    public MoveObjectType Type;
    public int Multiplier;
}