using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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
        main = this;
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
            CrossFade(acidMusic, normalMusic, crossfadeDurationOut, crossfadeDurationIn, musicVolumeNormal);
        }
        else
        {
            CrossFade(normalMusic, acidMusic, crossfadeDurationOut, crossfadeDurationIn, musicVolumeAcid);
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

    public void Fade(AudioSource fadeSource, float targetVolume, float duration = 0.5f)
    {
        AudioFade fade = new AudioFade(duration, targetVolume, fadeSource);
        fades.Add(fade);
    }

    public void FadeOutMenuMusic(float duration = 0.5f)
    {
        Fade(menuMusic, 0, duration);
    }

    public void CrossFade(AudioSource fadeOutSource, AudioSource fadeInSource, float durationOut, float durationIn, float volume)
    {
        AudioFade fadeOut = new AudioFade(durationOut, 0f, fadeOutSource);
        AudioFade fadeIn = new AudioFade(durationIn, volume, fadeInSource);
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
}

public class AudioFade
{
    public AudioFade(float duration, float target, AudioSource track)
    {
        this.duration = duration;
        IsFading = true;
        timer = 0f;
        originalVolume = track.volume;
        targetVolume = target;
        audioSource = track;
    }
    public bool IsFading { get; private set; }
    private float duration;
    private float timer;
    private float targetVolume;
    private AudioSource audioSource;
    private float originalVolume;

    public void Update()
    {
        timer += Time.unscaledDeltaTime / duration;
        audioSource.volume = Mathf.Lerp(originalVolume, targetVolume, timer);
        if (timer >= 1)
        {
            audioSource.volume = targetVolume;
            IsFading = false;
        }
    }
}