using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public int skillID;
    public CharacterRuntimeState owner;
    public SkillRuntimeState skillState;
    public int prefabID; // 풀 매니저 기준

    public float damage; // 공격력

    public float maxCoolTime; // 추가
    public float timer; // 추가, 쿨타임 타이머

    public virtual void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        this.owner = owner;
        this.skillState = skillState;
        this.maxCoolTime = skillState.coolTime; // 임시, 이후 구조 수정에 따라 변경될 수 있음.

        timer = 0;
    }

    //public  void Init()
    //{
    //    Debug.Log("부모 executor에서 Init 함수 호출");
    //}

    public virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    public virtual void Deactivate() {
        gameObject.SetActive(false);
    }

    public virtual void SkillLevelUp()
    {
        Debug.Log("부모 executor에서 레벨업 함수 호출");
    }

    public virtual void ApplyStats()
    {

    }

}
