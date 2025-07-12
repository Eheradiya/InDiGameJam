using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallType
{
    Normal,
    StrongWall,
    BombWall,
    AutoWall,
    ChallengeWall
}

public class Wall : MonoBehaviour
{
    [SerializeField] WallType wallType;
    int hp = 10;

    private const int BOMB_EXPLOSION_DAMAGE = 5;

    [SerializeField] Sprite normal;
    [SerializeField] Sprite strong;
    [SerializeField] Sprite bomb;
    [SerializeField] Sprite auto;
    [SerializeField] Sprite challenge;

    SpriteRenderer spriteRenderer;

    [SerializeField] private BoxCollider2D explosionTriggerCollider;
    [SerializeField] private float explosionDuration = 0.1f;

    private HashSet<GameObject> affectedObjectsInExplosion;
    private bool hasExploded = false;

    public List<GameObject> autoDetectedEnemies = new List<GameObject>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(wallType == WallType.BombWall)
        {
            if(explosionTriggerCollider == null || !explosionTriggerCollider.isTrigger)
            {
                Debug.LogError($"���߿� Collider�� ���ų� Trigger�� �ƴ�: {gameObject.name}", this);
                enabled = false;
                return;
            }

            explosionTriggerCollider.enabled = false;
        }

        switch(wallType)
        {
            case WallType.Normal: hp = 10; break;
            case WallType.StrongWall: hp = 20; break;
            case WallType.BombWall: hp = 1; break;
            case WallType.AutoWall: hp = 10; break;
            case WallType.ChallengeWall: hp = 10; break;
        }

        spriteRenderer.sprite = GetSpriteForWallType(wallType);
    }

    private Sprite GetSpriteForWallType(WallType type)
    {
        switch(type)
        {
            case WallType.Normal: return normal;
            case WallType.StrongWall: return strong;
            case WallType.BombWall: return bomb;
            case WallType.AutoWall: return auto;
            case WallType.ChallengeWall: return challenge;
            default: return null;
        }
    }

    public void TakeDamage(int damage)
    {
        if(wallType == WallType.BombWall && hasExploded) return;

        hp -= damage;
        Debug.Log($"{gameObject.name} HP: {hp}");

        if(hp <= 0)
        {
            if(wallType == WallType.BombWall && hasExploded == false)
            {
                hasExploded = true;
                StartCoroutine(BombSequence());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator BombSequence()
    {
        Debug.Log($"BombWall ����: {gameObject.name}");

        explosionTriggerCollider.enabled = true;

        // ���� ���� ���� ��� �ݶ��̴��� ����
        Vector2 center = explosionTriggerCollider.bounds.center;
        Vector2 size = explosionTriggerCollider.bounds.size;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);

        affectedObjectsInExplosion = new HashSet<GameObject>();

        foreach(Collider2D col in hits)
        {
            GameObject target = col.gameObject;

            if(target == gameObject || affectedObjectsInExplosion.Contains(target))
                continue;

            if(target.CompareTag("Enemy") || target.CompareTag("Player") || target.CompareTag("Wall"))
            {
                ApplyDamageToTarget(target, BOMB_EXPLOSION_DAMAGE);
                affectedObjectsInExplosion.Add(target);
            }
        }

        // ��� ��� �� �ݶ��̴� ��Ȱ��ȭ (Ȥ�� �� Ʈ���� �浹 ���)
        yield return new WaitForSeconds(explosionDuration);

        explosionTriggerCollider.enabled = false;
        Debug.Log($"BombWall �ı�: {gameObject.name}");

        Destroy(gameObject);
    }

    private void ApplyDamageToTarget(GameObject target, int damage)
    {
        if(target.CompareTag("Enemy"))
        {
            Debug.Log($"Enemy {target.name}�� ������: {damage}");
            // TODO: ������ ������ ó��
        }
        else if(target.CompareTag("Player"))
        {
            Debug.Log($"Player {target.name}�� ������: {damage}");
            target.GetComponent<PlayerController>()?.TakeDamage(damage);
        }
        else if(target.CompareTag("Wall"))
        {
            Debug.Log($"Wall {target.name}�� ������: {damage}");
            target.GetComponent<Wall>()?.TakeDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(wallType != WallType.AutoWall || !explosionTriggerCollider.enabled) return;

        GameObject target = other.gameObject;

        // [�߿�] Enemy �±׸� ����
        if(target != null && target.CompareTag("Enemy"))
        {
            if(!autoDetectedEnemies.Contains(target))
            {
                autoDetectedEnemies.Add(target);
                Debug.Log($"[AutoWall] ������: {target.name}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(wallType != WallType.AutoWall) return;

        GameObject target = other.gameObject;

        // Enemy�� �������� ��� ����Ʈ���� ����
        if(target != null && target.CompareTag("Enemy"))
        {
            autoDetectedEnemies.Remove(target);
            Debug.Log($"[AutoWall] ��Ż��: {target.name}");
        }
    }
}
