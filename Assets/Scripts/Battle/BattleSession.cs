using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSession : MonoBehaviour
{
    public static BattleSession instance;

    [Header("#Game Setting")]
    public float gameTime;
    public float maxGameTime; // 임시로 20
    public bool isEnd; // 배틀이 끝났는지?
    public bool isLive; // 일시적 시간 정지를 위한 플래그

    //public GameObject currentMap; // 추가, 나중에 GameManger ApplyMap과 연결 필요

    [Header("#Common Player Info")]
    public int currentExp;
    public int currentLevel; // 0부터
    public int killCount;
    public int[] nextExp = {3, 5, 10, 30, 50, 100, 200, 250 };

    [Header("#Team")]
    public CharacterData[] teamData; // 팀원 정보, 임시로 별도의 클래스를 만들지 않고 여기서 바로 취급한다.
    public CharacterRuntimeState[] teamRuntimeState; // 22
    public CharacterData backData; // 백타임 팀원 추가
    public CharacterRuntimeState backRuntimeState; // 22
    public int currentSlotNum; // 홀드 중인 캐릭터, 0부터 넘버링

    [Header("#Skills")]
    public ActiveCharacterController activeCharacterController;
    public SkillExecutor[] allExecutors;
    public SkillExecutor[] skillExecutors;
    public SkillExecutor[] allBackExecutors; // 추가
    public SkillExecutor backExecutor; // 추가
    public List<SkillExecutor> currentExecutor = new List<SkillExecutor>(); // 수정, 서브 스킬을 갖도록.

    [Header("# Ultimate")]
    public int currentKillCountForBond; // 유대치와 관련한 킬 수
    const int NeedKillCount = 10;
    public bool isEmpathy; // 공감 발현 상태
    public int bondCount; // 유대치
    public GameObject ultUI; // 팀원 일러스트 컷인 UI
    WaitForSecondsRealtime wait;
    WaitForFixedUpdate waitForFixed;

    [Header("#Game Object")]
    public LevelUp popup;
    public HUD hud;
    public GameObject gameOverUI;
    public GameObject gameClearUI;
    public GameObject enemyClaner;
    public GameObject escUI; // 추가
    public SkillCoolTimeIcon skillCoolTimeUI; // 추가

    public PoolManager pool;
    void Awake()
    {
        instance = this;

    }

    // 팀 데이터를 받도록 수정함 이에 맞게 UI도 수정할 필요가 있다.
    public void BattleStart(TeamMaker.Team team)
    {   
        teamData = team.front; // 추가
        backData = team.back; // 추가
        AudioManager.instance.PlayBgm(false); // 배경음악 추가
        //currentMap.SetActive(true); // 맵 추가

        // 기본 정보
        //isLive = true;
        //maxGameTime = 20; //temp
        gameTime = 0;
        isEnd = false;
        currentExp = 0;
        currentLevel = 0;
        killCount = 0;
        currentKillCountForBond = 0; // 이하 추가
        isEmpathy = false;
        bondCount = 0;
        wait = new WaitForSecondsRealtime(1f);
        waitForFixed = new WaitForFixedUpdate();

        // 게임 오브젝트 관련
        gameOverUI.SetActive(false);
        gameClearUI.SetActive(false);
        enemyClaner.SetActive(false);
        if (popup != null)
        {
            popup.Hide();
        }


        // 팀 편성 캐릭터 관련
        currentSlotNum = 0;
        activeCharacterController = GetComponentInChildren<ActiveCharacterController>();
        

        teamRuntimeState = new CharacterRuntimeState[teamData.Length];
        for (int i=0; i< teamRuntimeState.Length; i++)
        {
            if (teamData[i] != null)
                teamRuntimeState[i] = new CharacterRuntimeState(teamData[i]);
        }

        // (백 내용 추가)
        if (backData != null)
        {
            backRuntimeState = new CharacterRuntimeState(backData);
        }
        

        InitAllSkillExecutors();

        GetSkillExecutors();

        ApplyCurrentSlot();

        BackSkillActive(); // 추가

        hud.Icon_Init(); // 추가

        Resume();

    }

    void Update()
    {
        if (!isLive) return;
        
        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameClear();
        }

        // 숫자키를 누르면 슬롯 변경, 1 적은 수로!
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4) && bondCount >= 4) // 추가
            StartCoroutine(Ultimate());

        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            escUI.gameObject.SetActive(true);
        }
            
    }

    // 팀에 포함된 캐릭터의 SkillExecutor만을 변수에 담는 내용의 함수
    void GetSkillExecutors()
    {
        List<SkillExecutor> list = new List<SkillExecutor>();
        foreach (SkillExecutor executor in allExecutors)
        {
            for (int i=0; i< teamRuntimeState.Length; i++)
            {
                if (executor.owner == teamRuntimeState[i])
                {
                    list.Add(executor);
                }
            }
            
        }

        skillExecutors = list.ToArray();

        // (백 내용 추가)
        foreach (SkillExecutor executor in allBackExecutors)
        {
            if (backData != null)
            {
                if (executor.owner == backRuntimeState)
                {
                    backExecutor = executor;
                }
            }
        }
    }

    // 모든 SkillExecutor의 Init을 호출하고 비활성화 한다.
    void InitAllSkillExecutors()
    {
        foreach (SkillExecutor executor in allExecutors)
        {
            CharacterRuntimeState owner = null;
            SkillRuntimeState skill = null;

            FindOwnerAndSkillByID(executor.skillID, out owner, out skill);

            if (owner == null ||  skill == null)
            {
                Debug.LogWarning($"Executor 초기화 실패. SkillID: {executor.skillID}");
                continue;
            }
            executor.Init(owner, skill);
            executor.Deactivate();
        }

        // (백내용 추가)
        if (backData != null)
        {
            foreach (SkillExecutor executor in allBackExecutors)
            {
                CharacterRuntimeState owner = null;
                SkillRuntimeState skill = null;

                FindOwnerAndSkillByID(executor.skillID, out owner, out skill);

                if (owner == null || skill == null)
                {
                    Debug.LogWarning($"Executor 초기화 실패. SkillID: {executor.skillID}");
                    continue;
                }
                executor.Init(owner, skill);
                executor.Deactivate();
            }
        }
    }

    void FindOwnerAndSkillByID(int skillID, out CharacterRuntimeState owner, out SkillRuntimeState skill)
    {
        owner = null;
        skill = null;

        foreach (CharacterRuntimeState character in teamRuntimeState)
        {
            if (character == null) continue;
            foreach(SkillRuntimeState s in character.skills)
            {
                if (s == null) continue;

                if (s.originSkillData.skillID == skillID)
                {
                    owner = character; 
                    skill = s;
                    break;
                }
            }
            
        }

        if (owner != null) return;

        // (백 내용 추가)
        if (backData != null)
        {
            if (backRuntimeState.skills[0].originSkillData.skillID == skillID)
            {
                owner = backRuntimeState;
                skill = owner.skills[0];
            }
        }
    }

    // 0부터 넘버링, 현재 홀드중인 캐릭터의 슬롯 번호를 바꾼다.
    public void ChangeSlot(int slotIdx)
    {
        if (slotIdx < 0 || slotIdx >= teamRuntimeState.Length) return;
        if (teamRuntimeState[slotIdx] == null) return;
        if (teamRuntimeState[slotIdx].isDead) return;
        if (slotIdx == currentSlotNum) return;

        // 필살기 관련 추가
        if (isEmpathy)
        {
            bondCount++;
            isEmpathy = false;
        }
        currentKillCountForBond = 0;

        currentSlotNum = slotIdx;
        ApplyCurrentSlot();
    }

    // 현재 홀드중인 캐릭터의 스킬 실행을 담당한다.
    public void ApplyCurrentSlot()
    {
        CharacterRuntimeState currentState = teamRuntimeState[currentSlotNum];
        activeCharacterController.ApplyCharacter(currentState);

        // 이미 실행중인 스킬이 있다면
        if (currentExecutor != null)
        {
            foreach (SkillExecutor s in currentExecutor)
            {
                if (s != null) //  추가: s가 비어있지 않을 때만 끔.
                {
                    s.Deactivate();
                }
            }
            currentExecutor.Clear();
        }

        SkillRuntimeState basicSkillState = currentState.skills[0]; // 기본 스킬은 0번째이므로
        //currentExecutor.Add(FindExecutorBySkillID(basicSkillState.originSkillData.skillID)); // 리스트 구조에 맞게 수정
        SkillExecutor basicEx = FindExecutorBySkillID(basicSkillState.originSkillData.skillID);
        if (basicEx != null)
        {
            currentExecutor.Add(basicEx);
        }
        else
        {
            Debug.LogError($"기본 스킬(ID: {basicSkillState.originSkillData.skillID})의 Executor를 찾지 못해 리스트에 넣지 못했습니다.");
        }

        // 증강 스킬이 있는 경우에 맞게 수정
        if (currentState.skills.Length > 1 && currentState.skills[1] != null && currentState.skills[1].isActive)
        {
            SkillRuntimeState plusSkillState = currentState.skills[1]; // 증강 스킬은 1번째이므로
            //currentExecutor.Add(FindExecutorBySkillID(plusSkillState.originSkillData.skillID)); // 리스트 구조에 맞게 수정
            SkillExecutor plusEx = FindExecutorBySkillID(plusSkillState.originSkillData.skillID);
            if (plusEx != null)
            {
                currentExecutor.Add(plusEx);
            }
        }
        
        if (currentExecutor.Count == 0) // 이상현상
        {
            Debug.LogError($"SkillExecutor를 찾지 못했습니다. SkillID: {basicSkillState.originSkillData.skillID}");
            return;
        }

        //currentExecutor.Init(currentState, basicSkillState);
        // 리스트로 바뀌었으므로 수정
        foreach (SkillExecutor s in currentExecutor)
        {
            s.Activate();
        }

        // 아이콘 호출 관련 추가
        RefreshActiveSkillIcon();
    }

    public SkillExecutor FindExecutorBySkillID(int skillID)
    {
        foreach (SkillExecutor ex in skillExecutors)
        {
            if (ex.skillID == skillID)
            {
                return ex;
            }
        }

        // (백 내용 추가)
        foreach (SkillExecutor ex in allBackExecutors)
        {
            if (ex.skillID == skillID)
            {
                return ex;
            }
        }

        return null;
    }

    // 추가: 백타입 캐릭터의 스킬은 레벨업과 상관없이 계속 실행된다.
    public void BackSkillActive()
    {
        if (backData != null)
        {
            backExecutor.Activate();
        }
    }
    
    public void OnEnemyKilled(Enemy enemy)
    {
        killCount++;

        // 필살기 관련 추가
        currentKillCountForBond++; 
        if (currentKillCountForBond >= NeedKillCount)
        {
            isEmpathy = true;
        }

        UpExp();
    }

    public void UpExp()
    {
        if (!isLive) return;
        
        currentExp++;

        // Level UP
        if (currentExp >= Mathf.Min(nextExp[currentLevel], nextExp[nextExp.Length - 1]))
        {
            currentLevel++;
            LevelUpStats();
            popup.Show();
            currentExp = 0;
        }

        

    }

    public void LevelUpStats()
    {
        foreach (CharacterRuntimeState c in teamRuntimeState)
        {
            if (c.currentHp != c.originCharacterData.maxHealth)
                c.currentHp += c.originCharacterData.maxHealth * 0.1f;
            c.currentPower *= 1.20f;
            c.currentSpeed *= 1.20f;
        }

        activeCharacterController.ApplyCharacter(teamRuntimeState[currentSlotNum]);
        ApplyStatsAllExecutors();
    }

    // 레벨업시 모든 스킬 실행자에게도 스탯 반영
    public void ApplyStatsAllExecutors()
    {
        foreach (SkillExecutor s in skillExecutors)
        {
            if (s == null) continue;
            s.ApplyStats();
        }

        // (백 내용 추가)
        if (backExecutor != null)
        {
            backExecutor.ApplyStats();
        }
    }

    // 스킬 아이콘 활성화 여부
    public void RefreshActiveSkillIcon()
    {
        if (skillCoolTimeUI == null) return;

        SkillExecutor activeSkill = null;

        foreach (SkillExecutor skill in currentExecutor)
        {
            if (skill.skillState.originSkillData.isActiveSkill)
            {
                activeSkill = skill;
                break;
            }
        }

        if (activeSkill != null)
        {
            skillCoolTimeUI.Init(activeSkill);
        }
        else skillCoolTimeUI.gameObject.SetActive(false);
    }


    // time stop
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    // time flow
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }

    public SkillRuntimeState[] GetCanLevelUpSkills()
    {
        List<SkillRuntimeState> results = new List<SkillRuntimeState>();

        foreach (CharacterRuntimeState character in teamRuntimeState)
        {
            if (character == null || character.isDead) continue;
            for (int i = 0; i < character.skills.Length; i++)
            {
                if (character.skills[i] == null) continue;
                if (character.skills[i].CanLevelUp())
                {
                    results.Add(character.skills[i]);
                }
            }
                
        }

        return results.ToArray();
    }

    public void ChearacterDead()
    {
        
        hud.DeadIcon(BattleSession.instance.currentSlotNum);

        bool isAllDead = true;

        // 루프를 통해 살아있는 팀원이 있는지 확인하고, 해당 캐릭터로 넘긴다.
        for (int i = 0; i < teamRuntimeState.Length; i++)
        {
            CharacterRuntimeState state = teamRuntimeState[i];

            if (state == null) continue;
            if (state.isDead) continue;

            ChangeSlot(i);
            isAllDead = false;
            return;
        }

        if (isAllDead)
        {
            // game over
            Debug.Log("게임 오버");

            GameOver();
        }
    }

    // 팀 편성으로 돌아가기 버튼, 임시
    public void BattleRetry()
    {
        //SceneManager.LoadScene(0);

        Time.timeScale = 1f; // 추가

        gameOverUI.SetActive(false);

        GameManager.instance.isBattle = false;
        GameManager.instance.StartTeamMaking();
        pool.ClearAllObjects();
    }

    // 클리어시 다음으로 버튼, 임시
    public void BattleClear()
    {
        // SceneManager.LoadScene(0);

        Time.timeScale = 1f; // 추가
        Debug.Log("스토리 시작 시점의 Time.timeScale: " + Time.timeScale);
        gameClearUI.SetActive(false);
        // 수정
        GameManager.instance.isBattle = false;
        GameManager.instance.StartStory();
        pool.ClearAllObjects();
    }

    // 모든 팀원 전멸 시 게임 오버
    public void GameOver()
    {
        isEnd = true;

        StartCoroutine(GameOverRoutine());
    }

    // 바로 죽는 게 아니라 묘비 애니메이션 잠깐 보이도록 코루틴 실행
    IEnumerator GameOverRoutine()
    {
        // 사운드
        AudioManager.instance.PlayBgm(true); // temp
        AudioManager.instance.PlaySfx(AudioManager.SFX.Lose); // temp
        

        isLive = false;

        yield return new WaitForSeconds(0.5f);
        gameOverUI.gameObject.SetActive(true);
        Stop();
    }

    // 생존 성공 시 게임 클리어
    public void GameClear()
    {
        isEnd = true;

        StartCoroutine(GameClearRoutine());
    }

    // 바로 끝나는 게 아니라 묘비 애니메이션 잠깐 보이도록 코루틴 실행
    IEnumerator GameClearRoutine()
    {
        // 사운드
        AudioManager.instance.PlayBgm(true); // temp
        AudioManager.instance.PlaySfx(AudioManager.SFX.Win); // temp

        isLive = false;
        enemyClaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        gameClearUI.gameObject.SetActive(true);
        Stop();
    }

    IEnumerator Ultimate()
    {
        bondCount -= 4;

        Time.timeScale = 0.1f;
        ultUI.gameObject.SetActive(true);
        yield return wait;

        //추가
        if (CameraShake.instance != null)
            CameraShake.instance.Shake(0.5f, 0.3f);

        enemyClaner.SetActive(true);
        ultUI.gameObject.SetActive(false);

        Time.timeScale = 1f;

        yield return waitForFixed;
        enemyClaner.SetActive(false);
    }
}
