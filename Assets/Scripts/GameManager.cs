using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using Fungus;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum Quest { Prologue_0, Prologue_1, Chapter1_0, Chapter1_1, Chapter2, Chapter3, Chapter4, Chapter5, Chapter6, Chapter7 }; // temp
    Quest[] quests;
    public Quest currentQuest; // 추가
    public int questIndex = 0;
    public List<CharacterData> playables; // 플레이 가능 캐릭터 리스트, 차후엔 인스펙터에서 프리즘과 루나만 남긴다. 
    // 팀메이커에 넘길 땐 배열로 넘겨야 한다. 나중에 작성 ㄱㄱ
    public CharacterData[] allCharacters; // 인스펙터에서 연결
    public GameObject currentMap;
    //public int mapIndex = 0; // 추가 
    //public int storyIndex = 0; // 추가
    //public GameObject[] flowCharts; // 추가

    [Header("# Plag")]
    public bool isTeamMaking;
    public bool isBattle;
    public bool isStory; // 추가

    [Header("# Object")]
    public GameObject TeamMaker;
    public GameObject BattleSessionHUD;
    public GuideManager Guide; // 추가
    public GameObject[] maps;

    public GameObject virtualCamera; // 추가

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        
        isTeamMaking = false; // temp
        isBattle = false;
        isStory = false;

        // 캐릭터 해금 추가
        quests = (Quest[])Enum.GetValues(typeof(Quest));
        if (!PlayerPrefs.HasKey("MyData"))
        {
            QuestInit();
        }
        LoadPlayableCharacters();
    }

    private void Start()
    {
        StartStory(); // temp
    }

    // 게임 최초 실행시에만 실행
    public void QuestInit()
    {
        PlayerPrefs.SetInt("MyData", 1);

        foreach (Quest quest in quests)
        {
            PlayerPrefs.SetInt(quest.ToString(), 0); // false 의미
        }

        questIndex = 0;
    }

    // 게임 시작시 한 번만 실행
    void LoadPlayableCharacters()
    {
        playables.Clear();
        foreach (Quest quest in quests)
        {
            if (PlayerPrefs.GetInt(quest.ToString()) == 1)
            {
                MakePlayableCharacter(quest);
            }
        }
    }

    void MakePlayableCharacter(Quest quest)
    {
        switch (quest)
        {
            case Quest.Prologue_0:
                   break;
            case Quest.Prologue_1:
                break;
            case Quest.Chapter1_0:
                // 드롭 추가
                AddPlayableCharacter(CharacterData.Character.Drop);
                break;
            case Quest.Chapter2:
                // 도화 추가
                AddPlayableCharacter(CharacterData.Character.Dowha);
                // 비비안 추가
                AddPlayableCharacter(CharacterData.Character.Vivian);
                break;
            case Quest.Chapter3:
                break;
            case Quest.Chapter4:
                // 미스티 추가
                AddPlayableCharacter(CharacterData.Character.Misty);
                break;
            case Quest.Chapter5:
                // 아멜 추가
                AddPlayableCharacter(CharacterData.Character.AmelF);
                AddPlayableCharacter(CharacterData.Character.AmelD);
                break;
            case Quest.Chapter6:
                break;
            case Quest.Chapter7:
                // 리엘 추가
                AddPlayableCharacter(CharacterData.Character.Riel);
                break;
        }
    }

    void AddPlayableCharacter(CharacterData.Character character)
    {
        CharacterData data = Array.Find(allCharacters, element => element.character == character);
        if (data != null && !playables.Contains(data))
        {
            playables.Add(data);
        }
    }

    // 스토리 클리어시 PlayerPref.SetInt 1 하고 MakePlayableCharacter 호출하는 함수
    void UnlockQuest(Quest quest)
    {
        PlayerPrefs.SetInt(quest.ToString(), 1);
        PlayerPrefs.Save();

        MakePlayableCharacter(quest);
    }

    // 스토리에 따라 맵을 적용시킴, 수정
    void ApplyMap(Quest quest)
    {
        switch (quest)
        {
            case Quest.Prologue_0:
                currentMap = maps[0];
                break;
            case Quest.Prologue_1:
                break;
            case Quest.Chapter1_0:
                currentMap = maps[1];
                break;
            default:
                break;
        }

        foreach (GameObject map in maps)
        {
            map.SetActive(false);
        }
        currentMap.SetActive(true);
    }

    public void StartTeamMaking()
    {
        isTeamMaking = true;
        isBattle = false;
        isStory = false;

        // 플레이어블 캐릭터를 넘기는 로직을 생각해봐야겠다. 

        //StorySessionHUD.SetActive(false);
        TeamMaker.SetActive(true);
        BattleSessionHUD.SetActive(false);

        if (currentQuest == Quest.Prologue_0)
        {
            Guide.ShowPopup();
        }
    }

    public void StartBattle()
    {
        isTeamMaking = false;
        isBattle = true;
        isStory = false;

        TeamMaker.SetActive(false);
        //StorySessionHUD.SetActive(false);
        BattleSessionHUD.SetActive(true);


        ApplyMap(currentQuest); // 추가

        // ★ 추가: 스토리가 진행될 때는 플레이어 추적 카메라 끄기
        if (virtualCamera != null) virtualCamera.SetActive(true);
    }

    public void StartStory()
    {
        isTeamMaking = false;
        isBattle = false;
        isStory = true;

        TeamMaker.SetActive(false);
        BattleSessionHUD.SetActive(false);
        //StorySessionHUD.SetActive(true);

        currentQuest = quests[questIndex];
        

        // ★ 추가: 스토리가 진행될 때는 플레이어 추적 카메라 끄기
        if (virtualCamera != null) virtualCamera.SetActive(false);

        // 추가
        Fungus.Flowchart.BroadcastFungusMessage(currentQuest.ToString());

        questIndex++;

    }
}
