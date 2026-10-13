using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [Header("# BGM")]
    public AudioClip[] bgmClips;
    public AudioSource bgmSources;
    int bgmChannelIndex; // 인덱스
    public float bgmVolume;

    [Header("# Story SFX")]
    public AudioClip[] storySfxClips;
    public AudioSource storySources;
    int storySfxChannelIndex; // 인덱스
    public float storySfxVolume;

    [Header("# Battle SFX")]
    public AudioClip[] sfxClips;
    public AudioSource[] sfxSources;
    public int sfxChannels; // 틀 수 있는 효과음의 개수
    int sfxChannelIndex = 0; // 인덱스
    public float sfxVolume;
    public enum SFX { DowhaBasic, DowhaBasic1, LunaBasic, Hit, Hit1, LevelUp, Dead, Select, Win, Lose};

    private void Awake()
    {
        instance = this;
        Init();
    }
    void Init()
    {
        // BGM
        GameObject bgmObject = new GameObject("BGMPlayer");
        bgmObject.transform.parent = transform;

        //bgmSources = new AudioSource();
        bgmSources = bgmObject.AddComponent<AudioSource>();
        bgmSources.clip = bgmClips[0];
        bgmSources.playOnAwake = false;
        bgmSources.loop = true;
        bgmSources.volume = bgmVolume;

        // Story SFX
        GameObject storySfxObject = new GameObject("StorySFXPlayer");
        storySfxObject.transform.parent = transform;

        //storySources = new AudioSource();
        storySources = bgmObject.AddComponent<AudioSource>();
        storySources.playOnAwake = false;
        storySources.loop = false;
        storySources.volume = storySfxVolume;

        // SFX
        GameObject sfxObject = new GameObject("SFXPlayer");
        sfxObject.transform.parent = transform;
        sfxSources = new AudioSource[sfxChannels];

        for (int i = 0; i < sfxChannels; i++)
        {
            sfxSources[i] = sfxObject.AddComponent<AudioSource>();
            sfxSources[i].playOnAwake = false;
            sfxSources[i].loop = false;
            sfxSources[i].volume = sfxVolume;
        }
    }

    public void PlaySfx(SFX sfx)
    {
        for (int i = 0; i < sfxChannels; i++)
        {
            int loopIndex = (sfxChannelIndex + i) % sfxChannels;

            if (sfxSources[loopIndex].isPlaying) continue;

            // 추가
            int ranIdx = 0;
            if (sfx == SFX.Hit || sfx == SFX.DowhaBasic)
            {
                ranIdx = Random.Range(0, 2);
            }

            sfxChannelIndex = loopIndex;
            sfxSources[loopIndex].clip = sfxClips[(int)sfx + ranIdx];
            sfxSources[loopIndex].Play();
            break;

        }

    }

    public void PlayBgm(bool isPlaying)
    {
        if (!isPlaying)
        {
            bgmSources.Play();
        }
        else
        {
            bgmSources.Stop();
        }
    }
}
