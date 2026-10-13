using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Collider2D col;
    public Rigidbody2D target; // 플레이어, 인스펙터로 넣어줌

    public float speed;
    public bool isLive;
    public float health;
    public float maxHealth;

    public float currentSpeed; // 추가

    public int monsterID;

    public RuntimeAnimatorController[] animController; // 인스펙터로 적 애니메이션 컨트롤러 삽입
    Animator animator;
    WaitForSeconds wait;
    bool isHit;

    void OnEnable()
    {
        isLive = true;
        target = BattleSession.instance.activeCharacterController.GetComponent<Rigidbody2D>();

        col.enabled = true;
        rigid.simulated = true;
        spriteRenderer.sortingOrder = 2;
        animator.SetBool("Dead", false);
    }
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        wait = new WaitForSeconds(0.3f);
        
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") || isHit) return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * currentSpeed * Time.fixedDeltaTime; // 수정
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!isLive) return;

        spriteRenderer.flipX = rigid.position.x > target.position.x;
    }

    
    public void Init(SpawnData data)
    {
        speed = data.speed;
        maxHealth = data.health;
        health = maxHealth;
        animator.runtimeAnimatorController = animController[data.monsterID];

        monsterID = data.monsterID;

        currentSpeed = speed; // 추가
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag != "bullet" || !isLive)
        {
            return;
        }

        health -= collision.GetComponent<Bullet>().damage;
        isHit = true;
        StartCoroutine("KnockBack");// 넉백

        if (health > 0)
        {
            isLive = true;
            animator.SetTrigger("Hit");

            // 사운드
            AudioManager.instance.PlaySfx(AudioManager.SFX.Hit); // temp
        }
        else // 사망
        {
            isLive = false;
            BattleSession.instance.OnEnemyKilled(this);

            // 사운드
            AudioManager.instance.PlaySfx(AudioManager.SFX.Dead); // temp

            col.enabled = false;
            rigid.simulated = false;
            spriteRenderer.sortingOrder = 1;
            animator.SetBool("Dead", true);
            //gameObject.SetActive(false); 애니메이터가 실행하도록 지움
        }

    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (!BattleSession.instance.isLive) return;
        
        switch (monsterID)
        {
            case 0:
                BattleSession.instance.activeCharacterController.currentState.currentHp -= 1 * Time.deltaTime;
                break;
            case 1:
                BattleSession.instance.activeCharacterController.currentState.currentHp -= 2 * Time.deltaTime;
                break;
        }

        float hp = target.GetComponent<ActiveCharacterController>().currentState.currentHp;
        //Debug.Log($"현재 캐릭터의 체력: {hp}");
    }

    IEnumerator KnockBack()
    {
        yield return null;

        Vector3 playerPos = target.position;
        Vector3 myPos = transform.position;
        Vector3 dir = myPos - playerPos;
        rigid.AddForce(dir.normalized * 5, ForceMode2D.Impulse);

        yield return wait;
        isHit = false;
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    public void SetCurrentSpeed(bool isSlow)
    {
        if (isSlow)
        {
            currentSpeed = speed * 0.01f;
        }

        else
        {
            currentSpeed = speed;
        }
    }
}
