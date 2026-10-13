using System.Collections;
using UnityEngine;

public class DohwaPlusSkillExecutor : SkillExecutor
{
    public bool isParrying;// 패링 모드에 진입했는지
    public float onParryTime; // 패링모드 지속시간, 0.5초
    public bool isCoolDown; // 쿨타임 중인가
    //public float maxCoolTime; // 발동에 필요한 쿨타임
    //public float cooldownTimer; // 남은 쿨타임 타이머

    public float damagePercent; // 증가되는 데미지 계수, %로 저장, 레벨업시 상승하는 값
    public float finalDamage; // 데미지가 증가되는 레벨업 옵션이므로 별도로 만들어줌.

    WaitForFixedUpdate wait;

    public ActiveCharacterController character;
    public DowhaBasicSkillExecutor basicExecutor;

    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);

        prefabID = 15;
        damage = owner.currentPower;
        
        //maxCoolTime = skillState.coolTime; // 임시, 이후 구조 수정에 따라 변경될 수 있음.
        //cooldownTimer = 0;
        onParryTime = 0.8f;

        isCoolDown = false;
        isParrying = false;

        damagePercent = 100;
        finalDamage = damage * (damagePercent * 0.01f);

        character = GetComponentInParent<ActiveCharacterController>();
        basicExecutor = transform.parent.GetComponentInChildren<DowhaBasicSkillExecutor>();

        wait = new WaitForFixedUpdate();
    }

    public override void Activate()
    {
        base.Activate();
        Debug.Log("도화 증강 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("도화 증강 스킬 off");
        base.Deactivate();

        isParrying = false;
        if (character != null)
        {
            character.isDashing = false;
        }

        // 칼 색상 변경
        ChangeColorBlade(Color.white);
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;

        // 쿨타임 타이머
        if (isCoolDown && !isParrying)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                isCoolDown = false;
            }
        }

        if (!isCoolDown && !isParrying && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ParryMode());
        }
    }

    IEnumerator ParryMode()
    {
        isParrying = true;
        character.isDashing = true;

        // 칼 색상 변경
        ChangeColorBlade(new Color(1f, 0.8f, 0.2f));

        float Timer = 0;
        while (Timer <= onParryTime)
        {
            Timer += Time.deltaTime;
            yield return wait;
        }


        // 칼 색상 변경
        ChangeColorBlade(Color.white);

        isParrying = false;
        character.isDashing = false;
        isCoolDown = true;
        timer = maxCoolTime;
    }

    // 패링 성공시 칼날 날림, 스캐너가 호출
    public void OnParrySuccess()
    {
        if (!isParrying) return;
        isParrying = false;

        if (basicExecutor != null)
        {
            foreach (Transform b in basicExecutor.transform)
            {
                // 1. 베이직 칼 하나의 위치를 pos로 받음
                Transform excal = BattleSession.instance.pool.Get(prefabID).transform;
                excal.gameObject.SetActive(true);
                Vector3 pos = b.position;
                excal.position = pos;

                // 2. 해당 위치에서 -> 바깥쪽 직선 방향을 계산함
                Vector3 dir = (pos - character.transform.position).normalized;
                excal.rotation = Quaternion.FromToRotation(Vector3.up, dir) * Quaternion.Euler(0, 0, -45);

                // 3.  Bullet의 Init 붙여줌
                excal.GetComponent<Bullet>().Init(finalDamage, 15, dir);
            }
        }

        ChangeColorBlade(Color.white);
        if (character != null) character.isDashing = false;
    }

    public void ChangeColorBlade(Color color)
    {
        if (basicExecutor == null)
            basicExecutor = character.GetComponentInChildren<DowhaBasicSkillExecutor>();

        if (basicExecutor == null)
        {
            Debug.Log("기본 스킬 실행자 못 찾음.");
            return;
        }

        foreach (Transform b in basicExecutor.transform) {
            if (b.gameObject.activeSelf)
            {
                SpriteRenderer sprite = b.GetComponent<SpriteRenderer>();
                if (sprite != null) sprite.color = color;
            }
        }

    }

    // 데미지 계수 증가
    public override void SkillLevelUp()
    {
        if (base.skillState.originSkillData.levels[base.skillState.level].type != EffectType.Damage)
            return;

        this.damagePercent += base.skillState.originSkillData.levels[base.skillState.level].value;
    }

    public override void ApplyStats()
    {
        damage = owner.currentPower;
        // 추가
        finalDamage = damage * (damagePercent * 0.01f);
    }
}
