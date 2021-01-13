using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    private Image hpImage;
    private Text hpText;
    // Start is called before the first frame update
    void Start()
    {
        hpImage = GetComponent<Image>();
        hpText = GameObject.Find("CurrentHP").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        HPCheck();
    }
    private void HPCheck()
    {
        switch(PlayerMove.healthPoint)
        {
            case 100:
                hpImage.fillAmount = 1;
                hpText.text = "100";
                break;
            case 90:
                hpImage.fillAmount = 0.9f;
                hpText.text = "90";
                break;
            case 80:
                hpImage.fillAmount = 0.8f;
                hpText.text = "80";
                break;
            case 70:
                hpImage.fillAmount = 0.7f;
                hpText.text = "70";
                break;
            case 60:
                hpImage.fillAmount = 0.6f;
                hpText.text = "60";
                break;
            case 50:
                hpImage.fillAmount = 0.5f;
                hpText.text = "50";
                break;
            case 40:
                hpImage.fillAmount = 0.4f;
                hpText.text = "40";
                break;
            case 30:
                hpImage.fillAmount = 0.3f;
                hpText.text = "30";
                break;
            case 20:
                hpImage.fillAmount = 0.2f;
                hpText.text = "20";
                break;
            case 10:
                hpImage.fillAmount = 0.1f;
                hpText.text = "10";
                break;
            case 0:
                hpImage.fillAmount = 0;
                hpText.text = "0";
                break;
        }    
    }
}
