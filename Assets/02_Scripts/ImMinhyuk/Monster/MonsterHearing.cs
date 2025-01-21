using System.Collections.Generic;
using UnityEngine;

public class MonsterHearing : MonoBehaviour
{
    // 이 스크립트를 갖고 있는 몬스터는 누구인가?
    Monster monster;

    // 몬스터가 들을 수 있는 모든 오디오 소스
    public static List<AudioSource> audioSources = new List<AudioSource>();

    // 최초 오디오 소스 집계는 한번만 한다.
    private static bool isInitialized = false;

    public MonsterHearing(Monster _monster)
    {
        monster = _monster;       

        // 이 코드는 첫 번째 몬스터가 처음 시작할 때만 실행됨
        if (!isInitialized)
        {
            InitializeAudioSources();
            isInitialized = true;  // 초기화가 완료되었으므로 이후 몬스터에서는 실행되지 않음
        }
    }

    // AudioSource를 관리하는 함수
    private static void InitializeAudioSources()
    {
        // 모든 AudioSource 긁어오기
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in allAudioSources)
        {
            audioSources.Add(audioSource);
        }
    }

    // 재생중인 AudioSource List 반환  
    public List<AudioSource> IsPlayingList()
    {
        List<AudioSource> playingSources = new List<AudioSource>();
        foreach (var source in audioSources)
        {
            if (source.isPlaying) // 재생 중인지 확인
            {
                playingSources.Add(source);
            }
        }
        return playingSources;
    }

    // AudioSource 객체를 갖고 있는 오브젝트가 생기면 이 함수를 호출해줘야 한다.
    public static void AddAudioSource(AudioSource audioSource)
    {
        if (!audioSources.Contains(audioSource))
        {
            audioSources.Add(audioSource);
        }
    }

    // AudioSource 객체를 갖고 있는 오브젝트가 제거되면 이 함수를 호출해줘야 한다.
    public static void RemoveAudioSource(AudioSource audioSource)
    {
        if (audioSources.Contains(audioSource))
        {
            audioSources.Remove(audioSource);
        }
    }

    // 모든 AudioSource를 밀어줌
    public static void RemoveAllAudioSource()
    {
        audioSources.Clear();
    }
}
