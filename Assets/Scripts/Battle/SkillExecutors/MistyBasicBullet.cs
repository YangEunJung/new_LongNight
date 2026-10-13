using UnityEngine;

public class MistyBasicBullet : Bullet
{
    MistyBasicSkillExecutor executor;
    PolygonCollider2D polyCollider;

    // 유니티 인스펙터에서 할당: 폭발 애니메이션이 들어있는 자식 오브젝트
    public Transform visualEffect;

    private void Awake()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
    }

    public void InitMusicExplosion(float damage, MistyBasicSkillExecutor executor, Vector2[] worldPoints)
    {
        this.damage = damage;
        this.per = -100; // 관통 로직 사용 안함
        this.executor = executor;

        // 콜라이더 계산은 여기서 한다.
        // 1. 악보의 중앙 좌표 찾기
        Vector2 centor = Vector2.zero;
        foreach (Vector2 p in worldPoints)
        {
            centor += p;
        }
        centor /= worldPoints.Length;

        transform.position = centor;

        // 2. 콜라이더 씌우기
        Vector2[] localPoints = new Vector2[worldPoints.Length];
        for (int i = 0; i < worldPoints.Length; i++)
        {
            localPoints[i] = worldPoints[i] - centor;
        }
        polyCollider.points = localPoints;

        // 3. 폭팔 이팩트 크기 조정: 가로 세로 중 더 작은 것에 맞춤
        Vector2 size = polyCollider.bounds.size;
        float minScale = Mathf.Min(size.x, size.y);
        
        if (minScale < 2.0f) minScale = 2.0f;// 너무 작아지는 것을 방지하기 위한 최소 크기 보정 (필요시 조절)

        if (visualEffect != null)
        {
            visualEffect.localScale = new Vector3(minScale, minScale, 1f);
        }

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("enemy")) // 적이랑 부딪힌 게 아니면
        {
            return;
        }

        if (executor != null)
        {
            Debug.Log("폭발");
        }
    }
}
