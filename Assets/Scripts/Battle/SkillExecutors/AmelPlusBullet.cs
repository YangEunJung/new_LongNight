using UnityEngine;

public class AmelPlusBullet : Bullet
{
    AmelPlusSkillExecutor executor;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("enemy")) // 적이랑 부딪힌 게 아니면
        {
            return;
        }

        if (executor != null)
        {
            Debug.Log("폭발");
        }
    }

    public void InitCardExplosion(float damage, AmelPlusSkillExecutor executor, bool isGood)
    {
        this.executor = executor;
        this.per = -100; // 관통 로직 사용 안함

        if (isGood)
        {
            this.damage = damage * 1.5f;
        }
        else
        {
            this .damage = damage;
        }
    }
}
