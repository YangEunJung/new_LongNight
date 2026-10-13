using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/Skill")]
public class SkillData : ScriptableObject
{
    public enum SkillType { basic, plus }

    [Header("# UI Info")]
    public SkillType type;
    public int skillID;
    public string skillName;
    [TextArea]
    public string description;
    public Sprite icon;
    public bool isActiveSkill; // 추가

    [Header("# Stats")]
    public bool canLevelUp;
    public int maxLevel;
    public float baseCoolTime;

    public GameObject projectilePrefab;

    public SkillLevelData[] levels;
}

[System.Serializable]
public class SkillLevelData
{
    public EffectType type; // 스킬 레벨업시 어떤 값이 오르는지
    public float value; // 상승 계수
}

public enum EffectType
{
    Damage, // 데미지
    CoolTime, // 쿨타임
    Per, //관통력
    Count, // 개수, +Count로 증가함.
    skillSpeed, // 공격 속도
    skillRunTime, // 스킬 지속시간
    Percent // 발동 확률
}