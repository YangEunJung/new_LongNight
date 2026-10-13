using System.Collections;
using UnityEngine;

public class VivianSkillExecutor : SkillExecutor
{
    public float dealTime; // 지속 시간
    
    //public float maxCoolTime; // 최대 쿨타임 시간
    //public float timer; // 쿨타임 타이머

    public float speed; // 레이져 발사 속도

    public GameObject spawnPositions; // 인스펙터로 연결, 빈 오브젝트에 생성 될 만한 위치만 자식으로 여럿 가지고 있음.
    Transform[] pos;

    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);

        prefabID = 18;
        damage = owner.currentPower;

        dealTime = 20f; // test
        //maxCoolTime = 15f; // test
        speed = 0.5f; // test
        timer = maxCoolTime;

        pos = new Transform[spawnPositions.transform.childCount];
        for (int i = 0; i < spawnPositions.transform.childCount; i++)
        {
            pos[i] = spawnPositions.transform.GetChild(i).transform;
        }
    }

    public override void Activate()
    {
        base.Activate();
        Debug.Log("비비안 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("비비안 스킬 off");
        base.Deactivate();
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;

        // 쿨타임 타이머 관리
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            StartCoroutine(Batch());
            timer = maxCoolTime;
        }
    }

    IEnumerator Batch()
    {
        Transform tower = null;

        // 자식으로 두지 않음.
        tower = BattleSession.instance.pool.Get(prefabID).transform;

        int randomIndex = Random.Range(0, pos.Length);
        tower.position = pos[randomIndex].transform.position;
        tower.GetComponent<VivianTower>().Init(this, prefabID + 1);
        tower.gameObject.SetActive(true);

        float runtimeTimer = 0f;
        while (runtimeTimer <= dealTime)
        {
            runtimeTimer += Time.deltaTime;

            yield return null;
        }

        
        tower.gameObject.SetActive(false);
    }

    public override void ApplyStats()
    {
        damage = owner.currentPower;
    }
}
