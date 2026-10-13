using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TeamMaker : MonoBehaviour
{
    public static TeamMaker instance;

    [Header("# Character Data")]
    public Team result; // 최종
    // 이 둘은 임시용! UI에서의 전용 배열
    public CharacterData[] frontSlots = new CharacterData[3];
    public CharacterData backSlot;
    public CharacterData hold;
    public int holdIndex; // 백은 10, 프론트는 각각 0, 1, 2

    public CharacterData[] playables; // 추후 게임 매니저에서 플레이 가능한 캐릭터를 다루는 내용을 넣는다. 임시론 인스펙터 창에서 관리.

    [Header("# UI")]
    public Text[] teamMakerNameTexts;
    public Image[] teamMakerIcons; //추가
    public Button[] frontInfoCharacterButtons;
    public Button[] backInfoCharacterButtons;
    public Text[] frontInfoNameTexts;
    public Text[] backInfoNameTexts;
    public Image[] frontInfo;//추가
    public Image[] backInfo; //추가
    public Text frontHoldName;
    public Text backHoldName;
    public Text frontHoldDesc;
    public Text backHoldDesc;
    public Image frontHoldIcon; // 추가
    public Image backHoldIcon;//추가

    public GameObject frontCharacterInfoUI;
    public GameObject backCharacterInfoUI;
    public GameObject warnPopUpUI;

    Color zero = new Color(1f, 1f, 1f, 0f); // 투명색
    Color one = new Color(1f, 1f, 1f, 1f); // 이미지 보이게

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        TeamMaker_Init();
    }

    // 선택된 캐릭터들을 팀 편성 창에서 확인할 수 있게 함
    void TeamMaker_Init()
    {
        // 먼저 전부 공백으로 없애준다.
        for (int i = 0; i < teamMakerNameTexts.Length; i++)
        {
            teamMakerNameTexts[i].text = "";
            teamMakerIcons[i].sprite = null; //추가
            teamMakerIcons[i].color = zero;
        }

        // 선택된 캐릭터들의 (초상화와) 이름을 채운다. 초상화 관련은 추후 업데이트
        for (int i = 0; i < frontSlots.Length; i++)
        {
            if (frontSlots[i] != null)
            {
                teamMakerNameTexts[i].text = frontSlots[i].characterName;
                teamMakerIcons[i].sprite = frontSlots[i].icon; //추가
                teamMakerIcons[i].color = one;
            }
                
        }
        if (backSlot != null)
        {
            teamMakerNameTexts[teamMakerNameTexts.Length-1].text = backSlot.characterName;
            teamMakerIcons[teamMakerIcons.Length - 1].sprite = backSlot.icon; //추가
            teamMakerIcons[teamMakerIcons.Length - 1].color = one;
        }
    }

    // 현재 플레이 가능한 캐릭터들을 배열로 받아 그들만 보이게 함
    // 플레이와 백 여부에 따라 달라야 하며, 스토리 진행도에 따라 캐릭터 해금 여부가 갈림
    void CharacterInfoUI_Init(bool isFront)
    {
        // 먼저 버튼을 비활성화 시키고 캐릭터 이름을 전부 공백으로 없애준다. 초상화 관련은 추후 업데이트.
        for (int i = 0; i < frontInfoNameTexts.Length; i++)
        {
            frontInfoCharacterButtons[i].interactable = false;
            frontInfoNameTexts[i].text = "";
            frontInfo[i].sprite = null; //추가
            frontInfo[i].color = zero;
        }
        for (int i = 0; i < backInfoNameTexts.Length; i++)
        {
            backInfoCharacterButtons[i].interactable = false;
            backInfoNameTexts[i].text = "";
            backInfo[i].sprite = null; //추가
            backInfo[i].color = zero;
        }
        frontHoldName.text = "";
        frontHoldDesc.text = "";
        backHoldName.text = "";
        backHoldDesc.text = "";
        frontHoldIcon.sprite = null; //추가
        backHoldIcon.sprite = null; //추가
        frontHoldIcon.color = zero; //추가
        backHoldIcon.color = zero; //추가

        // 플레이어블 캐릭터를 프론트와 백으로 나눠줌
        List<CharacterData> frontPlayable = new List<CharacterData>();
        List<CharacterData> backPlayable = new List<CharacterData>();
        for (int i = 0; i < playables.Length; i++)
        {
            if (playables[i].type == CharacterData.Type.Front)
                frontPlayable.Add(playables[i]);
            else if (playables[i].type == CharacterData.Type.Back)
                backPlayable.Add(playables[i]);
        }

        // 플레이 가능한 캐릭터의 (초상화와) 이름을 채워주고 이들만 버튼을 활성화 시킨다. 
        if (isFront)
        {
            CharacterData[] front = frontPlayable.ToArray();
            for (int i = 0; i < front.Length; i++)
            {
                frontInfoCharacterButtons[i].interactable = true;
                frontInfoNameTexts[i].text = front[i].characterName;
                frontInfo[i].sprite = front[i].icon;//추가
                frontInfo[i].color = one;
            }
        }
        else
        {
            CharacterData[] back = backPlayable.ToArray();
            for (int i = 0; i < back.Length; i++)
            {
                backInfoCharacterButtons[i].interactable = true;
                backInfoNameTexts[i].text = back[i].characterName;
                backInfo[i].sprite = back[i].icon;//추가
                backInfo[i].color = one;
            }
        }


    }

    // 팀 편성에서 결정 버튼 클릭시 작동하는 함수
    public void TeamGoButton()
    {
        // 1. result에 UI 전용 배열을 저장함
        BuildResultTeam();

        if (result.front.Length == 0)
        {
            Debug.LogWarning("프론트 캐릭터는 최소 1명 필요합니다.");
            warnPopUpUI.SetActive(true);
            return;
        }


        // 2.  팀 편성 ui를 꺼야함.
        GameManager.instance.StartBattle();

        // 3. result에 있는 캐릭터들을 BattleSession으로 보냄 및 배틀 시작
        BattleSession.instance.BattleStart(result);

        // 4. 사운드
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select); // temp
    }

    void BuildResultTeam()
    {
        result = new Team();

        List<CharacterData> list = new List<CharacterData>();

        for (int i = 0; i < frontSlots.Length; i++)
        {
            if (frontSlots[i] != null)
                list.Add(frontSlots[i]);
        }

        result.front = list.ToArray();
        result.back = backSlot;
    }

    // 팀 편성에서 각 자리에 해당하는 칸을 누르면 작동하는 함수 -> 캐릭터 정보 칸으로 이동한다.
    public void TeamToCharacterButton(int index)
    {
        // 1. 인덱스 번호 저장
        holdIndex = index;

        // 2. 캐릭터 정보 UI를 켠다. 
        if (index != 10)
        {
            CharacterInfoUI_Init(true);
            frontCharacterInfoUI.SetActive(true);
        }

        else
        {
            CharacterInfoUI_Init(false);
            backCharacterInfoUI.SetActive(true);
        }
            
    }

    // x버튼 클릭시 작동하는 함수 -> 캐릭터 정보창을 끈다.
    public void XButton(bool isFront)
    {
        if (isFront)
            frontCharacterInfoUI.SetActive(false);
        else
            backCharacterInfoUI.SetActive(false);
    }

    // 프론트 캐릭터 정보에서 결정 버튼 클릭시 작동하는 함수
    public void FrontCharacterInfoGoButton()
    {
        // 1. hold에 있는 캐릭터를 result에 해당하는 곳에 holdIndex에 맞게 집어 넣는다.
        if (hold == null) return;
        if (holdIndex < 0 || holdIndex >= frontSlots.Length) return;

        RemoveSameCharacterFromTeam(hold); // 만약 중복이라면 원래 자리에선 제거
        frontSlots[holdIndex] = hold;

        // 2. 캐릭터 정보창 ui를 꺼야함. XButton 호출해도 되려나.
        TeamMaker_Init();
        XButton(true);

        // 3. 사운드
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select); // temp
    }

    // 백 캐릭터 정보에서 결정 버튼 클릭시 작동하는 함수.
    public void BackCharacterInfoGoButton()
    {
        // 1. hold에 있는 캐릭터를 result에 해당하는 곳에 holdIndex에 맞게 집어 넣는다.
        if (hold == null) return;
        if (holdIndex != 10) return;

        RemoveSameCharacterFromTeam(hold);
        backSlot = hold;

        // 2. 캐릭터 정보창 ui를 꺼야함. XButton 호출해도 되려나.
        TeamMaker_Init();
        XButton(false);

        // 3. 사운드
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select); // temp
    }

    // 캐릭터 정보에서 각 캐릭터 클릭시 작동하는 함수
    public void CharacterButton(CharacterData data)
    {
        // 사전 작업: 현재의 UI 오브젝트가 프론트인지 확인
        bool isFront;
        if (data.type == CharacterData.Type.Front)
        {
            isFront = true;
        }
        else
            isFront = false;

        // 1. 왼쪽에 캐릭터의 이름과 (초상화), 설명이 업데이트 되어야 함
        if (isFront)
        {
            frontHoldName.text = data.characterName;
            frontHoldDesc.text = data.description;
            frontHoldIcon.sprite = data.icon; //추가
            frontHoldIcon.color = one;
        }
        else
        {
            backHoldName.text = data.characterName;
            backHoldDesc.text = data.description;
            backHoldIcon.sprite = data.icon; //추가
            backHoldIcon.color = one;
        }

        // 2. hold에 해당 캐릭터를 붙잡고 있어야 함.
        hold = data;
    }

    // 같은 캐릭터를 다른 슬롯에 넣는 경우는 기존 위치에서 제거하면 됨.
    void RemoveSameCharacterFromTeam(CharacterData data)
    {
        for (int i=0; i<frontSlots.Length; i++)
        {
            // 프론트에서 다른 슬롯에 넣는 일반적인 경우
            if (frontSlots[i] == data)
            {
                frontSlots[i] = null;
            }

            // 하이브리드인 아멜의 경우, 백에 넣었다면 프론트에선 제거해줘야함.
            if (frontSlots[i] != null && (frontSlots[i].character == CharacterData.Character.AmelF && data.character == CharacterData.Character.AmelD))
            {
                frontSlots[i] = null;
            }

            // 하이브리드인 아멜의 경우, 프론트에 넣었다면 백에선 제거해줘야함.
            if (backSlot != null &&  (backSlot.character == CharacterData.Character.AmelD && data.character == CharacterData.Character.AmelF))
            {
                backSlot = null;
            }
        }
    }

    public void OKButton()
    {
        warnPopUpUI.SetActive(false);
    }

    public class Team
    {
        public CharacterData[] front;
        public CharacterData back;
    }
}
