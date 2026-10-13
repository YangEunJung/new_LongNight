using UnityEngine;

public class PrismBasicBullet : Bullet
{
    PrismBasicSkillExecutor executor;

    public void InitWater(float damage, PrismBasicSkillExecutor executor)
    {
        this.damage = damage;
        this.per = -100; // 관통 로직 사용 안함
        this.executor = executor;

        // 돌진 시 캐릭터와 같이 움직이므로 Bullet 자체의 velocity는 주지 않습니다.
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("enemy")) // 적이랑 부딪힌 게 아니면
        {
            return;
        }

        if (executor != null)
        {
            executor.hasHitEnemy = true;
        }
    }

}
