using UnityEngine;

public class DowhaBasicSkillExecutor: SkillExecutor
{
    //public float damage; // 공격력
    public int count; // 개수
    public float speed; // 회전 속도

    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);
        
        prefabID = 1;
        damage = base.owner.currentPower;
        speed = 150;

        Batch();
    }
    public override void Activate()
    {
        base.Activate();
        //Init();
        Debug.Log("도화 기본 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("도화 기본 스킬 off");
        base.Deactivate();
    }

    public void Batch()
    {
        for (int i = 0; i < count; i++)
        {
            Transform blade;

            if (i < transform.childCount)
            {
                blade = transform.GetChild(i);
            }
            else
            {
                blade = BattleSession.instance.pool.Get(prefabID).transform;
                blade.parent = transform;
            }

            float angle = 360 * i / count;
            blade.localPosition = Quaternion.Euler(0, 0, angle) * Vector3.up * 2f;
            blade.localRotation = Quaternion.Euler(0, 0, angle + 45);

            blade.GetComponent<Bullet>().Init(damage, -100, Vector3.zero);
        }
    }

    void Update()
    {
        if (!BattleSession.instance.isLive) return;

        transform.Rotate(Vector3.back * speed * Time.deltaTime);

        //// test
        //if (Input.GetButtonDown("Jump"))
        //{
        //    SkillLevelUp(2);
        //}

    }

    // 칼 개수 증가
    public override void SkillLevelUp()
    {
        if (base.skillState.originSkillData.levels[base.skillState.level].type != EffectType.Count)
            return;

        this.count += Mathf.FloorToInt(base.skillState.originSkillData.levels[base.skillState.level].value);
        Batch();
    }

    public override void ApplyStats()
    {
        damage = owner.currentPower;
        Batch(); // 칼들이 이미 기존의 damage를 들고 있기 때문!
    }
}
