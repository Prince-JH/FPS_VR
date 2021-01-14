using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool isPause;
    [SerializeField]
    private GameObject pauseMenu;
    
    private void Update()
    {
        Pause();
    }
    public void OnClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PlayerMove.healthPoint = 100;
    }
    private void Pause()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                isPause = false;
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                isPause = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }
}
