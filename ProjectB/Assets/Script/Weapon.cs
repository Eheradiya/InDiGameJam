using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletSpawnPos;

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

    // �Ѿ� �߻� �޼���
    private void FireBullet()
    {
        Instantiate(bullet, transform.position, Quaternion.identity);
        Fire_Hardtack hardtackMethod = bullet.GetComponent<Fire_Hardtack>();
        print(player.targetPos);
        hardtackMethod.targetDirection = player.targetPos;
        // ����, ����Ʈ ���� ���⿡ �߰�
        // 
        SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[6]);
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


