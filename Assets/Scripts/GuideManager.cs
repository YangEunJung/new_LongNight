using UnityEngine;
using UnityEngine.UI;

public class GuideManager : MonoBehaviour
{
    // 왼 오 버튼, 이미지, x버튼 등

    public int index;
    public Sprite[] list; // 인스펙터 연결
    public Image image; // 22

    public Button left; //33
    public Button right; //44

    private void Awake()
    {
        index = 0;
        UpdateButtonState();
    }

    public void LeftButton()
    {
        if (index <= 0)
        {
            return;
        }

        index -= 1;
        index = Mathf.Max(index, 0);
        image.sprite = list[index];

        UpdateButtonState();
    }

    public void RightButton()
    {
        if (index >= list.Length - 1)
        {
            return;
        }

        index += 1;
        index = Mathf.Min(index, list.Length);
        image.sprite = list[index];

        UpdateButtonState();
    }

    public void XButton()
    {
        gameObject.SetActive(false);
    }

    public void ShowPopup()
    {
        gameObject.SetActive (true);
    }

    void UpdateButtonState()
    {
        left.interactable = index > 0;
        right.interactable = index < list.Length - 1;
    }
}
