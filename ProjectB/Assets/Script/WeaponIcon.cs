using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Collections.Generic;

public struct GunCount
{
    public int pirtalCount;
    public int UZICount;
    public int RailGunCount;
    public int RocketRuncherCount;
}

public class WeaponIcon : MonoBehaviour
{
    [SerializeField] private List<Sprite> weaponSprites;

    public Image img;

    public float scrollValue = 0f;
    public float scrollSpeed = 1f;
    public float minValue = 0f;
    public float maxValue = 10f;

    public int previousIconIndex  = -1;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    private void Update()
    {
        if(Mouse.current == null) return;

        float scrollDelta = Mouse.current.scroll.ReadValue().y;

        if(Mathf.Abs(scrollDelta) > 0.01f)
        {
            scrollValue += scrollDelta * scrollSpeed * Time.deltaTime;
            scrollValue = Mathf.Clamp(scrollValue, minValue, maxValue);
            Debug.Log($"Scroll Value: {scrollValue}");
        }
        else if(scrollDelta >= 10f)
        {
            scrollDelta = 0;
        }

        UpdateIconSprite();
    }

    private void UpdateIconSprite()
    {
        int newIconIndex = Mathf.FloorToInt(scrollValue / 2f) % weaponSprites.Count;
        newIconIndex = (newIconIndex + weaponSprites.Count) % weaponSprites.Count; // ���� ��� ����

        // ���� �ε����� �ٸ� ���� ��������Ʈ ������Ʈ
        if(newIconIndex != previousIconIndex)
        {
            img.sprite = weaponSprites[newIconIndex]; // [�� ��] ��������Ʈ �Ҵ�
            previousIconIndex = newIconIndex; // ���� �ε��� ����

            Debug.Log($"������ ����: {weaponSprites[newIconIndex].name} (�ε���: {newIconIndex})");
        }
    }
}
