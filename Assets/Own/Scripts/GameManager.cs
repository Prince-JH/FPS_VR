using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool isPause;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    private Transform[] spawnTransform;
    public static int enemyCount = 0;
    public static int kill = 0;
    public static bool isPlay = true;
    private void Start()
    {
        InvokeRepeating("EnemySpawn", 0, 2f);
    }
    private void Update()
    {
        TryPause();
    }
    public void OnClickRestart()
    {
        isPlay = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        enemyCount = 0;
        kill = 0;
        PlayerMove.healthPoint = 100;
        if (isPause)
            Pause();
    }
    private void TryPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }
    private void Pause()
    {
        if (isPause)
        {
            Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
            Cursor.visible = false;
            isPause = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            isPause = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;//마우스 커서 고정
            Cursor.visible = true;
        }

    }
    private void EnemySpawn()
    {
        if (enemyCount < 8)
        {
            GameObject enemyClone = Instantiate(enemy, spawnTransform[Random.Range(0, spawnTransform.Length)]);
            enemyCount++;
        }
    }
    public void OnClickExit()
    {
        Application.Quit();
    }
}
