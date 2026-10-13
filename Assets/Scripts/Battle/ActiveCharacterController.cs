using System.Collections;
using UnityEngine;

public class ActiveCharacterController : MonoBehaviour
{
    Vector2 inputVec;

    // 드롭 스킬을 위해 추가
    Vector2 lastMoveDirection = Vector2.down;
    public Vector2 lastMoveDir => lastMoveDirection;

    public Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator animator;

    public float moveSpeed;
    public bool isDashing; // 프리즘을 위해 추가

    public CharacterRuntimeState currentState;

    bool isDeathRoutineRunning;
    WaitForSeconds wait;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        wait = new WaitForSeconds(0.8f);
    }
    void Update()
    {
        if (!BattleSession.instance.isLive) return;
        if (currentState != null && currentState.isDead) return;
        if (isDashing) return; //  대쉬 중일 땐 일반 이동 제한

        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        // 드롭 스킬을 위해 추가
        if (inputVec.sqrMagnitude > 0)
        {
            lastMoveDirection = inputVec.normalized;
        }
    }

    void FixedUpdate()
    {
        if (!BattleSession.instance.isLive) return;
        if (currentState != null && currentState.isDead) return;
        if (isDashing) return; //  대쉬 중일 땐 일반 이동 제한

        Vector2 nextVec = inputVec.normalized * Time.fixedDeltaTime * moveSpeed; 
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        if (!BattleSession.instance.isLive) return;
        if (currentState != null && currentState.isDead) return;

        if (inputVec.x != 0)
            sprite.flipX = inputVec.x < 0;

        animator.SetFloat("speed", inputVec.magnitude);
    }

    // 현재 홀드 중인 캐릭터로 상태를 변경하고, 이동 속도 및 애니메이션을 변경시킨다.
    public void ApplyCharacter(CharacterRuntimeState curCharacter)
    {
        currentState = curCharacter;
        moveSpeed = curCharacter.currentSpeed;

        animator.runtimeAnimatorController = curCharacter.originCharacterData.moveAnim;
        animator.Rebind();
        animator.Update(0f);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!BattleSession.instance.isLive) return;

        // 체력 0으로 감소시 죽음
        if (currentState.currentHp <= 0 && !currentState.isDead)
        {
            StartCoroutine(DeadRoutine());
        }
    }

    IEnumerator DeadRoutine()
    {
        isDeathRoutineRunning = true;

        currentState.currentHp = 0;
        currentState.isDead = true;
        animator.SetTrigger("isDead");

        yield return wait;

        BattleSession.instance.ChearacterDead();

        isDeathRoutineRunning = false;
    }

}
