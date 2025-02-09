using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    Text message;
    void Start()
    {
        message = this.GetComponent<Text>();
    }

    void Update()
    {
        
    }
    public void ShowMessageBox(string msg)
    {
        message.text = msg;
        message.enabled = true;
        Invoke("CloseMessageBox", 1.0f);

    }
    void CloseMessage()
    {
        message.enabled = false;
    }
}
