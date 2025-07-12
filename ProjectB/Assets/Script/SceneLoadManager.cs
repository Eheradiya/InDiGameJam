using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private static SceneLoadManager instance = null;

    public GameObject MainMenuCanvas;


    private void Awake()
    {
        // �̹� �ν��Ͻ��� �����ϴ��� Ȯ��
        if (instance == null)
        {
            // �ν��Ͻ��� �������� ������ ���� �ν��Ͻ��� �����ϰ� �������� �ʵ��� ����
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // �ν��Ͻ��� �̹� �����ϸ� �ߺ��Ǵ� �ν��Ͻ��� ����
            Destroy(gameObject);
        }
    }
    public static SceneLoadManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoInGame()
    {
        SceneManager.LoadScene("Test1");
        MainMenuCanvas.SetActive(false);
        SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[0]);
    }
}
