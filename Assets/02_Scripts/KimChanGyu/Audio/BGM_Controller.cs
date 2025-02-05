using System.Collections;
using UnityEngine;
using VInspector;

public class BGM_Controller : MonoBehaviour
{
    public static BGM_Controller Instance { get; private set; }

    [SerializeField] AudioSource audioSource;

    [SerializeField] SerializedDictionary<AudioName, AudioClip> audioClipDictionary;

    [SerializeField] float pivotX;

    [SerializeField] float dangerBgmDuration;

    bool isDangerBgm = false;

    float dangerValue = 0f;

    Coroutine dangerCoroutine = null;

    AudioName currentBGM = AudioName.PlayerScan;

    private void Awake()
    {
        Instance = this;
    }
    private void FixedUpdate()
    {
        AudioName changeAudioName = currentBGM;

        if (isDangerBgm)
        {
            changeAudioName = AudioName.DangerBGM;
        }
        else
        {
            if (transform.position.x > pivotX)
            {
                changeAudioName = AudioName.FieldBGM;
            }
            else
            {
                changeAudioName = AudioName.CaveBGM;
            }
        }

        if (changeAudioName != currentBGM) ChangeBGM(changeAudioName);
    }
    public void ChangeBGM(AudioName changeAudioName)
    {
        currentBGM = changeAudioName;

        audioSource.Stop();

        audioSource.clip = audioClipDictionary[changeAudioName];

        audioSource.Play();
    }

    public void Danger()
    {
        dangerValue = dangerBgmDuration;

        if (dangerCoroutine == null)
        {
            dangerCoroutine = StartCoroutine(DangerCoroutine());
        }
    }
    IEnumerator DangerCoroutine()
    {
        for (; dangerValue > 0f; dangerValue -= Time.deltaTime) yield return null;

        isDangerBgm = false;

        dangerCoroutine = null;
    }
}