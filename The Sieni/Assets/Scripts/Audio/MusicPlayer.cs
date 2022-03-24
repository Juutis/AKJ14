using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer main;


    private AudioSource menuMusic;
    private AudioSource normalMusic;
    private AudioSource acidMusic;

    [SerializeField]
    private AudioClip normalMusicClip;
    [SerializeField]
    private AudioClip acidMusicClip;

    [SerializeField]
    private float acidPitch = 0.8f;

    private float originalAcidPitch = 1f;

    private List<AudioFade> fades = new List<AudioFade>();

    [SerializeField]
    float musicVolumeNormal = 0.5f;
    [SerializeField]
    float musicVolumeAcid = 0.5f;
    [SerializeField]
    float crossfadeDurationOut = 2.5f;
    [SerializeField]
    float crossfadeDurationIn = 2.5f;

    private bool isCurrentlyNormal = true;

    [SerializeField]
    private AudioSource audioSourcePrefab;

    [SerializeField]
    private bool isMainMenu = false;

    private void Awake()
    {
        if (main != null)
        {
            Destroy(this);
            main.ResetEffects();
            return;
        }
        main = this;
        DontDestroyOnLoad(this);
    }

    public void Kill()
    {
        Debug.Log("ill");
        main = null;
        Destroy(gameObject);
    }

    private void Start()
    {
        Debug.Log("init audios");
        InitializeAudioSources();
        StartMusic(true);
    }

    public void StartMusic(bool isNormal)
    {
        if (!isMainMenu)
        {
            if (isNormal)
            {
                normalMusic.volume = musicVolumeNormal;
            }
            else
            {
                acidMusic.volume = musicVolumeAcid;
            }
            normalMusic.Play();
            acidMusic.Play();
        }
        else
        {
            menuMusic.volume = musicVolumeNormal;
            menuMusic.Play();
        }
    }

    public void SwitchMusic(bool isNormal)
    {
        if (isNormal == isCurrentlyNormal)
        {
            return;
        }
        isCurrentlyNormal = isNormal;
        string normalOrAcid = isNormal ? "normal" : "acid";
        Debug.Log($"Switching to {normalOrAcid}");
        if (isNormal)
        {
            CrossFade(acidMusic, normalMusic, crossfadeDurationOut, crossfadeDurationIn, musicVolumeNormal, 1.0f);
        }
        else
        {
            CrossFade(normalMusic, acidMusic, crossfadeDurationOut, crossfadeDurationIn, musicVolumeAcid, acidPitch);
        }
    }


    private void InitializeAudioSources()
    {
        if (normalMusic == null)
        {
            normalMusic = InitializeAudioSource("Normal music", normalMusicClip);
        }
        if (acidMusic == null)
        {
            acidMusic = InitializeAudioSource("Acid music", acidMusicClip);
            originalAcidPitch = acidMusic.pitch;
        }
    }

    private AudioSource InitializeAudioSource(string name, AudioClip clip)
    {
        AudioSource source = Instantiate(audioSourcePrefab);
        source.clip = clip;
        source.volume = 0;
        source.transform.SetParent(transform);
        source.transform.position = Vector2.zero;
        source.loop = true;
        source.name = name;
        return source;
    }

    public void Fade(AudioSource fadeSource, float targetVolume, float duration = 0.5f, float targetPitch = 1.0f)
    {
        AudioFade fade = new AudioFade(duration, targetVolume, fadeSource, targetPitch);
        fades.Add(fade);
    }

    public void FadeOutMenuMusic(float duration = 0.5f)
    {
        Fade(menuMusic, 0, duration);
    }

    public void CrossFade(AudioSource fadeOutSource, AudioSource fadeInSource, float durationOut, float durationIn, float volume, float targetPitch)
    {
        AudioFade fadeOut = new AudioFade(durationOut, 0f, fadeOutSource, targetPitch);
        AudioFade fadeIn = new AudioFade(durationIn, volume, fadeInSource, targetPitch);
        fades.Add(fadeOut);
        fades.Add(fadeIn);
    }

    public void Update()
    {
        for (int index = 0; index < fades.Count; index += 1)
        {
            AudioFade fade = fades[index];
            if (fade != null && fade.IsFading)
            {
                fade.Update();
            }
            if (!fade.IsFading)
            {
                fades.Remove(fade);
            }
        }
    }

    public void ResetEffects()
    {
        fades.Clear();
        isCurrentlyNormal = true;
        normalMusic.volume = musicVolumeNormal;
        acidMusic.volume = 0;
        acidMusic.pitch = originalAcidPitch;
    }
}

public class AudioFade
{
    public AudioFade(float duration, float target, AudioSource track, float targetPitch)
    {
        this.duration = duration;
        IsFading = true;
        timer = 0f;
        originalVolume = track.volume;
        targetVolume = target;
        audioSource = track;

        originalPitch = track.pitch;
        this.targetPitch = targetPitch;
    }
    public bool IsFading { get; private set; }
    private float duration;
    private float timer;
    private float targetVolume;
    private AudioSource audioSource;
    private float originalVolume;

    private float originalPitch, targetPitch;

    public void Update()
    {
        timer += Time.unscaledDeltaTime / duration;
        audioSource.volume = Mathf.Lerp(originalVolume, targetVolume, timer);
        audioSource.pitch = Mathf.Lerp(originalPitch, targetPitch, timer);
        if (timer >= 1)
        {
            audioSource.volume = targetVolume;
            audioSource.pitch = targetPitch;
            IsFading = false;
        }
    }
}