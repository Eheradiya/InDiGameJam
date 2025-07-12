using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpBar : MonoBehaviour
{
    public Slider ExpSlider;
    public int maxExp = 4;
    public int currentExp;
    public int level = 0;

    [SerializeField] private TextMeshProUGUI ExpText;
    [SerializeField] private TextMeshProUGUI LvText;

    // Start is called before the first frame update
    void Start()
    {
        currentExp = 0;
        ExpSlider.maxValue = maxExp;
        ExpSlider.value = currentExp;
        ExpText.text = currentExp.ToString() + " / " + maxExp.ToString();
        LvText.text = "���� : " + level.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //�׽�Ʈ��
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddExp(1);
        }

    }

    public void RefreshExpUI()
    {
        currentExp = LevelManager.Instance.exp;
        level = LevelManager.Instance.level;
        maxExp = LevelManager.Instance.maxExp;
        ExpSlider.maxValue = maxExp;
        ExpSlider.value = currentExp;
        LvText.text = "���� : " + level.ToString();
        ExpText.text = currentExp.ToString() + " / " + maxExp.ToString();
    }
    public void AddExp(int ExpPoint)
    {
        currentExp += ExpPoint;
        ExpSlider.value = currentExp;
        ExpText.text = currentExp.ToString() + " / " + maxExp.ToString();
        if (currentExp >= maxExp)
        {
            // ������ �Լ�
            //
            level += 1;
            LvText.text = "���� : " + level.ToString();

            currentExp = 0;
            maxExp += 2;
            ExpSlider.maxValue = maxExp;
            ExpSlider.value = currentExp;
            ExpText.text = currentExp.ToString() + " / " + maxExp.ToString();
        }
    }
}
