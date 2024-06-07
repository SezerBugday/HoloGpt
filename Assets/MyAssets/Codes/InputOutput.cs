using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputOutput : MonoBehaviour
{
   
    ChatCPT chatCPT;
    public TMP_InputField inputField;
    public Button button;   
    public TextMeshProUGUI OutputText;
    // Start is called before the first frame update
    
    void Start()
    {
       
        chatCPT = GetComponent<ChatCPT>();
        button.onClick.AddListener((() => SendPromt(inputField.text)));
    }

    // Update is called once per frame
   void SendPromt(string yazi)
    {

        Debug.Log("SendPromt: " + yazi);
        Debug.Log("SendPromt: " + yazi);
        yazi = inputField.text;
        chatCPT.Basla(yazi);
       
        
        
    }
   void Update()
    {
       OutputText.text = chatCPT.yazi;
    }

}
