using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int per; // 관통력
    public float damage; // 데미지

    public Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per > 0) // 도화의 관통 무기가 아니라면
        {
            rigid.linearVelocity = dir * 15;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("enemy") || per == -100) // 원딜 무기가 아니거나 적이랑 부딪힌 게 아니면
        {
            return;
        }

        per--;

        if (per < 0)
        {
            rigid.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("area") || per == -100) return;

        gameObject.SetActive(false);
    }
}
