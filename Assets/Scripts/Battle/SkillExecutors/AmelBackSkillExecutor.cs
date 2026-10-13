using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AmelBackSkillExecutor : SkillExecutor
{
    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);

        
    }

    public override void Activate()
    {
        base.Activate();
        Debug.Log("æ∆∏· πÈ Ω∫≈≥ on");

        foreach (SkillExecutor ex in BattleSession.instance.skillExecutors)
        {
            if (ex == null) continue;

            ex.maxCoolTime = Mathf.Max(ex.maxCoolTime - 5f, 0);
            Debug.Log($"{ex.gameObject.name}¿« ƒ≈∏¿”¿Ã {ex.maxCoolTime}¿∏∑Œ ∞®º“µ !");
        }
    }

    public override void Deactivate()
    {
        Debug.Log("æ∆∏· πÈ Ω∫≈≥ off");
        base.Deactivate();
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;
    }
}
