using UnityEngine;

public class CharacterRuntimeState
{
    public CharacterData originCharacterData;

    [Header("Stats")]
    public float currentHp;
    public float currentSpeed;
    public float currentPower;

    [Header("Flag")]
    public bool isDead;

    [Header("Skill")]
    public SkillRuntimeState[] skills; // 0번째는 기본 스킬, 1번째는 증강 스킬

    public  CharacterRuntimeState(CharacterData originCharacterData)
    {
        // 1. 기본적인 스탯 데이터를 참조해 초기화
        this.originCharacterData = originCharacterData;
        currentHp = originCharacterData.maxHealth;
        currentSpeed = originCharacterData.originSpeed;
        currentPower = originCharacterData.originPower;
        isDead = false;

        // 2. 스킬 초기화
        skills = new SkillRuntimeState[originCharacterData.skillCount];
        for (int i=0; i< originCharacterData.skillCount; i++)
        {
            if (originCharacterData.skills[i] != null)
                skills[i] = new SkillRuntimeState(originCharacterData.skills[i]);
        }
    }


}
