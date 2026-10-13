using Unity.Cinemachine;
using UnityEngine;

public class VivianTower : MonoBehaviour
{
    Scaner scaner;
    VivianSkillExecutor skillExecutor;

    int prefabID;
    float fireTimer;

    private void Awake()
    {
       scaner = GetComponent<Scaner>();
    }
    
    public void Init(VivianSkillExecutor skillExecutor, int id)
    {
       this.skillExecutor = skillExecutor;
        this.prefabID = id;
        // 켜질 때마다 사격 쿨타임 초기화
        fireTimer = 0f;
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= skillExecutor.speed)
        {
            if (scaner.nearestTarget != null)
            {
                Fire();
                fireTimer = 0f;
            }
        }
    }

    // Tower가 호출해야 하려나... 
    public void Fire()
    {
        // 1. 투사체 꺼내기
        Transform fire = null;
        fire = BattleSession.instance.pool.Get(prefabID).transform;

        // 2. 방향 계산
        Vector2 dir = (scaner.nearestTarget.transform.position - transform.position);
        dir = dir.normalized;

        // 3. Bullet Init 호출후 날리기, 스프라이트 방향 고려해서 z축으로 180도 회전시켜야 함
        fire.position = transform.position;
        fire.rotation = Quaternion.FromToRotation(Vector3.up, dir) * Quaternion.Euler(0, 0, 180);
        
        fire.GetComponent<Bullet>().Init(skillExecutor.damage, 1, dir);
    }
}
