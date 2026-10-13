using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistyBasicSkillExecutor : SkillExecutor
{
    public float speed; // speed=timer면 음표 두기
    float Timer;
    public float firstTime; // 높은 음자리표를 기준으로 음표가 유지되는 시간, 레벨업시 빨라지는 대상
    float musicTimer;
    public bool musicStart; // 악보가 시작되었는지 체크, 높은 음자리표가 확인한다.
    bool isExploding; // 폭발 중인지 체크하기

    public ActiveCharacterController character;

    LineRenderer lineRenderer;
    int positionIndex = 0;

    WaitForSeconds wait;

    List<Transform> activeNotes = new List<Transform>(); // 자식 오브젝트로 두지 않고 리스트로 관리하도록 수정

    Transform currentBomb; // 추가

    private void Awake()
    {
        character = GetComponentInParent<ActiveCharacterController>();
        lineRenderer = GetComponentInParent<LineRenderer>();

        speed = 0.4f;
        firstTime = 7f; // 초기값

        wait = new WaitForSeconds(1f);
    }
    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);
        prefabID = 6;
        damage = 20; // 몬스터 원콤을 위해

        musicStart = false;
        isExploding = false;
    }
    public override void Activate()
    {
        base.Activate();
        Debug.Log("미스티 기본 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("미스티 기본 스킬 off");
        base.Deactivate();

        // 굳어버린 상태 강제 초기화를 위해 추가
        isExploding = false;
        FailMusic();

        // 켜져 있던 폭발 잔해가 있다면 강제로 끄기
        if (currentBomb != null)
        {
            currentBomb.gameObject.SetActive(false);
            currentBomb = null;
        }
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;

        if (isExploding) return;

        // 악보 시작시 전체 타이머 실행 및 성공 실패 여부 체크하기
        if (musicStart)
        {
            musicTimer += Time.deltaTime;

            // 1. 성공 체크 (높은 음자리표 근처 도달)
            // 방어 코드: 시작하자마자 터지는 것을 막기 위해 최소 1.5초 이상 지났을 때만 판정
            if (musicTimer > 1.5f && activeNotes.Count > 0)
            {
                Transform firstPos = activeNotes[0];
                float distance = Vector2.Distance(firstPos.position, character.transform.position);
                // 근처에 도달했다면
                if (distance < 1.0f)
                {
                    StartCoroutine(SuccessMusic());
                }
            }

            if (musicStart && musicTimer >= firstTime)
            {
                FailMusic();
            }
        }


        Timer += Time.deltaTime; ;
        if (Timer > speed)
        {
            MakeMusic();
            Timer = 0;
        }
    }

    void MakeMusic()
    {

        Transform note = null;

        if (activeNotes.Count == 0) // 리스트가 빈 경우
        {
            note = BattleSession.instance.pool.Get(prefabID).transform;

            musicStart = true;
        }
        else // 이미 깔린 음표가 있다면 일반 음표를 꺼냄.
        {
            note = BattleSession.instance.pool.Get(prefabID + 1).transform;
        }
        //    if (transform.GetChild(0).gameObject.activeSelf) // 첫번째 자식이 활성화된 경우
        //{
        //    // 음표를 가져온다.
        //    // 자식중에 비활성화된 음표가 있는지 찾고, 없다면 pool에서 가져온다.
        //    for (int i = 1; i < transform.childCount; i++)
        //    {
        //        if (!transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).CompareTag("music")) // 비활성화된 음표를 발견했다면
        //        {
        //            note = transform.GetChild(i);
        //            break;
        //        }
        //    }

        //    if (note == null) // 비활성화된 음표가 하나도 없다면
        //    {

        //    }


        //    else // 첫번쩨 자식이 비활성화된 경우
        //    {
        //        // 첫번째 자식인 높은 음자리표를 가져온다.
        //        note = transform.GetChild(0);

        //        musicStart = true;
        //    }

        //}

        //else // 자식이 없는 경우
        //{
        //    // 무조건 높은 음자리표를 pool에서 가져온다.
        //    note = BattleSession.instance.pool.Get(prefabID).transform;
        //    note.parent = transform;


        //    musicStart = true;
        //}

        note.position = character.transform.position;
        note.gameObject.SetActive(true); // 반드시 활성화

        note.GetComponent<MistyBasicNote>().InitMusicNote(this);

        // List에 등록하는 내용 추가.
        activeNotes.Add(note);

        // 라인 렌더러 추가
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(positionIndex, note.position);
        positionIndex++;
    }


    IEnumerator SuccessMusic()
    {
        isExploding = true;
        musicStart = false;
        musicTimer = 0;
        Debug.Log("악보 완성");

        // 1. 활성화된 음표들의 위치 수집
        Vector2[] points = new Vector2[activeNotes.Count];
        for (int i = 0; i < activeNotes.Count; i++)
        {
            points[i] = activeNotes[i].position;
        }

        // 3. 폭발 이펙트
        currentBomb = BattleSession.instance.pool.Get(prefabID + 2).transform;

        currentBomb.gameObject.SetActive(true);
        currentBomb.GetComponent<MistyBasicBullet>().InitMusicExplosion(damage, this, points);
        // 계산 로직은 Bullet에 위임

        yield return wait; // 애니메이션 용

        // 코루틴이 정상적으로 끝났을 때 끄기
        if (currentBomb != null)
        {
            currentBomb.gameObject.SetActive(false);
            currentBomb = null;
        }

        FailMusic();

        isExploding = false;
    }


    // 실패 및 종료 함수
    public void FailMusic()
    {
        musicStart = false;
        musicTimer = 0;

        // 음표들 비활성화
        // 안전 장치 추가
        if (activeNotes != null)
        {
            foreach (Transform note in activeNotes)
            {
                if (note != null)
                    note.gameObject.SetActive(false);
            }
            activeNotes.Clear();
        }


        // 라인 렌더러 초기화
        // 안전장치 추가
        if (lineRenderer != null)
            lineRenderer.positionCount = 0;
        positionIndex = 0;

        Debug.Log("악보 초기화");
    }

    // 지속시간 증가
    public override void SkillLevelUp()
    {
        if (base.skillState.originSkillData.levels[base.skillState.level].type != EffectType.skillRunTime)
            return;

        this.firstTime += Mathf.FloorToInt(base.skillState.originSkillData.levels[base.skillState.level].value);
    }

    public override void ApplyStats()
    {
        damage = owner.currentPower;
    }
}
