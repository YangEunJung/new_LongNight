using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;

    public Item[] items;

    public int selectionCount = 3;

    public GameObject warnPopup;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        if(items == null || items.Length == 0)
    {
            items = GetComponentsInChildren<Item>(true);
        }

        // (선택) Show에서도 rect를 쓰고 있으니 이것도 안전하게 한 번 더 체크!
        if (rect == null)
        {
            rect = GetComponent<RectTransform>();
        }

        // 사운드
        AudioManager.instance.PlaySfx(AudioManager.SFX.LevelUp); // temp

        rect.localScale = Vector3.one;
        BattleSession.instance.Stop();


        // 스킬 버튼 일단 다 끄기
        for (int i=0; i<items.Length; i++)
        {
            items[i].gameObject.SetActive(false);
        }

        SkillRuntimeState[] skillList = GetRandomSkills(BattleSession.instance.GetCanLevelUpSkills());
        if (skillList.Length == 0)
        {
            Debug.Log("레벨업 가능한 스킬이 없습니다.");
            warnPopup.SetActive(true);
            return;
        }

        SkillRuntimeState[] selected = GetRandomSkills(skillList);
        for (int i = 0; i < selected.Length; i++)
        {
            items[i].gameObject.SetActive(true);
            items[i].Init(selected[i]);
        }
    }

    public void Hide()
    {
        // ★ 추가: 만약 rect가 아직 연결되지 않았다면, 지금 당장 내 몸에서 찾아와라!
        if (rect == null)
        {
            rect = GetComponent<RectTransform>();
        }

        // 사운드
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select); // temp

        rect.localScale = Vector3.zero;
        BattleSession.instance.Resume();
    }

    public SkillRuntimeState[] GetRandomSkills(SkillRuntimeState[] all)
    {
        List<SkillRuntimeState> pool = new List<SkillRuntimeState>(all);
        List<SkillRuntimeState> result = new List<SkillRuntimeState>();

        int pickCount = Mathf.Min(selectionCount, pool.Count);

        for (int i = 0; i < pickCount; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            result.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return result.ToArray();
    }


}
