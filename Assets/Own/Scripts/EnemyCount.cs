using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCount : MonoBehaviour
{
    [SerializeField]
    private Text enemyTotal;
    [SerializeField]
    private Text enemyCurrent;
    [SerializeField]
    private GameObject[] enemies;
    // Start is called before the first frame update
    void Start()
    {
        enemyTotal.text = enemies.Length.ToString() + " ";
    }

    // Update is called once per frame
    void Update()
    {
        enemyCurrent.text = EnemyCounting().ToString();
    }
    private int EnemyCounting()
    {
        int total = enemies.Length;
        foreach(GameObject eachEnemy in enemies)
        {
            if (!eachEnemy.activeSelf)
                total--;
        }
        return total;
    }
}
