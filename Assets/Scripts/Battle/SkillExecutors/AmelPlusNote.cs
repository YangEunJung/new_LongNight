using UnityEngine;
using System.Collections;

public class AmelPlusNote : MonoBehaviour
{
    AmelPlusSkillExecutor executor;
    public bool isGood;
    public bool isHit;


    WaitForSeconds wait;

    private void Awake()
    {
        wait = new WaitForSeconds(0.1f);
    }

    public void InitNote(bool isGood, AmelPlusSkillExecutor executor)
    {
        this.isGood = isGood;
        this.executor = executor;
    }


    public IEnumerator Bomb(bool isGood)
    {
        Transform bullet = null;

        // 좋은 카드인 경우
        if (isGood)
        {
            bullet = BattleSession.instance.pool.Get(executor.prefabID + 3).transform;
        }
        // 기본 카드인 경우
        else
        {
            bullet = BattleSession.instance.pool.Get(executor.prefabID + 2).transform;
        }

        bullet.position = transform.position;
        bullet.gameObject.SetActive(true);
        bullet.GetComponent<AmelPlusBullet>().InitCardExplosion(executor.damage, executor, isGood);

        yield return wait;

        bullet.gameObject.SetActive(false);
        isHit = false;

        // 수정: 폭발(코루틴)이 모두 끝난 뒤에야 비로소 카드 본체를 끕니다.
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("enemy")) return;

        //gameObject.SetActive(false);
        isHit = true;

        StartCoroutine(Bomb(isGood));
    }
}
