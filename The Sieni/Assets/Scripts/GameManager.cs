using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    [SerializeField]
    private List<EffectDuration> durations;

    [SerializeField]
    private List<ShroomMultiplierIncrement> multipliers;

    [SerializeField]
    public List<ShroomCount> shroomsRequiredForWin;

    public bool StoryMode = true;

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

    public void Update()
    {
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
            // Debug.Log("You hit a tree!");
            WorldMover.main.IsMoving = false;
            SoundManager.main.PlaySound(GameSoundType.Hit);
            GameOver();
        }
        else
        {
            moveObject.Kill();
        }
        if (objectType == MoveObjectType.RegularShroom || objectType == MoveObjectType.Bird)
        {
            //int score = 1;
            int scoreGained = GainScore(1);
            Vector3 offset = new Vector3(0, 1f, 0f);
            UIManager.main.ShowPoppingMessage(PlayerInput.main.transform.position + offset, $"+{scoreGained}");
            SoundManager.main.PlaySound(GameSoundType.PickupRegular);
            AddCollectedShroom(objectType);
        }
        if (objectType == MoveObjectType.MoveShroom)
        {
            ShroomEffects.Main.SetOnAcid(true);
            changeControls();
            moveShroomEffectCount++;
            Invoke("EndMoveShroomEffect", getDurationForType(objectType));
            UIManager.main.RemapButtons();
            SoundManager.main.PlaySound(GameSoundType.PickupMovement);
            MusicPlayer.main.SwitchMusic(false);
            AddCollectedShroom(objectType);
        }
        if (objectType == MoveObjectType.VisionShroom)
        {
            ShroomEffects.Main.SetOnAcid(true);
            ShroomEffects.Main.SetDizzyCamera(true);
            visionShroomEffectCount++;
            Invoke("EndVisionShroomEffect", getDurationForType(objectType));
            SoundManager.main.PlaySound(GameSoundType.PickupVision);
            MusicPlayer.main.SwitchMusic(false);
            AddCollectedShroom(objectType);
        }
        if (objectType == MoveObjectType.DisableControlShroom)
        {
            ShroomEffects.Main.SetOnAcid(true);
            disableControls();
            disableControlsShroomEffectCount++;
            Invoke("EndDisableControlsShroomEffect", getDurationForType(objectType));
            UIManager.main.RemapButtons();
            SoundManager.main.PlaySound(GameSoundType.PickupButton);
            MusicPlayer.main.SwitchMusic(false);
            AddCollectedShroom(objectType);
        }
    }

    public int GainScore(int score)
    {
        // Debug.Log($"Gained {score * scoreMultiplier} score! Now you have {totalScore}!");
        int scoreGained = score * scoreMultiplier;
        totalScore += scoreGained;
        UIManager.main.UpdateScore(scoreMultiplier, totalScore, collectedShrooms);
        return scoreGained;
    }

    public void GainMultiplier(MoveObjectType objectType)
    {
        int multiplierAddition = multipliers.FirstOrDefault(x => x.Type == objectType)?.Multiplier ?? 0;
        scoreMultiplier += multiplierAddition;
        UIManager.main.UpdateScore(scoreMultiplier, totalScore, collectedShrooms);
        if (multiplierAddition != 0)
        {
            Vector3 offset = new Vector3(0, 0.5f, 0f);
            UIManager.main.ShowPoppingMessage(PlayerInput.main.transform.position + offset, $"X{multiplierAddition} multiplier!");
        }
    }

    public void GameOver()
    {
        // Debug.Log("Game over!");
        UIManager.main.ShowGameOver();
    }

    public void EndMoveShroomEffect()
    {
        moveShroomEffectCount--;
        if (moveShroomEffectCount <= 0)
        {
            if (disableControlsShroomEffectCount <= 0)
            {
                RemappableInput.Main.ResetDirections();
            }
            if (totalEffectCount <= 0)
            {
                ShroomEffects.Main.SetOnAcid(false);
                MusicPlayer.main.SwitchMusic(true);
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
                MusicPlayer.main.SwitchMusic(true);
            }
        }
    }

    public void EndDisableControlsShroomEffect()
    {
        disableControlsShroomEffectCount--;
        if (disableControlsShroomEffectCount <= 0)
        {
            RemappableInput.Main.EnableHorizontalControls();
            if (moveShroomEffectCount <= 0)
            {
                RemappableInput.Main.ResetDirections();
            }
            if (totalEffectCount <= 0)
            {
                ShroomEffects.Main.SetOnAcid(false);
                MusicPlayer.main.SwitchMusic(true);
            }
            UIManager.main.RemapButtons();
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
            RemappableInput.Main.InvertRandomAxis();
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

    private void disableControls()
    {
        if (disableControlsShroomsEaten < 5)
        {
            RemappableInput.Main.DisableHorizontalControls();
        }
        else
        {
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
        updateStoryMode();
    }

    public int GetCollectedCount(MoveObjectType type)
    {
        return collectedShrooms.GetValueOrDefault(type, 0);
    }

    private void updateStoryMode()
    {
        if (!StoryMode) return;

        foreach (var winRequirement in shroomsRequiredForWin)
        {
            if (GetCollectedCount(winRequirement.Type) < winRequirement.Count) return;
        }

        UIManager.main.ShowWin();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

[Serializable]
public class ShroomCount
{
    public MoveObjectType Type;
    public int Count;
}
