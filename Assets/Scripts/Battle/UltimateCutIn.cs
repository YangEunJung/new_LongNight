using UnityEngine;
using UnityEngine.UI;

public class UltimateCutIn : MonoBehaviour
{
    public Image[] characters;
    int index;
    Color zero = new Color(1f, 1f, 1f, 0f); // 투명색
    Color one = new Color(1f, 1f, 1f, 1f); // 이미지 보이게

    private void OnEnable()
    {
        if (BattleSession.instance.teamData == null) return;

        index = 0;
        for (int i=0; i<characters.Length; i++)
        {
            characters[i].sprite = null;
            characters[i].color = zero;
        }
        
        for (int i=0; i< BattleSession.instance.teamData.Length; i++)
        {
            characters[i].sprite = BattleSession.instance.teamData[i].Illustrate;
            characters[i].color = one;
            index++;
        }
        if (BattleSession.instance.backData != null)
        {
            characters[index].sprite = BattleSession.instance.backData.Illustrate;
            characters[index].color = one;
        }
    }
}
