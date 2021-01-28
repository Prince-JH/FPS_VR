using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class MenuSelect : MonoBehaviour
{
    public SteamVR_Input_Sources hand = SteamVR_Input_Sources.Any;
    public SteamVR_Action_Boolean trigger;
    public SteamVR_Action_Vector2 trackPadPosition;
    public SteamVR_Action_Boolean trackPadClick;
    private Animator animator;
    [SerializeField]
    private Button[] buttons;
    private Button currentButton;
    private int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PushButton();
    }

    private void PushButton()
    {
        if(trackPadClick.GetStateDown(hand))
        {
            Vector2 pos = trackPadPosition.GetAxis(hand);
            if (pos.y > 0)
            {
                currentButton = buttons[1];
            }
            else if (pos.y < 0)
            {
                currentButton = buttons[0];
            }
            currentButton.onClick.Invoke();
        }
    }
    /*
    private void SelectButton()
    {
        if(trackPadClick.GetStateDown(hand))
        {
            Vector2 pos = trackPadPosition.GetAxis(hand);
            if(pos.y > 0)
            {
                i++;
                if (i >= buttons.Length)
                    i = 0;
                currentButton = buttons[i];
            }
            else if(pos.y < 0)
            {
                i--;
                if (i < 0)
                    i = buttons.Length - 1;
                currentButton = buttons[i];                
            }
        }
    }
    private void PushButton()
    {
        if(trigger.GetStateDown(hand))
        {
            currentButton.onClick.Invoke();
        }
    }
    private void ButtonEffect()
    {
        for(int j = 0; j < buttons.Length; j++)
        {
            if(buttons[j] == currentButton)
            {
                Color color = buttons[j].GetComponent<Image>().color;
                buttons[j].GetComponent<Image>().color = new Color(color.r, color.g, color.b, 255);
            }
            else
            {
                Color color = buttons[j].GetComponent<Image>().color;
                buttons[j].GetComponent<Image>().color = new Color(color.r, color.g, color.b, 180);
            }
        }
    }
    */
}
