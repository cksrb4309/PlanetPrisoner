using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class PlayerAudioController : MonoBehaviour
{
    static PlayerAudioController instance = null;

    [SerializeField] SerializedDictionary<AudioName, AudioClip> audioClipDictionary;

    [SerializeField] PlayerGroundChecker groundChecker = null;

    [SerializeField] List<AudioClip> walkAudioClips;
    [SerializeField] List<AudioClip> runAudioClips;

    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;

    [SerializeField] float walkDelay;
    [SerializeField] float runDelay;

    [SerializeField] float walkThreshold;
    [SerializeField] float runThreshold;

    AudioSource audioSource;

    float footstepMeter = 0;
    float currentSpeed = 0f;

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();

        MonsterHearing.AddAudioSource(audioSource);

        GetComponent<PlayerController>().BindToPlayerAudioController(AddMoveValue);
    }
    private void OnDisable()
    {
        MonsterHearing.RemoveAudioSource(audioSource);

        GetComponent<PlayerController>().UnbindFromAudioController(AddMoveValue);
    }
    private void Start()
    {
        StartCoroutine(FootStepAudioCoroutine());
    }
    IEnumerator FootStepAudioCoroutine()
    {
        while (true)
        {
            while (currentSpeed == 0f) yield return null;

            yield return new WaitForSeconds(
                Mathf.Lerp(walkDelay, runDelay,
                Mathf.InverseLerp(walkSpeed, runSpeed, currentSpeed)));

            if (footstepMeter > runThreshold)
            {
                footstepMeter = 0;
                FootstepAudioPlay(isWalk: false);

            }
            else if (footstepMeter > walkThreshold)
            {
                footstepMeter = 0;
                FootstepAudioPlay(isWalk: true);
            }
        }
    }
    void AddMoveValue(float value)
    {
        if (!groundChecker.IsColliderBelow) return;

        footstepMeter += value * Time.deltaTime;

        currentSpeed = value;
    }
    public void PlayAudio(AudioName audioName) =>
        audioSource.PlayOneShot(audioClipDictionary[audioName]);
    public void PlayAudio(int index) => PlayAudio((AudioName)index);
    public static void PlayerAudioPlay(AudioName audioName) =>
        instance.PlayAudio(audioName);
    #region 걷는 소리, 뛰는 소리 재생 함수
    void FootstepAudioPlay(bool isWalk)
    {
        List<AudioClip> playAudioClips = isWalk ? walkAudioClips : runAudioClips;

        audioSource.PlayOneShot(playAudioClips[Random.Range(0, playAudioClips.Count)]);
    }
    #endregion
}

public enum AudioName
{
    PlayerJump,
    PlayerLand,
    PlayerAttackHit,
    PlayerScan,
    FieldBGM,
    CaveBGM,
    DangerBGM,
    PlayerAttackSwing,
    TrapTrigger,
    TrapInstall,
}