using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum DataType { Level, Exp, KillCount, Time, MainHp, SubHp0, SubHp1, SubHp2, Icon }
    public DataType type;
    Text myText;
    Slider mySlider;

    public Icon[] icons;
    //public Ultimate ultimate; // 추가

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();

    }

    void LateUpdate()
    {
        if (!BattleSession.instance.isLive) return;

        switch (type)
        {
            case DataType.Level:
                myText.text = string.Format("Lv. {0:F0}", BattleSession.instance.currentLevel);
                break;
            case DataType.Exp:
                mySlider.value = (float)BattleSession.instance.currentExp / BattleSession.instance.nextExp[BattleSession.instance.currentLevel];
                break;
            case DataType.KillCount:
                myText.text = string.Format("{0:F0}명", BattleSession.instance.currentKillCountForBond);
                break;
            case DataType.Time:
                int min = Mathf.FloorToInt((BattleSession.instance.maxGameTime - BattleSession.instance.gameTime) / 60);
                int sec = Mathf.FloorToInt((BattleSession.instance.maxGameTime - BattleSession.instance.gameTime) % 60);
                myText.text = string.Format("{0:D2} : {1:D2}", min, sec);
                break;
            case DataType.MainHp:
                mySlider.value = BattleSession.instance.activeCharacterController.currentState.currentHp / BattleSession.instance.activeCharacterController.currentState.originCharacterData.maxHealth;
                break;
            case DataType.SubHp0:
                float ch0_MaxHp = BattleSession.instance.teamData[0].maxHealth;
                float ch0_CurHp = BattleSession.instance.teamRuntimeState[0].currentHp;
                mySlider.value = ch0_CurHp / ch0_MaxHp;
                break;
            case DataType.SubHp1:
                if (BattleSession.instance.teamData.Length <= 1) break; // ★ 추가
                float ch1_MaxHp = BattleSession.instance.teamData[1].maxHealth;
                float ch1_CurHp = BattleSession.instance.teamRuntimeState[1].currentHp;
                mySlider.value = ch1_CurHp / ch1_MaxHp;
                break;
            case DataType.SubHp2:
                if (BattleSession.instance.teamData.Length <= 2) break; // ★ 추가
                float ch2_MaxHp = BattleSession.instance.teamData[2].maxHealth;
                float ch2_CurHp = BattleSession.instance.teamRuntimeState[2].currentHp;
                mySlider.value = ch2_CurHp / ch2_MaxHp;
                break;
        }
    }

    public void DeadIcon(int index)
    {
        foreach (Icon i in icons)
        {
            if (i.iconIndex == index)
            {
                i.iconImage.color = new Color(120 / 255f, 120 / 255f, 120 / 255f);
            }
        }
    }

    public void Icon_Init()
    {
        if (icons == null || icons.Length == 0)
            return;

        // 일단 모두 비활성화
        for (int i=0; i<3; i++)
        {
            icons[i].gameObject.SetActive(false);
        }
        
        // 팀원의 개수만큼 활성화
        int memberCount = BattleSession.instance.teamData.Length;

        for (int i = 0; i < memberCount; i++)
        {
            icons[i].gameObject.SetActive(true);
            icons[i].InitIcon(BattleSession.instance.teamData[i]); // 추가
            icons[i].iconImage.color = new Color(1, 1, 1);
        }
    }
}
