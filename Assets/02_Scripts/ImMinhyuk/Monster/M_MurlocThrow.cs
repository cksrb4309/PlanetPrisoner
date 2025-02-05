using UnityEngine;

public class M_MurlocThrow : M_Murloc
{
    [SerializeField] GameObject projectile;
    protected override void UpdateAttack()
    {
        // DO NOT
    }

    public void ThrowWeapon()
    {
        // 투사체 생성
        GameObject thrownProjectile = Instantiate(projectile, transform.position, Quaternion.identity);

        // 투사체에게 원거리 멀록의 공격력을 부여한다.
        thrownProjectile.GetComponent<MurlocSpear>().SetAttackPower(Stat.attackPower);

        // 방향 설정
        Vector3 direction = (target.transform.position + new Vector3(0,0.5f,0) - transform.position).normalized;
        Vector3 offset = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));
        direction = (direction+ offset).normalized; // 오차를 줌

        // 회전
        thrownProjectile.transform.rotation = Quaternion.LookRotation(direction);

        // rigidbody로 힘 부여
        thrownProjectile.GetComponent<Rigidbody>().linearVelocity = (direction * 15);
    }

    // Json에서 스텟을 가져온다.
    protected override M_Stat SetStat()
    {
        M_Stat _stat;

        if (!MonsterStat.Instance.StatDict.TryGetValue("MurlocThrow", out _stat))
        {
            Debug.LogWarning($"MurlocThrow not found in stat Dictionary");
        }

        return _stat;
    }
}
