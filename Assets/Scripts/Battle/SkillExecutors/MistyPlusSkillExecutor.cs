using System.Collections;
using UnityEngine;

public class MistyPlusSkillExecutor : SkillExecutor
{
    public ActiveCharacterController character;

    public bool isCooldown; // 쿨타임 중인가
    //public float maxCoolTime; // 남은 쿨타임 최대치, (이후 아멜 백에 의해 변동될 수 있음)
    //public float cooldownTimer; // 남은 쿨타임 타이머

    public bool onDdang; // 스킬을 사용하고 있는가
    public float ddangTime; // 땡땡이 상태 지속시간, 레벨업시 상승하는 값

    WaitForFixedUpdate wait;
    
    public float dashSpeed; //빨라지는 이동 속도 값, 인스펙터로 넣음, 약 2f?
    //public Vector2 dashVec; // 이동 방향

    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);

        prefabID = 13;
        damage = owner.currentPower;
        //maxCoolTime = skillState.coolTime;

        isCooldown = false;
        onDdang = false;

        character = GetComponentInParent<ActiveCharacterController>();

        wait = new WaitForFixedUpdate() ;
    }

    public override void Activate()
    {
        base.Activate();
        Debug.Log("미스티 증강 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("미스티 증강 스킬 off");
        base.Deactivate();

        // 마찬가지로 레벨업 등으로 강제로 꺼질 때 청소하는 역할
        onDdang = false;

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
        if (!BattleSession.instance.isLive) return;

        //if (onDdang) return;

        if (!onDdang && isCooldown)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isCooldown = false;
                timer = 0;
            }
        }

        if (!onDdang && !isCooldown && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DdangDdangE());
        }
    }

    IEnumerator DdangDdangE()
    {
        onDdang = true;
        character.isDashing = true;

        // Bullet Component Get으로 구해서 Init 함수 호출함.
        Transform shiled;

        if (transform.childCount > 0)
        {
            shiled = transform.GetChild(0);
        }
        else
        {
            shiled = BattleSession.instance.pool.Get(prefabID).transform;
            shiled.parent = transform;
        }

        shiled.gameObject.SetActive(true);
        shiled.localPosition = Vector3.zero;

        shiled.GetComponent<MistyPlusBullet>().InitShiled(damage, this);


        float Timer = 0;
        while (Timer <= ddangTime)
        {
            Timer += Time.fixedDeltaTime;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 dir = new Vector2(h, v).normalized;

            character.rigid.MovePosition(character.rigid.position + dir * Time.fixedDeltaTime * character.moveSpeed * dashSpeed);

            yield return wait;
        }

        //yield return wait;

        // 끝난 뒤
        onDdang = false;
        isCooldown = true;
        timer = maxCoolTime;
        character.isDashing = false;

        // 돌진이 끝나면 Water 자식 오브젝트(이펙트+충돌체) 비활성화
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // 지속시간 증가
    public override void SkillLevelUp()
    {
        if (base.skillState.originSkillData.levels[base.skillState.level].type != EffectType.skillRunTime)
            return;

        this.ddangTime += base.skillState.originSkillData.levels[base.skillState.level].value;
    }

    public override void ApplyStats()
    {
        damage = owner.currentPower;
    }
}
