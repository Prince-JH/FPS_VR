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
        Pause();
    }
    public void OnClickRestart()
    {
        isPlay = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        enemyCount = 0;
        kill = 0;
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
    private void EnemySpawn()
    {
        if(enemyCount < 8)
        {
            GameObject enemyClone = Instantiate(enemy, spawnTransform[Random.Range(0, spawnTransform.Length)]);
            enemyCount++;
        }
    }
}
