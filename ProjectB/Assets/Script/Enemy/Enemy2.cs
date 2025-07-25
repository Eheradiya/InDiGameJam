using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy2 : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rb;
    GameObject aggroWall;
    GameObject targetObject;
    SpriteRenderer spriteRenderer;
    Color originalColor;
    Animator animator;

    ExpBar expbar;

    [SerializeField] GameObject attackCream;

    float speed = 0.64f;
    int moveAnim = 0;
    float moveCooldown = 0.5f;
    Vector2 targetDirection;

    float attackCooldown = 2f;
    bool canAttack = false;
    bool nowAttack = false;
    int attackAnim = 0;

    bool isIdle = false;
    bool isBlocked = false;
    bool isPlayerNearable = false;

    public int HP = 10;
    float takeDamageTimer = 0f;
    float dieTimer = 3f;
    bool isDie = false;

    int attackDamage = 2;

    public Image hpFillImage;
    public int currentHP;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        animator = GetComponent<Animator>();

        expbar = FindObjectOfType<ExpBar>();
        currentHP = HP;
    }

    void Move(Vector2 moveDir)
    {
        rb.velocity = moveDir * speed;
    }

    void Stop()
    {
        rb.velocity = Vector2.zero;
    }
    void Attack()
    {
        GameObject fireObj = Instantiate(attackCream, transform.position + (Vector3)(targetDirection * 0.15f), Quaternion.identity);
        fireObj.GetComponent<Enemy2_Fire>().SetDirection(targetDirection);
        fireObj.GetComponent<Enemy2_Fire>().attackDamage = attackDamage;
    }
    void Update()
    {
        spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);

        if (currentHP > 0)
        {
            if (aggroWall == null)
            {
                aggroWall = GameObject.FindGameObjectWithTag("ChallengeWall");
                if (aggroWall != null)
                    targetObject = aggroWall;
                else if (player != null)
                    targetObject = player;
            }

            targetDirection = (targetObject.transform.position - this.transform.position).normalized;



            if (attackCooldown > 0)
                attackCooldown -= Time.deltaTime;

            animator.SetFloat("Attack", attackAnim);
            animator.SetBool("NowAttack", nowAttack);
            if (attackCooldown <= 0 && (canAttack == true || attackAnim >= 1))
            {
                nowAttack = true;
                if (attackAnim == 0)
                {
                    attackAnim = 1;
                    attackCooldown = 0.3f;
                }
                else if (attackAnim == 1)
                {
                    attackAnim = 2;
                    attackCooldown = 0.3f;
                }
                else if (attackAnim == 2)
                {
                    attackAnim = 3;
                    attackCooldown = 0.3f;
                }
                else if (attackAnim == 3)
                {
                    attackAnim = 4;
                    attackCooldown = 0.3f;

                    SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[7]);
                }
                else if (attackAnim == 4)
                {
                    attackAnim = 0;
                    attackCooldown = 1f;
                    Attack();
                    nowAttack = false;
                    canAttack = false;

                }
            }


            animator.SetFloat("Move", moveAnim);
            animator.SetBool("Idle", isIdle);
            if (moveCooldown > 0)
                moveCooldown -= Time.deltaTime;

            if (nowAttack == false)
            {
                if (isIdle == false)
                {
                    if (moveCooldown <= 0)
                    {
                        if (moveAnim == 0)
                        {
                            moveAnim = 1;
                            moveCooldown = 0.3f;
                        }
                        else if (moveAnim == 1)
                        {
                            moveAnim = 2;
                            moveCooldown = 0.3f;
                        }
                        else if (moveAnim == 2)
                        {
                            moveAnim = 3;
                            moveCooldown = 0.3f;
                        }
                        else if (moveAnim == 3)
                        {
                            moveAnim = 0;
                            moveCooldown = 0.8f;
                        }
                    }

                    if (moveAnim >= 1 && moveAnim <= 2)
                        Move(targetDirection);
                    else
                        Stop();
                }
            }
            else
            {
                moveAnim = 0;
                Stop();
            }

            if (Mathf.Abs(targetDirection.x) > 0.01f)
            {
                if (targetDirection.x >= 0)
                    this.transform.localScale = new Vector3(0.32f, 0.32f, 0.32f);
                else
                    this.transform.localScale = new Vector3(-0.32f, 0.32f, 0.32f);
            }
            int wallLayer = LayerMask.GetMask("Wall");
            RaycastHit2D longHit = Physics2D.Raycast(transform.position, targetDirection, 1.6f, wallLayer);
            RaycastHit2D shortHit = Physics2D.Raycast(transform.position, targetDirection, 0.64f, wallLayer);
            Debug.DrawRay(transform.position, targetDirection * 1.6f, Color.red);

            if (longHit.collider != null && longHit.collider.CompareTag("Wall"))
            {
                canAttack = true;
                if (shortHit.collider != null && shortHit.collider.CompareTag("Wall"))
                    isBlocked = true;
                else
                    isBlocked = false;
            }
            else
            {
                isBlocked = false;
            }

            if(player != null && Vector2.Distance(transform.position, player.transform.position) <= 2.24f)
            {
                canAttack = true;
                if (Vector2.Distance(transform.position, player.transform.position) <= 1.28f)
                    isPlayerNearable = true;
                else
                    isPlayerNearable = false;
            }


            if(isBlocked || isPlayerNearable)
            {
                isIdle = true;
            }
            else
            {
                isIdle = false;
            }


            if (takeDamageTimer > 0)
            {
                spriteRenderer.color = new Color(0.8f, 0f, 0f, 1f);
                takeDamageTimer -= Time.deltaTime;
            }
            else if (spriteRenderer.color != originalColor)
            {
                spriteRenderer.color = originalColor;
            }
        }
        else
        {
            if (!isDie)
            {
                isDie = true;

                Stop();
                spriteRenderer.color = originalColor;
                //소리 재생 함수 필요

                animator.SetBool("Die", true);
                GetComponent<BoxCollider2D>().enabled = false;

                SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[8]);
                //expbar.AddExp(1);
                LevelManager.Instance.AddExp(1);
            }

            if (dieTimer > 0)
            {
                dieTimer -= Time.deltaTime;
            }
            else
            {
                LevelManager.Instance.EnemyDie(this.gameObject);
                Destroy(this.gameObject);
            }
        }

    }


    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        //소리 재생 함수 필요
        SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[9]);
        takeDamageTimer = 0.5f;
        float fillAmount = (float)currentHP / HP;
        hpFillImage.fillAmount = fillAmount;
    }

}
