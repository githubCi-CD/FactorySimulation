using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Asset.Script.Backend;
using UnityEngine.SceneManagement;
using Assets.Scripts;
using static API_DTO;

public class ConnectFactory : MonoBehaviour
{
    public TMP_InputField URL_InputField;
    public TMP_InputField FactoryName_InputField;
    public TMP_Dropdown ExistedFactoryList_Dropdown;
    public TMP_Text Message_Text;
    public Button Connect_Button;

    private API_DTO.ResponseFactoryListDTO FactoryInfos;

    public enum Msg_type
    {
        NETWORK_CONNECTION_ERROR = 0,
        FACTORY_CONNECTION_ERROR = 1,
        HOST_INPUT = 2,
        FACTORY_NAME_INPUT = 3,

    }
    private List<String> msg_set = new List<string>()
    {
        $"호스트:{0}에 연결되지 않았습니다.(네트워크 오류)",
        $"공장:{0}에 접속할 수 없습니다.",
        $"호스트를 입력해주세요",
        $"공장이름을 입력해주세요",
    };

    public void messageOutput(Msg_type msg)
    {
        switch (msg)
        {
            case Msg_type.NETWORK_CONNECTION_ERROR:
                Message_Text.text = string.Format(msg_set[(int)Msg_type.NETWORK_CONNECTION_ERROR], Configration.Instance.serverHost);
                break;
            case Msg_type.FACTORY_CONNECTION_ERROR:
                Message_Text.text = string.Format(msg_set[(int)Msg_type.FACTORY_CONNECTION_ERROR], FactoryName_InputField.text);
                break;
            case Msg_type.HOST_INPUT:
                Message_Text.text = string.Format(msg_set[(int)Msg_type.HOST_INPUT]);
                break;
            case Msg_type.FACTORY_NAME_INPUT:
                Message_Text.text = string.Format(msg_set[(int)Msg_type.FACTORY_NAME_INPUT]);
                break;
            default:
                break;
        }
        Debug.Log(Message_Text.text);
    }

    void Start()
    {
        URL_InputField.text = Configration.Instance.serverHost;
        ExistedFactoryList_Dropdown.ClearOptions();
        OnClickFactoryList();
        ExistedFactoryList_Dropdown.onValueChanged.AddListener(delegate { OnClickFactoryList(); });
        ExistedFactoryList_Dropdown.onValueChanged.AddListener(delegate { selectDropdownValue(); });
        Connect_Button.onClick.AddListener( delegate { OnClickConnect();  });
        Message_Text.text = "[공장이름]란에 새로운 이름을 작성하면 새로운 공장이 개설됩니다.";
    }

    public void selectDropdownValue()
    {
        if (ExistedFactoryList_Dropdown.value == 1)
        {
            FactoryName_InputField.text = "";
        }
        else
        {
            FactoryName_InputField.text = ExistedFactoryList_Dropdown.options[ExistedFactoryList_Dropdown.value].text;
        }
        OnClickFactoryList();
    }

    public void OnClickFactoryList()
    {
        Configration.Instance.serverHost = URL_InputField.text;
        if (Configration.Instance.standAloneMode == true)
        {
            UpdateFactoryList(BackendDemoData.responseFactoryListDemo());
        }
        else
        {
            APIHandler.Instance.GetFactoryList(APIType.GET_FACTORY_LIST, UpdateFactoryList);
        }
    }

    public bool UpdateFactoryList(API_DTO.ResponseFactoryListDTO res)
    {
        if(res == null)
        {
            messageOutput(Msg_type.NETWORK_CONNECTION_ERROR);
            return false;
        }
        ExistedFactoryList_Dropdown.ClearOptions();
        ExistedFactoryList_Dropdown.options.Add(new TMP_Dropdown.OptionData("공장 목록"));
        ExistedFactoryList_Dropdown.options.Add(new TMP_Dropdown.OptionData("새로운 공장"));
        API_DTO.ResponseFactoryListDTO factorys;
        FactoryInfos = res;
        foreach (API_DTO.FactoryInfoDTO factory in res.factoryList)
        {
            
            factorys = new API_DTO.ResponseFactoryListDTO();
            factorys.factoryList = new List<API_DTO.FactoryInfoDTO>
            {
                factory
            };            
            TMPro.TMP_Dropdown.OptionData option = new TMPro.TMP_Dropdown.OptionData(factory.name);
            
            ExistedFactoryList_Dropdown.options.Add(new TMP_Dropdown.OptionData(factory.name));
        }

        return true;
    }

    public void OnClickConnect()
    {
        if(URL_InputField.text == "")
        {
            messageOutput(Msg_type.HOST_INPUT);
            return;
        }
        if (FactoryName_InputField.text == "")
        {
            messageOutput(Msg_type.FACTORY_NAME_INPUT);
            return;
        }

        Configration.Instance.serverHost = URL_InputField.text;
        if (Configration.Instance.standAloneMode == true)
        {
            API_DTO.ResponseLoginLogoutDto connectFactoryDTO = new API_DTO.ResponseLoginLogoutDto();
            connectFactoryDTO.id = 1;
            connectFactoryDTO.name = FactoryName_InputField.text;
            ConnectToFactory(connectFactoryDTO);
        }
        else
        {
            APIHandler.Instance.ConnectFactory(APIType.CONNECT_FACTORY, FactoryName_InputField.text, ConnectToFactory);
        }
    }

    private bool ConnectToFactory(API_DTO.ResponseLoginLogoutDto res)
    {
        if (res == null)
        {
            messageOutput(Msg_type.NETWORK_CONNECTION_ERROR);
            return false;
        }
        FactoryInfoDTO loginFactoryInfo = FactoryInfos.factoryList.Find(x => x.id == res.id);
        Configration.Instance.factoryId = res.id;
        Configration.Instance.factoryName = res.name;
        if (Configration.Instance.standAloneMode == true)
        {
            Configration.Instance.totalCount = loginFactoryInfo.totalCount;
        }
        Debug.Assert(Configration.Instance.startAtFactoryMode == false, "공장시작 모드로 설정되어있습니다.");
        SceneManager.LoadScene("Factory");

        return true;
    }
}
