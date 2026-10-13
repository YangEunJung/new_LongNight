using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DropPlusSkillExecutor : SkillExecutor
{
    public ActiveCharacterController character;

    public bool isCooldown; // 쿨타임 중인가
    //public float maxCoolTime; // 남은 쿨타임 최대치, (이후 아멜 백에 의해 변동될 수 있음), 레벨업시 변동되는 값.
    //public float cooldownTimer; // 남은 쿨타임 타이머

    public bool onSkill; // 스킬을 사용하고 있는가
    public float skillSpeed; // 휘두르는 속도
    public float swingTime; // 휘두르는 시간

    WaitForFixedUpdate wait;
    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);

        prefabID = 14;
        damage = owner.currentPower;
        //maxCoolTime = skillState.coolTime;
        skillSpeed = 200;

        swingTime = 3f; // temp
        isCooldown = false;
        onSkill = false;

        character = GetComponentInParent<ActiveCharacterController>();

        wait = new WaitForFixedUpdate();
    }

    public override void Activate()
    {
        base.Activate();
        Debug.Log("드롭 증강 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("드롭 증강 스킬 off");
        base.Deactivate();

        // 마찬가지로 레벨업 등으로 강제로 꺼질 때 청소하는 역할
        onSkill = false;

        // 켜져 있는 쉴드(자식 오브젝트) 강제로 끄기
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;

        if (isCooldown && !onSkill)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0;
                isCooldown = false;
            }
        }

        if (!onSkill && !isCooldown && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(PlayBall());
        }
    }

    IEnumerator PlayBall()
    {
        onSkill = true;

        Transform bat = null;
        if (transform.childCount > 0)
        {
            bat = transform.GetChild(0);
        }
        else
        {
            bat = BattleSession.instance.pool.Get(prefabID).transform;
            bat.parent = transform;
        }

        bat.gameObject.SetActive(true);
        bat.localPosition = Vector3.up * 2f;

        bat.GetComponent<Bullet>().Init(damage, -100, Vector3.zero);

        float Timer = 0;
        while (Timer < swingTime)
        {
            Timer += Time.deltaTime;
            transform.Rotate(Vector3.back * skillSpeed * Time.deltaTime);
            yield return wait;
        }

        // 스킬이 끝난 뒤 회전축을 원래 각도(0,0,0)로 초기화 (다음 스윙을 위해)
        transform.localRotation = Quaternion.identity;

        onSkill = false;
        isCooldown = true;
        timer = maxCoolTime;

        bat.gameObject.SetActive(false);
    }

    // 쿨타임 감소
    public override void SkillLevelUp()
    {
        if (base.skillState.originSkillData.levels[base.skillState.level].type != EffectType.CoolTime)
            return;

        this.maxCoolTime -= base.skillState.originSkillData.levels[base.skillState.level].value;
    }

    public override void ApplyStats()
    {
        damage = owner.currentPower;
    }
}
