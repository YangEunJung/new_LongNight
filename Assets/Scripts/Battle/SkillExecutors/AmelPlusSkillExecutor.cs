using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AmelPlusSkillExecutor : SkillExecutor
{
    public float speed; // speed=timer면 폭탄 두기
    //float timer;
    public int goodCardLuck; // 좋은 카드를 뽑을 확률, 레벨업시 상승하는 값. %값으로 저장.

    //List<GameObject> cardPool = new List<GameObject>();
    //List<GameObject> goodCardPool = new List<GameObject>();

    public ActiveCharacterController character;


    private void Awake()
    {
        character = GetComponentInParent<ActiveCharacterController>();
        speed = 4f; // temp
        goodCardLuck = 20; // 초기값
    }

    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);
        prefabID = 9;
        damage = owner.currentPower; 
    }
    public override void Activate()
    {
        base.Activate();
        Debug.Log("아멜 증강 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("아멜 증강 스킬 off");
        base.Deactivate();
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;

        timer += Time.deltaTime;
        if (timer >= speed)
        {
            Batch();
            timer = 0;
        }
    }

    void Batch()
    {
        Transform card = null;
        int ran = Random.Range(0, 100);
        bool isGood = false;

        // 좋은 카드
        if (ran < goodCardLuck)
        {
            card = BattleSession.instance.pool.Get(prefabID + 1).transform;
            isGood = true;
        }
        // 일반 카드
        else
        {
            card = BattleSession.instance.pool.Get(prefabID).transform;
        }

        card.position = character.transform.position;
        card.gameObject.SetActive(true);
        card.GetComponent<AmelPlusNote>().InitNote(isGood, this);

    }

    
    // 좋은 카드 뽑을 확률 증가
    public override void SkillLevelUp()
    {
        if (base.skillState.originSkillData.levels[base.skillState.level].type != EffectType.Percent)
            return;

        this.goodCardLuck += Mathf.FloorToInt(base.skillState.originSkillData.levels[base.skillState.level].value);
    }

    public override void ApplyStats()
    {
        damage = owner.currentPower;
    }
}
