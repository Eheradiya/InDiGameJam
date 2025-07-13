using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletSpawnPos;
    [SerializeField] GameObject weapon;

    WeaponIcon weaponIcon;
    PlayerInput playerInput;
    [SerializeField] PlayerController player;

    float t = 0f;
    bool input;
    [SerializeField] float delay = 1f; // 발사 간격

    private void Start()
    {
        weaponIcon = weapon.GetComponent<WeaponIcon>();
        player = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        //playerInput.actions["Fire"].performed += OnFire;
        //playerInput.actions["Fire"].canceled += OnFire;
    }

    private void OnDisable()
    {
        //playerInput.actions["Fire"].performed -= OnFire;
        //playerInput.actions["Fire"].canceled -= OnFire;
    }

    private void Update()
    {
        if(t > 0)
            t -= Time.deltaTime;

        if(t <= 0 && Input.GetMouseButton(0))
        {
            t = 1;
            player.anim.SetBool("Attack", true);
            FireBullet();
        }
        else
        {
            player.anim.SetBool("Attack", false);
        }
    }

    //private void OnFire(InputAction.CallbackContext context)
    //{
    //    if(t <= 0)
    //    {
    //        t = delay;

    //        if(context.performed)
    //        {
    //            player.anim.SetBool("Attack", true);

    //            FireBullet();
    //        }
    //        else if(context.canceled)
    //        {
    //            player.anim.SetBool("Attack", false);

    //            FireBullet();
    //        }
    //    }
    //}

    private void FireBullet()
    {
        Sprite sprite = weaponIcon.GetCurrentWeaponSprite();
        if(sprite == null) return;

        string objectName = "Fire_" + sprite.name;
        string resourcePath = "Prefab/Player/" + objectName;

        bullet = Resources.Load<GameObject>(resourcePath);
        Instantiate(bullet, transform.position, Quaternion.identity);

        SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[6]);
    }
}
