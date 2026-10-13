using UnityEngine;

public class LunaBasicSkillExecutor: SkillExecutor
{
    //public float damage; // 공격력
    public int per = 1; // 관통력
    public float speed; // 발사 속도

    float Timer;

    ActiveCharacterController character;
    Scaner scaner;

    private void Awake()
    {
        character = GetComponentInParent<ActiveCharacterController>();
        scaner = character.GetComponent<Scaner>();
    }
    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);
        prefabID = 2;
        damage = base.owner.currentPower;
        speed = 0.3f;
    }
    public override void Activate()
    {
        base.Activate();
        //Init();
        Debug.Log("루나 기본 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("루나 기본 스킬 off");
        base.Deactivate();
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;

        Timer += Time.deltaTime; ;
        if (Timer > speed)
        {
            Shoot();
            Timer = 0;
        }
    }

   

    void Shoot()
    {
        if (!scaner.nearestTarget) return;

        // 사운드
        AudioManager.instance.PlaySfx(AudioManager.SFX.LunaBasic); // temp

        Vector3 targetPos = scaner.nearestTarget.transform.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform arrow = BattleSession.instance.pool.Get(prefabID).transform;
        arrow.position = character.transform.position;
        arrow.rotation = Quaternion.FromToRotation(Vector3.up, dir);


        arrow.GetComponent<Bullet>().Init(damage, per, dir);
    }

    // 관통력 증가
    public override void SkillLevelUp()
    {
        if (base.skillState.originSkillData.levels[base.skillState.level].type != EffectType.Per)
            return;

        this.per += Mathf.FloorToInt(base.skillState.originSkillData.levels[base.skillState.level].value);
    }

    public override void ApplyStats()
    {
        damage = owner.currentPower;
    }
}
