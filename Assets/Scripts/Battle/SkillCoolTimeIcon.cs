using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTimeIcon : MonoBehaviour
{
    public Image black; // 인스펙터 연결
    WaitForFixedUpdate wait;

    public Image originIcon; // 22
    public Image blackIcon; // 33

    SkillExecutor currentSkill;

    private void Awake()
    {
        wait = new WaitForFixedUpdate();
    }

    public void Init(SkillExecutor skill)
    {
        currentSkill = skill;

        Sprite skillSprite = skill.skillState.originSkillData.icon;
        originIcon.sprite = skillSprite;
        blackIcon.sprite = skillSprite;

        gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        if (currentSkill == null) return;

        if (currentSkill.maxCoolTime > 0.0f)
        {
            black.fillAmount = currentSkill.timer / currentSkill.maxCoolTime;
        }
    }
}
