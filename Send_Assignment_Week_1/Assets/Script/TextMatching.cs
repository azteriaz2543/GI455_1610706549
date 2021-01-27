using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMatching : MonoBehaviour
{
    [SerializeField] Text[] Text;
    [SerializeField] string [] Info;
    [SerializeField] InputField TextField;
    [SerializeField] Text Notifications;
    [SerializeField] GameObject FoundPanel;

    bool isFound;

    void Start()
    {
        FoundPanel.SetActive(false);

        Info[0] = "Genshin Impact";
        Info[1] = "Apex Legend";
        Info[2] = "Terraia";
        Info[3] = "Minecraft";
        Info[4] = "Among Us";
        Info[5] = "Craftopia";
        Info[6] = "UNO";
        Info[7] = "Super Hexagon";

        for (int i = 0; i < Info.Length; i++)
        {
            Text[i].text = Info[i];
        }

        isFound = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (isFound == false)
            {
                Find_Button();
            }
            else if (isFound == true)
            {
                FoundOK_Button();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isFound == false)
            {
                Find_Button();
            }
            else if (isFound == true)
            {
                FoundOK_Button();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isFound == true)
            {
                FoundOK_Button();
            }
        }
    }
    public void FoundOK_Button()
    {
        FoundPanel.SetActive(false);
        isFound = false;
        TextField.text = "";
    }
    public void Find_Button()
    {
        FoundPanel.SetActive(true);
        isFound = true;

        for (int i = 0; i < Info.Length; i++)
        {
            if (TextField.text == Info[i])
            {
                Notifications.text = "< <color=#ff0000ff>" + Info[i] + "</color> > is Found... !!!";
                break;
            }
            else if (TextField.text == "")
            {
                Notifications.text = " <size=40><color=#800080ff> You don't enter text \n Sorry ,Please try again. </color></size>";
            }
            else Notifications.text = "< <color=#ff0000ff>" + TextField.text + "</color> > Not Found...";
        }

            
    }
}
