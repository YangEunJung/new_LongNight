using UnityEngine;

public class BattleEsc : MonoBehaviour
{
    public void OnClickReturn()
    {
        gameObject.SetActive(false);
        BattleSession.instance.Resume();
    }

    public void OnClickGoTeam()
    {
        // юс╫ц
        BattleSession.instance.BattleRetry();
        gameObject.SetActive(false);
    }
}
