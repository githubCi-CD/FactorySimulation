using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ConnectFactory : MonoBehaviour
{
    public TMP_InputField URL_InputField;
    public TMP_InputField FactoryName_InputField;
    public TMP_Dropdown ExistedFactoryList_Dropdown;
    public TMP_Text Message_Text;
    public Button Connect_Button;

    void Start()
    {
        URL_InputField.text = Configration.Instance.serverHost;
        FactoryName_InputField.interactable = true;
        ExistedFactoryList_Dropdown.interactable = false;
        Message_Text.text = "[공장이름]란에 새로운 이름을 작성하면 새로운 공장이 개설됩니다.";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
