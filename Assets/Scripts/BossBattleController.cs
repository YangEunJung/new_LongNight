using UnityEngine;

public class BossBattleController : MonoBehaviour
{
    // 보스전은 제한시간 없이, 보스 격파가 승리 조건, 팀 전멸은 패배, 기존 재도전 흐름 사용
    // 2차 전투도 1차 편성을 그대로 사용 ( 2차 전투 시작 시 전투 레벨과 경험치를 초기화)
    // 캐릭터 레벨업: 보스 체력 구간을 통과할 때마다.

    // 우선 순위: 공격을 받고, 패배/승리 조건을 판정하는 최소 흐름
}
