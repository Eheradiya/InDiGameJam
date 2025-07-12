using UnityEngine;
using UnityEngine.Tilemaps;

public enum WallType
{
    Normal,
    StrongWall,
    BombWall,
    AutoWall,
    ChallengeWall
}

// �븻 ��
public class Wall : MonoBehaviour
{
    [SerializeField] WallType wallType;

    int hp = 10;

    // ��� �̹�����
    [SerializeField] Sprite normal;
    [SerializeField] Sprite strong;
    [SerializeField] Sprite bomb;
    [SerializeField] Sprite auto;
    [SerializeField] Sprite challenge;

    SpriteRenderer spriteRenderer;

    private Tilemap tilemap;


    [SerializeField] LayerMask enemyLayer;

    int bombDamage = 5;
    private bool hasExploded = false; // �ߺ� ������ ���� �÷���

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        tilemap = FindObjectOfType<Tilemap>();

        switch(wallType)
        {
            case WallType.Normal:    // �Ϲ� ��
                spriteRenderer.sprite = normal;
                hp = 10;
                break;
            case WallType.StrongWall:    // �ܴ��� ��
                spriteRenderer.sprite = strong;
                hp = 20;
                break;
            case WallType.BombWall:      // ��ź ��
                spriteRenderer.sprite = bomb;
                hp = 1; // Bombs usually have low HP to trigger quickly
                break;
            case WallType.AutoWall:      // ����Ÿ���� ��
                spriteRenderer.sprite = auto;
                hp = 10;
                break;
            case WallType.ChallengeWall:     // ���� ��
                spriteRenderer.sprite = challenge;
                hp = 10;
                break;

            default:     // �ƹ��͵� ����
                print("���� ���");
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        print($"�ش� ��: {this.gameObject.name}, ���� HP: {hp}");

        if(hp <= 0)
        {
            if(wallType == WallType.BombWall)
            {
                Bomb();
            }
            Destroy(this.gameObject);
        }
    }

    void Bomb()
    {
        Debug.Log($"--- BombWall ����: �ֺ� 3x3 ������ {bombDamage} ������ ���� ���� ---");

        // �ش� �÷��̾��� ��ġ�� Int�� �ٲ㼭 ���� �׸��� ���� ����� ��ġ�� ���
        Vector3Int bombWallCell = tilemap.WorldToCell(transform.position);

        bool affectedAnyObject = false;

        // 3 x 3 �ν�
        for(int x = bombWallCell.x - 1; x <= bombWallCell.x + 1; x++)
        {
            for(int y = bombWallCell.y - 1; y <= bombWallCell.y + 1; y++)
            {
                Vector3Int currentGridCell = new Vector3Int(x, y, bombWallCell.z);

                Vector3 currentCellWorldCenter = tilemap.GetCellCenterWorld(currentGridCell);

                Collider2D[] hitObjects = Physics2D.OverlapBoxAll(
                    currentCellWorldCenter,
                    new Vector2(0.9f, 0.9f),
                    0f
                );

                if(hitObjects.Length > 0)
                {
                    Debug.Log($"Ÿ�� ({currentGridCell.x}, {currentGridCell.y})���� ��ü �߰�:");
                    foreach(Collider2D hitCollider in hitObjects)
                    {
                        if(hitCollider.gameObject == this.gameObject)
                        {
                            Debug.Log($"- (�ڽ�) {hitCollider.name}�� �ǳʍ��ϴ�.");
                            continue;
                        }

                        // Check tags and apply damage
                        if(hitCollider.CompareTag("Enemy"))
                        {
                            affectedAnyObject = true;
                            Debug.Log($"- Enemy {hitCollider.name}���� {bombDamage} ������ ����.");
                            // ���� Ÿ�� �Լ� ȣ��
                            
                        }
                        else if(hitCollider.CompareTag("Player"))
                        {
                            affectedAnyObject = true;
                            Debug.Log($"- Player {hitCollider.name}���� {bombDamage} ������ ����.");
                            hitCollider.GetComponent<PlayerController>()?.TakeDamage(bombDamage);
                        }
                        else if(hitCollider.CompareTag("Wall"))
                        {
                            affectedAnyObject = true;
                            Debug.Log($"- Wall {hitCollider.name}���� {bombDamage} ������ ����.");
                            hitCollider.GetComponent<Wall>()?.TakeDamage(bombDamage);
                        }
                        else
                        {
                            Debug.Log($"- �νĵ� ��ü: {hitCollider.name} (�±� ���� �Ǵ� �ش� ����)");
                        }
                    }
                }
            }
        }

        if(!affectedAnyObject)
        {
            Debug.Log("�ֺ� 3x3 �������� ������ ���� ��ü�� ã�� ���߽��ϴ�.");
        }
        Debug.Log("--- ���� ȿ�� ���� ---");
    }
}
