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
    private int missionNum;
    [SerializeField]
    private GameObject missionComplete;
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject laser;
    // Start is called before the first frame update
    void Start()
    {
        enemyTotal.text = missionNum.ToString() + " /";
    }

    // Update is called once per frame
    void Update()
    {
        MissionClear();
        enemyCurrent.text = EnemyCounting().ToString();
    }
    private int EnemyCounting()
    {
        int total = missionNum;
        total -= GameManager.kill;
        if (total <= 0)
            total = 0;
        return total;
    }
    private void MissionClear()
    {
        if (GameManager.kill >= missionNum)
        {
            GameManager.isPlay = false;
            player.GetComponent<Rigidbody>().isKinematic = true;
            missionComplete.SetActive(true);

            laser.SetActive(true);
        }

    }
}
