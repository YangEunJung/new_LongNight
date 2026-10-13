using UnityEngine;

public class DowhaParry : MonoBehaviour
{
    public DohwaPlusSkillExecutor executor; // 인스펙터에서 연결

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("enemy")) return;

        if (executor == null)
            executor = BattleSession.instance.activeCharacterController.GetComponentInChildren<DohwaPlusSkillExecutor>();

        if (executor != null && executor.isParrying)
        {
            executor.OnParrySuccess();
        }
    }
}
