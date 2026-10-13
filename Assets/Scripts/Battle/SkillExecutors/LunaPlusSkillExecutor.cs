using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LunaPlusSkillExecutor : SkillExecutor
{
    public bool isCoolDown; // 쿨타임 중인가?
    //public float maxCoolTime; // 발동에 필요한 쿨타임, 레벨업에 따라 달라지는 값.
    //public float cooldownTimer; // 쿨타임을 재는 타이머

    public bool isCharge; // 차지모드인가
    public float maxChargeTime; // 최대로 차지할 때까지 걸리는 시간, 제한시간이라기보단 최댓값의 의미.
    public float maxTime; // 이쪽이 차지의 제한시간

    public float damagePercent; // 차징 게이지의 값, 소수점으로 저장
    public float finalDamage; // 최종 데미지

    //[Header("UI Settings")]
    // public Transform canvasTransform; // 꼿꼿하게 세워둘 캔버스
    // public Image chargeGaugeImage;    // 채워질 게이지 이미지 (Fill)

    public ActiveCharacterController character;

    public override void Init(CharacterRuntimeState owner, SkillRuntimeState skillState)
    {
        base.Init(owner, skillState);

        prefabID = 16;
        damage = owner.currentPower;

        //maxCoolTime = skillState.coolTime; // 임시, 이후 구조 수정에 따라 변경될 수 있음.
        timer = 0;

        maxChargeTime = 2f; // temp
        maxTime = 4f; // temp

        isCoolDown = false;
        isCharge = false;

        damagePercent = 1f;
        finalDamage = damage * damagePercent;

        character = GetComponentInParent<ActiveCharacterController>();
    }

    public override void Activate()
    {
        base.Activate();
        Debug.Log("루나 증강 스킬 on");
    }

    public override void Deactivate()
    {
        Debug.Log("루나 증강 스킬 off");
        base.Deactivate();

        if (isCharge)
        {
            Time.timeScale = 1f;
            isCharge = false;
        }
        if (character != null)
        {
            character.isDashing = false;
        }
        if (transform.childCount > 0)
            transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!BattleSession.instance.isLive) return;

        // 쿨타임 타이머 로직
        if (!isCharge && isCoolDown)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                isCoolDown = false;
            }
        }

        if (!isCoolDown)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(StartCharge());
            }
            else if (Input.GetKeyUp(KeyCode.Space) && isCharge)
            {
                Fire();
            }
        }
    }

    IEnumerator StartCharge()
    {
        isCharge = true;
        character.isDashing = true;

        Time.timeScale = 0.2f;

        // 활은 항상 첫번째 자식
        Transform bow = null;
        if (transform.childCount == 0)
        {
            bow = BattleSession.instance.pool.Get(prefabID).transform;
            bow.parent = transform;
        }
        else
        {
            bow = transform.GetChild(0);
        }

        bow.gameObject.SetActive(true);
        bow.localPosition = Vector3.up * 3.5f;
        bow.localRotation = Quaternion.Euler(0, 0, 90f);

        // 화살 찾기
        Transform arrow = bow.GetChild(0);

        

        // ★ 추가: 활을 꺼낸 직후, 활의 자식 중에서 Canvas와 Fill(게이지)을 알아서 찾아옵니다.
        Transform canvasTransform = bow.Find("Canvas");
        Image chargeGaugeImage = null;
        if (canvasTransform != null)
        {
            // Canvas의 자식인 Fill 오브젝트에서 Image 컴포넌트를 가져옵니다.
            chargeGaugeImage = canvasTransform.Find("Fill").GetComponent<Image>();
        }

        // 오프셋 계산 (Canvas를 정상적으로 찾았을 때만)
        Vector3 canvasOffset = Vector3.zero;
        if (canvasTransform != null)
        {
            canvasOffset = canvasTransform.position - character.transform.position;
        }

        float Timer = 0f;
        while (isCharge && Timer <= maxTime)
        {
            Timer += Time.unscaledDeltaTime;
            damagePercent = Mathf.Min(timer / maxChargeTime, 1f);

            // 1. 활 회전
            transform.Rotate(Vector3.back * 150f * Time.unscaledDeltaTime);

            // 2. 화살 당기기
            float pullX = Mathf.Lerp(0.5f, -0.1f, damagePercent); // Lerf(a, b, t): a와 b사이에서 t퍼센트만큼 이동한 지점의 값
            arrow.localPosition = new Vector3 (pullX, arrow.localPosition.y, arrow.localPosition.z);

            // UI 게이지 sliver.value = damagePercent
            if (chargeGaugeImage != null && canvasTransform != null)
            {
                // 부모(활)가 회전하더라도 캔버스는 무조건 정면(0,0,0)을 바라보게 강제 고정!
                canvasTransform.rotation = Quaternion.identity;
                canvasTransform.position = character.transform.position + canvasOffset;
                chargeGaugeImage.fillAmount = damagePercent;
                // 색상 변경 추가
                chargeGaugeImage.color = Color.Lerp(Color.white, Color.red, damagePercent);
            }

            yield return null;
        }

        // 제한 시간이 지났는데도 발사 안했다면 강제로 발사 시킴
        if (isCharge)
        {
            Fire();
        }
    }

    // 화살 발사
    public void Fire()
    {
        isCharge = false; // StartCharge 코루틴에서 죽이기 위함.
        character.isDashing = false;
        Time.timeScale = 1f;

        if (transform.childCount == 0) return;

        Transform bowPos = transform.GetChild(0);
        Transform dummyArrow = bowPos.GetChild(0);
        // 다음 차징을 위해 화살 위치 복구
        dummyArrow.localPosition = new Vector3(0.5f, dummyArrow.localPosition.y, dummyArrow.localPosition.z);
        bowPos.gameObject.SetActive(false);

        // 1. 화살 꺼냄
        Transform arrow = null;
        arrow = BattleSession.instance.pool.Get(prefabID + 1).transform;
        arrow.position = bowPos.position;
        arrow.gameObject.SetActive(true);

        // 2. 날아가는 방향 결정
        Vector3 dir = (arrow.position - character.transform.position).normalized;
        arrow.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        finalDamage = damage * (1f + damagePercent); // 한번 더 계산해야 반영됨
        arrow.GetComponent<Bullet>().Init(finalDamage, 15, dir);

        isCoolDown = true;
        timer = maxCoolTime;
    }

    // 쿨타임
    public override void SkillLevelUp()
    {
        if (base.skillState.originSkillData.levels[base.skillState.level].type != EffectType.CoolTime)
            return;

        this.maxCoolTime -= base.skillState.originSkillData.levels[base.skillState.level].value;
    }

    public override void ApplyStats()
    {
        damage = owner.currentPower;
        // 추가
        finalDamage = damage * (damagePercent + 1f);
    }
}

