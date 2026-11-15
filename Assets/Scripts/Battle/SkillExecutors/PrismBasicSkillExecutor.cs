using System.Collections;
using UnityEngine;

public class PrismBasicSkillExecutor : SkillExecutor
{
    public ActiveCharacterController character;
    
    //bool isDashing; // 돌진 중인가, character에서 다루게 함
    public bool isCooldown; // 쿨타임 중인가
    //public float cooldownTimer; // 남은 쿨타임 타이머
    //public float maxCoolTime; // 남은 쿨타임 최대치, (이후 아멜 백에 의해 변동될 수 있음)
    public bool hasHitEnemy; // 이번 돌진에서 적을 처치했는가?

    public float dashSpeed; // 돌진 스피드, 인스펙터 창에서 넣는다.
    public float dashTime = 2f; // 최대 돌진 시간
    public Vector2 dashVec; // 현재 돌진 방향

    WaitForFixedUpdate wait;
    WaitForSeconds waitSeconds;

    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);

        prefabID = 3;
        damage = 10; // 몬스터 원콤을 위해
        //maxCoolTime = skillState.coolTime; // 임시, 이후 구조 수정에 따라 변경될 수 있음.
        timer = 0;

        isCooldown = false;
        hasHitEnemy = false;

        wait = new WaitForFixedUpdate();
        waitSeconds = new WaitForSeconds(0.4f);
    }

    public override void Activate()
    {
        base.Activate();
        Debug.Log("프리즘 기본 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("프리즘 기본 스킬 off");
        base.Deactivate();

        hasHitEnemy = false;
        if (character != null)
        {
            character.isDashing = false;
        }

        // 켜져 있는 쉴드(자식 오브젝트) 강제로 끄기
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (character.isDashing) return;

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isCooldown = false;
                timer = 0;
            }
        }
        

        dashVec.x = Input.GetAxisRaw("Horizontal");
        dashVec.y = Input.GetAxisRaw("Vertical");

        if (!isCooldown && Input.GetKeyDown(KeyCode.Space))
        {
            if (dashVec != Vector2.zero) // 방향키를 누른 상태일 때만 돌진
            {
                StartCoroutine(Dash());
            }
        }
    }

    IEnumerator Dash()
    {
        hasHitEnemy = false; // 새로운 돌진을 시작할 때마다 아직 적 처치 하지 않음을 초기화
        character.isDashing = true;

        // Bullet Component Get으로 구해서 Init 함수 호출함.
        Transform water;

        if (transform.childCount > 0)
        {
            water = transform.GetChild(0);
        }
        else
        {
            water = BattleSession.instance.pool.Get(prefabID).transform;
            water.parent = transform;
        }

        water.gameObject.SetActive(true);
        water.localPosition = Vector3.zero;

        // 회전을 위해 추가
        water.rotation = Quaternion.FromToRotation(Vector3.up, dashVec.normalized * -1);
        water.GetComponent<PrismBasicBullet>().InitWater(damage, this);

        float Timer = 0;
        while (Timer < dashTime && !hasHitEnemy)
        {
            Timer += Time.fixedDeltaTime;

            Vector2 moveVec = dashVec.normalized * Time.deltaTime * dashSpeed;
            character.rigid.MovePosition(character.rigid.position + moveVec);

            yield return wait;
        }

        // 애니메이션이 너무 빨리 꺼져서 추가
        if (hasHitEnemy)
        {
            yield return waitSeconds;
        }

        EndDash();
        
    }

    public void EndDash()
    {
        character.isDashing = false;
        dashVec = Vector2.zero;

        // 돌진이 끝나면 Water 자식 오브젝트(이펙트+충돌체) 비활성화
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        if (hasHitEnemy)
        {
            isCooldown = false; // 쿨타임
            timer = 0; // 남은 쿨타임
        }
        else
        {
            isCooldown = true; // 쿨타임
            timer = maxCoolTime; // 남은 쿨타임
        }
    }
}
