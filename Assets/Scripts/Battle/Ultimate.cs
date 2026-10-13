using UnityEngine;
using UnityEngine.UI;

public class Ultimate : MonoBehaviour
{
    public GameObject[] heartEffect; //인스펙터에서 연결, 0이 텅 빈 것, 4가 꽉 찬 것

    public Text bondText;

    public void Awake()
    {
        //bondText = GetComponent<Text>();
    }

    void LateUpdate()
    {
        if (!BattleSession.instance.isLive) return;

        // 하트 UI 다루기
        if (BattleSession.instance.bondCount == 0)
        {
            foreach (GameObject heart in heartEffect)
            {
                heart.SetActive(false);
            }
            heartEffect[0].SetActive(true);
        }
        else if (BattleSession.instance.bondCount <= 4 && BattleSession.instance.bondCount >=1)
        {
            foreach (GameObject heart in heartEffect)
            {
                heart.SetActive(false);
            }
            heartEffect[BattleSession.instance.bondCount].SetActive(true);
        }
        else
        {
            foreach (GameObject heart in heartEffect)
            {
                heart.SetActive(false);
            }
            heartEffect[4].SetActive(true);
        }

            // 유대치 텍스트
            bondText.text = BattleSession.instance.bondCount.ToString();
    }
}
