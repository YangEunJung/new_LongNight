using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class DropBasicSkillExecutor : SkillExecutor
{
    public float speed; // speed=timer면 발사
    float Timer;
    bool isOnField;
    public bool hasHitEnemy;
    public float ballSpeed; // 공의 속력, 레벨업시 빨라지는 대상
    public float flyTime; // 공이 뻗어나가는 체공 시간

    public ActiveCharacterController character;
    Scaner scaner;

    private void Awake()
    {
        character = GetComponentInParent<ActiveCharacterController>();
        scaner = character.GetComponent<Scaner>();

        ballSpeed = 13; // 초기값
        flyTime = 0.8f;
    }
    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);
        prefabID = 5;
        damage = base.owner.currentPower;
        speed = 0.4f;
    }
    public override void Activate()
    {
        base.Activate();
        Debug.Log("드롭 기본 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("드롭 기본 스킬 off");
        base.Deactivate();

        // 1. 굳어버린 상태 강제 초기화
        isOnField = false;
        hasHitEnemy = false;

        // 2. 공중에 멈춰있는 공(자식 오브젝트)이 있다면 강제로 끄기
        if (transform.childCount > 0)
        {
            Transform ball = transform.GetChild(0);
            ball.gameObject.SetActive(false);

            // 공의 위치도 플레이어 쪽으로 강제 리셋
            ball.localPosition = Vector3.zero;
        }
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;

        if (isOnField) return;

        Timer += Time.deltaTime; ;
        if (Timer > speed)
        {
            StartCoroutine(Shoot());
            Timer = 0;
        }
    }

    IEnumerator Shoot()
    {
        hasHitEnemy = false;
        isOnField = true;

        Transform ball;

        if (transform.childCount > 0)
        {
            ball = transform.GetChild(0);
        }
        else
        {
            ball = BattleSession.instance.pool.Get(prefabID).transform;
            ball.parent = transform;
        }
        ball.gameObject.SetActive(true); // 반드시 활성화!

        //Vector3 targetPos = scaner.nearestTarget.transform.position;
        //Vector3 dir = character.lastMoveDir; // 매커니즘 수정
        //dir = dir.normalized;

        // 매커니즘 변경
        Vector2 inputVec = character.lastMoveDir;
        Transform target;
        target = FindTarget(inputVec);
        Vector2 dir;
        if (target != null)
        {
            dir = (target.position - character.transform.position).normalized;
        }
        else
        {
            dir = inputVec;
        }

        ball.position = character.transform.position;
        ball.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        // 사운드, 루나꺼로 임시
        AudioManager.instance.PlaySfx(AudioManager.SFX.LunaBasic); // temp

        ball.GetComponent<DropBasicBullet>().InitBall(damage, this, dir);


        // 수정, 적을 맞춰도 돌아오지 않고 끝까지 날아감
        float flytimer = 0f;
        while (flytimer < flyTime)
        {
            flytimer += Time.deltaTime;
            yield return null;
        }

        // 적을 맞췄거나 타임 아웃이 지난 경우 다시 플레이어에게 돌아옴
        Rigidbody2D ballRigid = ball.GetComponent<Rigidbody2D>();
        if (ballRigid != null)
            ballRigid.linearVelocity = Vector2.zero;

        // 추가: 가속도를 위한 변수 세팅
        float currentReturnSpeed = ballSpeed * 0.5f; // 처음엔 살짝 느리게 출발
        float acceleration = 40f; // 1초마다 증가할 속도 (가속도 텐션)

        // 플레이어에게 도착할 때까지 반복해서 이동
        while (true)
        {
            // 추가: 시간이 지날수록 돌아오는 속도가 기하급수적으로 빨라짐
            currentReturnSpeed += acceleration * Time.deltaTime;

            Vector3 returnVec = (character.transform.position - ball.position).normalized;
            ball.rotation = Quaternion.FromToRotation(Vector3.up, returnVec);

            // Vector3.MoveTowards를 사용하면 목표지점을 넘어가지 않고 정확히 멈춤.
            ball.position = Vector3.MoveTowards(ball.position, character.transform.position, currentReturnSpeed * Time.deltaTime);

            if (Vector3.Distance(character.transform.position, ball.position) < 0.5f)
            {
                break;
            }

            yield return null;
        }

        
        ball.gameObject.SetActive(false);

        isOnField = false;

    }

    // 매커니즘 변경
    Transform FindTarget(Vector2 inputVec)
    {
        float maxAngle = 40f; // temp
        float closestDistance = 1000000f;
        Transform closestTarget = null;

        Vector2 playerPos = character.transform.position;

        foreach (RaycastHit2D hit in scaner.targets)
        {
            if (hit.transform == null) continue;

            Vector2 toEnemy = (Vector2)hit.transform.position - playerPos;
            float angle = Vector2.Angle(inputVec , toEnemy);
            if (angle > maxAngle) continue;

            float distance = toEnemy.sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = hit.transform;
            }
        }

        return closestTarget;
    }


    //  증가
    public override void SkillLevelUp()
    {
        if (base.skillState.originSkillData.levels[base.skillState.level].type != EffectType.skillSpeed)
            return;

        this.ballSpeed += Mathf.FloorToInt(base.skillState.originSkillData.levels[base.skillState.level].value);
    }

    public override void ApplyStats()
    {
        damage = owner.currentPower;
    }
}
