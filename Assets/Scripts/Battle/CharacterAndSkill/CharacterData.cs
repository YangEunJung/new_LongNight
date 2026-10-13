using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/Character")]
public class CharacterData : ScriptableObject
{
    public enum Character { Prism, Luna, Drop, Riel, Misty, Dowha, Vivian, AmelF, AmelD }
    public enum Type { Front, Back}
    

    [Header("# UI Info")]
    public Character character;
    public string characterName;
    public int characterID;
    public Sprite icon;
    [TextArea]
    public string description;
    public Sprite Illustrate;

    [Header("# Animation")]
    public RuntimeAnimatorController moveAnim;

    [Header("# Stats")]
    public Type type;

    public float maxHealth;
    public float originSpeed;
    public float originPower; // 공격력

    [Header("# Skill")]
    public SkillData[] skills; // 0번째는 기본 스킬, 1번째는 증강 스킬
    public int skillCount; // 스킬 개수, 이후 프리즘은 3개가 되므로
    
}
