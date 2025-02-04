using UnityEngine;
using static Monster;

public class MonsterAnimEvent : MonoBehaviour
{
    private Monster monster;

    Collider[] hitRangeColliders;
    AudioSource audio;


    AudioClip[] basicSounds;              // Idle, Move 소리들
    AudioClip[] attackSounds;             // 공격 소리들
    AudioClip[] dieSounds;                // 사망 소리들

    private void Start()
    {
        monster = GetComponentInParent<Monster>();
        hitRangeColliders = monster.GetHitRangeColliders(); // 공격 판정에 사용될 몬스터 콜라이더 연동
        audio = monster.GetAudioSoruce();
        basicSounds = monster.GetoBasicAudioClip();
        attackSounds = monster.GetAttackAudioClip();
        dieSounds = monster.GetDieAudioClip();
    }

    // Idle 애니메이션 시작 프레임
    public void OnAnimIdleStart()
    {
        if (audio != null && !audio.isPlaying)
            audio.PlayOneShot(basicSounds[Random.Range(0, basicSounds.Length)]);
    }

    // 이동 애니메이션 시작 프레임
    public void OnAnimWalkStart()
    {
        if (audio != null && !audio.isPlaying)
            audio.PlayOneShot(basicSounds[Random.Range(0, basicSounds.Length)]);
    }

    // 공격 애니메이션 시작 프레임
    public void OnAnimAttackStart()
    {
        if (audio != null)
            audio.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);
    }

    // 사망 애니메이션 시작 프레임
    public void OnAnimDieStart()
    {
        if (audio != null)
            audio.PlayOneShot(dieSounds[Random.Range(0, dieSounds.Length)]);
    }

    // 공격 애니메이션중 히트판정용 콜라이더를 켜주는 프레임
    public void OnAnimAttackRangeColliderOn()
    {
        foreach (var hitRangeCollider in hitRangeColliders)
        {
            hitRangeCollider.enabled = true;
        }
    }

    // 공격 애니메이션후 히트판정용 콜라이더를 꺼주는 프레임
    public void OnAnimAttackRangeColliderOff()
    {
        foreach (var hitRangeCollider in hitRangeColliders)
        {
            hitRangeCollider.enabled = false;
        }
    }

    // 공격 애니메이션이 끝나고 다음 행동을 결정하는 애니메이션 프레임
    public void OnAnimAttackEndEvent()
    {
        if (monster != null)
        {
            monster.State = EState.Idle; // 상태 변경
        }
    }

    // 스턴 애니메이션이 끝나고 다음 행동을 결정하는 애니메이션 프레임
    public void OnAnimStunEndEvent()
    {
        if (monster != null && monster.State != EState.Death)
        {
            monster.State = EState.Idle;
        }
    }

    // (Patrol Eye)경고 애니메이션이 끝난 후 상태를 Idle로 변경하는 프레임
    public void OnAnimAlertEndEvent()
    {
        if (monster != null)
        {
            monster.State = EState.Idle;
        }
    }
}
