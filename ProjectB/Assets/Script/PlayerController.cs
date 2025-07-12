using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // �Է� �׼�
    PlayerInput playerInput;

    [SerializeField] Tilemap tilemap;

    // �ִϸ��̼�
    public Animator anim { get; private set; }

    Inventory inventory;

    Vector2 moveInput;  // �Է°�

    public List<GameObject> wallPrefab; //{ get; [SerializeField] set; }

    // ���߿� GameManager�� ������Ʈ�� �߰��� �ϰ� ������ �� ����. 
    public int _point; // { get; [SerializeField] set; }
    int hp = 10;
    int MaxPoint = 50;

    public Image hpFillImage;
    public int currentHP;

    Rigidbody2D rigidbody;

    public Vector2 targetPos { get; private set; }  // �÷��̾� ���⺤��

    private readonly List<Vector2Int> disallowedAbsoluteTileCoords = new List<Vector2Int>
{
        new Vector2Int(0, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1)
};

    float dot;

    float movespeed = 1f;

    BreadPointBar breadpointbar;

    public AudioSource footstepAudioSourse01;
    public AudioSource footstepAudioSourse02;
    private bool switchStepSound = false;

    private void OnEnable()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
    }

    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        inventory = GameObject.Find("Icon").GetComponent<Inventory>();

        wallPrefab = Resources.LoadAll<GameObject>("Walls").ToList();
        rigidbody = GetComponent<Rigidbody2D>();

        breadpointbar = FindObjectOfType<BreadPointBar>();
        currentHP = hp;
    }

    private void Update()
    {
        if (rigidbody.velocity != Vector2.zero)
            rigidbody.velocity = Vector2.zero;
        // ���ο� InputSystem�� ���콺 ��ġ ��������
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 playerPos = transform.position;

        Vector2 basePos = mousePos - playerPos;
        // Ÿ�ϱ��� �÷��̾��� ��ġ
        Vector3Int playerCell = tilemap.WorldToCell(playerPos);

        // Target tile for wall placement (based on player's position + small offset)
        Vector3Int targetCell = tilemap.WorldToCell((Vector3)playerPos + Vector3.down * 0.2f); // Using the existing logic for baseCell

        // World center of the target tile
        Vector3 centerPos = tilemap.GetCellCenterWorld(targetCell);

        targetPos = basePos.normalized;

        Vector2 movement = new Vector2(moveInput.x, moveInput.y);
        
        // �밢�� �̵� �ӵ� ����
        if(movement.magnitude > 1)
        {
            movement.Normalize();
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (switchStepSound && !footstepAudioSourse02.isPlaying)
            {
                switchStepSound = !switchStepSound;
                footstepAudioSourse01.Play();


            }
            else if (!switchStepSound && !footstepAudioSourse01.isPlaying)
            {
                switchStepSound = !switchStepSound;
                footstepAudioSourse02.Play();
            }
        }

        transform.Translate(movement * movespeed * Time.deltaTime);

        // �� ����
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            print("�� ��ġ �õ� Ÿ��: " + targetCell);

            Vector2Int targetCell2D = new Vector2Int(targetCell.x, targetCell.y);

            if(disallowedAbsoluteTileCoords.Contains(targetCell2D))
            {
                Debug.Log($"�� ��ġ �Ұ�: Ÿ�ϸ��� ���� ��ǥ ({targetCell2D.x}, {targetCell2D.y})�� ������ �ʴ� ��ġ�Դϴ�.");
                return;
            }

            if(_point > 0)
            {
                Collider2D hit = Physics2D.OverlapCircle(centerPos, 0.1f, LayerMask.GetMask("Wall"));
                if(hit == null)
                {
                    SendWallPoint(centerPos, inventory.keyNum, wallPrefab[inventory.keyNum - 1], inventory.keyNum);

                    SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[3]);
                }
                else
                {
                    Debug.Log("�̹� ���� �����մϴ�. ���� ��ġ���� �ʾҽ��ϴ�.");
                }
            }
            else
            {
                Debug.Log("���� ��ġ�� ����Ʈ�� �����մϴ�.");
            }
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        float Move = (int) (Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y)); 

        anim.SetFloat("Move", Move);
        
    }

    // ���ӸŴ����� �� ������.
    // ����Ʈ ���
    public void SendWallPoint(Vector3 center, int point, GameObject targetWall, int keyNum)
    {
        _point -= point;
        Instantiate(targetWall, center, Quaternion.identity);
        breadpointbar.CostBreadPoint(point);
    }

    // ����Ʈ ȹ��
    public void ApeendWallPoint(int point)
    {
        _point = _point >= 50 ? Mathf.Max(MaxPoint) : _point + point;
        breadpointbar.AddBreadPoint(point);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        UpdateHPBar();
        print($"�÷��̾��� hp�Ǵ� {currentHP}�Դϴ�.");
        SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[4]);

        if (hp <= 0)
            Destroy(this.gameObject);
    }

    void UpdateHPBar()
    {
        float fillAmount = (float)currentHP / hp;
        hpFillImage.fillAmount = fillAmount;
        Debug.Log(fillAmount);
    }
}
