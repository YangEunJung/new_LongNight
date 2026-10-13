using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class RielHole : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public float pullSpeed = 3f; // 빨아들이는 속도

    public void OnEnable()
    {
        enemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("enemy")) return;

        // 현재 생각 중인 구조
        // Enemy 코드 수정: speed 변수는 기본 속도 개념으로 냅두고, currentSpped라는 새 변수를 만든다.
        // Awake에선 currentSpeed를 speed 값으로 초기화한다.
        // OnTriggerStay에선 currentSpeed 값을 낮춘다.
        // OnTriggerExit에선 currentSpeed 값을 다시 speed 값으로 복구한다.
        // 뿐만 아니라 블랙홀이 사라질 때도 원래 속도로 복구해줘야 하므로 OnDisable(?)일때도 복구해준다.
        // 는 적들 하나하나의 속도를 어떻게 복구시켜주지? => 리스트로

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && !enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            collision.transform.position = Vector3.MoveTowards(collision.transform.position, transform.position, pullSpeed * Time.deltaTime);
            enemy.SetCurrentSpeed(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("enemy")) return;

        collision.transform.position = Vector3.MoveTowards(collision.transform.position, transform.position, pullSpeed * Time.deltaTime);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("enemy")) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            enemy.SetCurrentSpeed(false);
        }
    }

    void OnDisable()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.gameObject.activeSelf)
                enemy.SetCurrentSpeed(false);
        }

        enemies.Clear();
    }
}
