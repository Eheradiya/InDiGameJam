using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    GameObject bullet;
    [SerializeField] Transform bulletSpawnPos;
    [SerializeField] WeaponIcon weaponIcon;

    PlayerInput playerInput;
    [SerializeField] PlayerController player;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.actions["Fire"].performed += OnFire;
        playerInput.actions["Fire"].canceled += OnFire;
    }

    private void OnDisable()
    {
        playerInput.actions["Fire"].performed -= OnFire;
        playerInput.actions["Fire"].canceled -= OnFire;
    }

    private void Update()
    {

    }

    [SerializeField] float fireDelay = 0.2f;
    private Coroutine fireCoroutine;

    private void OnFire(InputAction.CallbackContext context)
    {
        if(context.performed)  // ���콺�� ������ ������ ����
        {
            player.anim.SetBool("Attack", true);
            fireCoroutine = StartCoroutine(FireContinuously());
        }
        else if(context.canceled)  // ���콺 ��ư���� ���� �� ����
        {
            if(fireCoroutine != null)
            {
                player.anim.SetBool("Attack", false);
                StopCoroutine(fireCoroutine);
            }
        }
    }

    private void LoadBulletFromSprite()
    {
        if (weaponIcon == null || weaponIcon.img == null || weaponIcon.img.sprite == null) return;

        string spriteName = "Fire_" + weaponIcon.img.sprite.name;
        GameObject loadedPrefab = Resources.Load<GameObject>($"Prefab/Player/{spriteName}");
        print($"localedPrefabs: {loadedPrefab}, spriteName: {spriteName}" );

        if (loadedPrefab != null)
        {
            bullet = loadedPrefab;
        }
    }

    // �Ѿ� �߻� �޼���
    private void FireBullet()
    {
<<<<<<< Updated upstream
        Instantiate(bullet, transform.position, Quaternion.identity);
        Fire_Hardtack hardtackMethod = bullet.GetComponent<Fire_Hardtack>();
        print(player.targetPos);
        hardtackMethod.targetDirection = player.targetPos;
        // ����, ����Ʈ ���� ���⿡ �߰�
        // 
        SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[6]);
=======
        LoadBulletFromSprite();
        if(bullet == null) return;

        GameObject spawnedBullet = Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity);
        Fire_Hardtack hardtackMethod = spawnedBullet.GetComponent<Fire_Hardtack>();
        if(hardtackMethod != null)
        {
            hardtackMethod.targetDirection = player.targetPos;
        }

        //Instantiate(bullet, transform.position, Quaternion.identity);
        //Fire_Hardtack hardtackMethod = bullet.GetComponent<Fire_Hardtack>();
        //print(player.targetPos);
        //hardtackMethod.targetDirection = player.targetPos;
        // ����, ����Ʈ ���� ���⿡ �߰� 
>>>>>>> Stashed changes
    }

    // ���� �ð����� �߻��ϴ� ��ƾ
    private IEnumerator FireContinuously()
    {
        while(true)
        {
            // �Ѿ� �߻�
            FireBullet();
            yield return new WaitForSeconds(fireDelay);
        }
    }
}


