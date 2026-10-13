using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    SkillRuntimeState skill;
    public Image icon;

    public Text[] texts;
    public Text levelText;
    public Text nameText;
    public Text descText;

    public GameObject activeSkillIcon; // 인스펙터로
    
    public void Init(SkillRuntimeState skill)
    {
        this.skill = skill;
        
        icon = GetComponentsInChildren<Image>()[2];
        icon.sprite = skill.originSkillData.icon;

        texts = GetComponentsInChildren<Text>();
        levelText = texts[0];
        nameText = texts[1];
        descText = texts[2];

        levelText.text = "Lv. " + (skill.level + 2);
        nameText.text = skill.originSkillData.skillName;
        descText.text = string.Format(skill.originSkillData.description, skill.originSkillData.levels[skill.level+1].value);

        if (skill.originSkillData.isActiveSkill) activeSkillIcon.SetActive(true);
        else activeSkillIcon.SetActive(false);
    }

    public void OnClick()
    {
        if (skill == null) return;

        skill.LevelUp();
        BattleSession.instance.ApplyCurrentSlot(); // 추가
    }
}
