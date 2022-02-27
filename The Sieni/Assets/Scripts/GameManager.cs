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
    private int disableControlsShroomEffectCount = 0;

    private int moveShroomsEaten = 0;
    private int disableControlsShroomsEaten = 0;

    private int scoreMultiplier = 1;
    private int totalScore = 0;

    private Dictionary<MoveObjectType, int> collectedShrooms;

    private int totalEffectCount
    {
        get
        {
            return moveShroomEffectCount + visionShroomEffectCount + disableControlsShroomEffectCount;
        }
    }

    private void Awake()
    {
        main = this;
        collectedShrooms = new Dictionary<MoveObjectType, int>();
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
            AddCollectedShroom(objectType);
        }
        if (objectType == MoveObjectType.MoveShroom)
        {
            ShroomEffects.Main.SetOnAcid(true);
            changeControls();
            moveShroomEffectCount++;
            Invoke("EndMoveShroomEffect", getDurationForType(objectType));
            UIManager.main.RemapButtons();
            AddCollectedShroom(objectType);
        }
        if (objectType == MoveObjectType.VisionShroom)
        {
            ShroomEffects.Main.SetOnAcid(true);
            ShroomEffects.Main.SetDizzyCamera(true);
            visionShroomEffectCount++;
            Invoke("EndVisionShroomEffect", getDurationForType(objectType));
            AddCollectedShroom(objectType);
        }
        if (objectType == MoveObjectType.DisableControlShroom)
        {
            ShroomEffects.Main.SetOnAcid(true);
            disableControls();
            disableControlsShroomEffectCount++;
            Invoke("EndDisableControlsShroomEffect", getDurationForType(objectType));
            UIManager.main.RemapButtons();
            AddCollectedShroom(objectType);
        }
    }

    public void GainScore(int score)
    {
        Debug.Log($"Gained {score * scoreMultiplier} score! Now you have {totalScore}!");
        totalScore += score * scoreMultiplier;
        UIManager.main.UpdateScore(scoreMultiplier, totalScore, collectedShrooms);
    }

    public void GainMultiplier(MoveObjectType objectType)
    {
        scoreMultiplier += multipliers.FirstOrDefault(x => x.Type == objectType)?.Multiplier ?? 0;
        UIManager.main.UpdateScore(scoreMultiplier, totalScore, collectedShrooms);
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
            if (disableControlsShroomEffectCount <= 0) {
                RemappableInput.Main.ResetDirections();
            }
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

    public void EndDisableControlsShroomEffect() {
        disableControlsShroomEffectCount--;
        if (disableControlsShroomEffectCount <= 0) {
            RemappableInput.Main.EnableHorizontalControls();
            if (moveShroomEffectCount <= 0) {
                RemappableInput.Main.ResetDirections();
                UIManager.main.RemapButtons();
            }
            if (totalEffectCount <= 0) {
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

    private void disableControls() {
        if (disableControlsShroomsEaten < 5) {
            RemappableInput.Main.DisableHorizontalControls();
        } else {
            RemappableInput.Main.DisableHorizontalControls();
            RemappableInput.Main.SwapHorizontalAndVerticalControls();
        }
        disableControlsShroomsEaten++;
    }
    
    private void AddCollectedShroom(MoveObjectType type)
    {
        if (!collectedShrooms.ContainsKey(type))
        {
            collectedShrooms[type] = 1;
        }
        else
        {
            collectedShrooms[type] += 1;
        }
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
