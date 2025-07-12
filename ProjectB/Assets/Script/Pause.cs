using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    // �Ͻ�����
    [SerializeField] private GameObject PausePanel;
    public bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // ���� �Ͻ�����
            PausePanel.SetActive(true); // UI ǥ��
        }
        else
        {
            Time.timeScale = 1f; // ���� �簳
            PausePanel.SetActive(false); // UI ����
        }
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1f; // ���� �簳
        PausePanel.SetActive(false); // UI ����
        SceneManager.LoadScene("Menu");
        SceneLoadManager.Instance.MainMenuCanvas.SetActive(true);
    }
    public void SettingButton()
    {
        SoundManager.Instance.OnSettingsButton();
    }
}
