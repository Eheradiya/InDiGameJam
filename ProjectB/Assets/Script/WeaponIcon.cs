using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Collections.Generic;

public class WeaponIcon : MonoBehaviour
{
    [SerializeField] private List<Sprite> weaponSprites;

    public Image img;

    public float scrollValue = 0f;
    public float scrollSpeed = 1f;
    public float minValue = 0f;
    public float maxValue = 10f;

    private int currentIndex = 0;
    private int nowIndex = 0;
    private int previousIconIndex = -1;

    private float lastScrollTime = 0f;
    [SerializeField] private float scrollIdleResetTime = 1.5f; // �Է� ���� �� �ʱ�ȭ �ð� (��)

    private void Start()
    {
        img = GetComponent<Image>();

        UpdateIconSprite();
    }

    private void Update()
    {
        if(Mouse.current == null) return;

        float scrollDelta = Mouse.current.scroll.ReadValue().y;

        if(Mathf.Abs(scrollDelta) > 0.01f)
        {
            if (scrollDelta > 0f)
            {
                nowIndex = (currentIndex + 1) % weaponSprites.Count;
                if (nowIndex >= 3 && LevelManager.Instance.level < 13)
                    nowIndex = nowIndex % 3;
                else if (nowIndex >= 2 && LevelManager.Instance.level < 6)
                    nowIndex = nowIndex % 2;
                else if (nowIndex >= 1 && LevelManager.Instance.level < 2)
                    nowIndex = nowIndex % 1;

                currentIndex = nowIndex;
            }
            else if (scrollDelta < 0f)
            {

                currentIndex = (currentIndex - 1 + weaponSprites.Count) % weaponSprites.Count;

                if (nowIndex >= 3 && LevelManager.Instance.level < 13)
                    nowIndex = nowIndex % 3;
                else if (nowIndex >= 2 && LevelManager.Instance.level < 6)
                    nowIndex = nowIndex % 2;
                else if (nowIndex >= 1 && LevelManager.Instance.level < 2)
                    nowIndex = nowIndex % 1;

                currentIndex = nowIndex;
            }
        }

        UpdateIconSprite();
    }

    private void UpdateIconSprite()
    {
        if(currentIndex != previousIconIndex)
        {
            img.sprite = weaponSprites[currentIndex];
            previousIconIndex = currentIndex;

            Debug.Log($"������ ����: {weaponSprites[currentIndex].name} (�ε���: {currentIndex})");
        }
    }

    public int GetCurrentIndex() => currentIndex;

    public Sprite GetCurrentWeaponSprite()
    {
        return weaponSprites[currentIndex]; // currentIndex�� ���ο��� ���� ��
    }
}