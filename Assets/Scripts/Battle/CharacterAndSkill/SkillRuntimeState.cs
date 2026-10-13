using UnityEngine;

public class SkillRuntimeState
{
    public SkillData originSkillData;

    [Header("Stats")]
    public int level;
    public float coolTime;

    [Header("Flag")]
    public bool isActive;

    public SkillRuntimeState(SkillData originSkillData)
    {
        this.originSkillData = originSkillData;

        if (originSkillData.type == SkillData.SkillType.basic)
        {
            level = 0;
            isActive = true;
            coolTime = originSkillData.baseCoolTime;

        }
        else if (originSkillData.type == SkillData.SkillType.plus)
        {
            level = -1;
            isActive = false;
            coolTime = originSkillData.baseCoolTime;
        }
    }

    public bool CanLevelUp()
    {
        return originSkillData.canLevelUp && (level < originSkillData.maxLevel);
    }

    public void LevelUp()
    {
        if (!CanLevelUp()) return;

        level++;
        if (level == 0 && originSkillData.type == SkillData.SkillType.plus)
        {
            // 증강 스킬 최초 활성화시
            isActive = true;
            //BattleSession.instance.currentExecutor.Add(BattleSession.instance.FindExecutorBySkillID(originSkillData.skillID)); // 추가
        }

        // SkillExecutor가 SkillLevelUp()를 호출함
        BattleSession.instance.FindExecutorBySkillID(originSkillData.skillID).SkillLevelUp();
    }
}
