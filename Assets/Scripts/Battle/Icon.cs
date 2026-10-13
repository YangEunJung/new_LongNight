using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    public int iconIndex;
    public Image iconImage;

    private void Awake()
    {
        //iconImage = transform.GetChild(0).GetComponent<Image>();
    }

    // Ãß°¡
    public void InitIcon(CharacterData data)
    {
        iconImage.sprite = data.icon;
    }
}
