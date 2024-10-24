using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;


    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;     //생성할 AudioSource의 수
    AudioSource[] sfxPlayers;
    //마지막으로 플레이한 오디오 채널의 index
    int channelIndex;

    public enum Sfx { Dead, Hit, Shoot, Reload, Jump}

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;


        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }


    public void PlaySfx(AudioClip sfx)
    {
        int clipIndex = 0;

        for (int i = 0; i < sfxClips.Length; i++)
        {
            if (sfxClips[i] == sfx)
            {
                clipIndex = i;
            }
        }

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;       //아래를 실행하지 않고 다음 index로 넘어갑니다. 


            channelIndex = loopIndex;       //channelindex는 실행한 채널의 넘버
            sfxPlayers[loopIndex].clip = sfxClips[clipIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }

    }
}
