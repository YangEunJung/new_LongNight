using System.Collections;
using UnityEngine;

public class RielSkillExecutor : SkillExecutor
{
    ActiveCharacterController character;
    Scaner scaner;

    public float runTime; // 지속 시간
    //public float timer; // 타이머
    public bool onBlack; // 스킬을 시행하고 있는지
    
    WaitForSeconds waitAnim;// start, end 애니메이션 시간
    Animator animator;

    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);

        prefabID = 20;

        runTime = 10f; //temp
        timer = maxCoolTime;

        onBlack = false;

        waitAnim = new WaitForSeconds(1f);

        character = GetComponentInParent<ActiveCharacterController>();
        scaner = character.GetComponent<Scaner>();

    }

    public override void Activate()
    {
        base.Activate();
        Debug.Log("리엘 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("리엘 스킬 off");
        base.Deactivate();
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;

        if (!onBlack)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StartCoroutine(BlackHole());
            }
        }
    }

    IEnumerator BlackHole()
    {
        onBlack = true;

        // 안전장치 추가
        if (scaner.nearestTarget == null)
        {
            onBlack = false;
            yield break; // 코루틴 즉시 종료
        }

        // 1. pool에서 프리펩 꺼내고, 위치는 Scaner로 찾음.
        Transform hole = BattleSession.instance.pool.Get(prefabID).transform;
        hole.position = scaner.nearestTarget.position;
       
        hole.gameObject.SetActive(true);

        // 2. 지속 시간 타이머 작동
        float runTimer = 0f;
        while (runTimer <= runTime)
        {
            runTimer += Time.deltaTime;
            yield return null;
        }

        // 3. 끝나면 end 애니
        onBlack = false;

        animator = hole.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("End");
        }

        yield return waitAnim;

        timer = maxCoolTime;
        hole.gameObject.SetActive(false);
    }
}
