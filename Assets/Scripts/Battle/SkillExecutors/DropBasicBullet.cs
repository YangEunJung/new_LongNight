using UnityEngine;

public class DropBasicBullet : Bullet
{
    DropBasicSkillExecutor executor;

    public void InitBall(float damage, DropBasicSkillExecutor executor, Vector3 dir)
    {
        this.executor = executor;
        this.damage = damage;
        this.per = -100; // 관통 설정 (프리즘과 맞춤)

        if (rigid != null)
            rigid.linearVelocity = dir * executor.ballSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("enemy")) // 적이랑 부딪힌 게 아니면
        {
            return;
        }

        //if (executor != null && !executor.hasHitEnemy) // 중복 타격 방지 로직 추가
        //{
        //    Debug.Log("적 타격");
        //    //rigid.linearVelocity = Vector3.zero;
        //    //executor.hasHitEnemy = true;
        //}

    }
}
