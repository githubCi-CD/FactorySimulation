using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static API_DTO;

namespace Asset.Script.Backend
{
    //FactoryDTO : 공장 정보 요청시 사용되는 DTO
    //LoginDTO : 로그인 요청시 사용되는 DTO
    //https://github.com/githubCi-CD/factory/blob/main/src/main/kotlin/spring/factotry/dto/LoginDto.kt

    //OriginDTO : 원자재 요청시 사용되는 DTO
    //https://github.com/githubCi-CD/factory/blob/main/src/main/kotlin/spring/factotry/dto/OriginDto.kt

    //StorageDto : 창고 정보 요청시 사용되는 DTO
    //https://github.com/githubCi-CD/factory/blob/main/src/main/kotlin/spring/factotry/dto/StorageDto.kt

    
    public enum APIType
    {
        GET_FACTORY_LIST,
        CONNECT_FACTORY,
        DISCONNECT_FACTORY,
        GET_STORAGE_INFO,
        GET_ORIGIN_INFO_LIST,
        GET_FACTORY_CAPITAL,
        REPORT_USAGE_ORIGIN_ACTIVELIQUID,
        REPORT_USAGE_ORIGIN_NPM,
        REPORT_USAGE_ORIGIN_NEGATIVE_ELECTRODE,
        REPORT_USAGE_ORIGIN_ELECTROLYTIC,
        REPORT_PRODUCE_PRODUCT,
        REPORT_PRODUCT_SELL,
        REPORT_PRODUCT_FAULTY
    }


    public class APIHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("인터넷 연결을 확인하세요.");
            }
        }

        public void GetFactoryList(APIType apiType, Func<ResponseFactoryListDTO, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.GET_FACTORY_LIST);
            Debug.Assert(afterFunc != null);
            StartCoroutine(FactoryListCorutine((ResponseFactoryListDTO res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator FactoryListCorutine(Action<ResponseFactoryListDTO> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/factory/list";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if(webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log("네트워크 에러");
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Assert(result != null);
                    Debug.Log(result);

                    ResponseFactoryListDTO res = JsonUtility.FromJson<ResponseFactoryListDTO>(result);
                    callback(res);
                }
            }
        }

        public void ConnectFactory(APIType apiType, string factoryName, Func<bool, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.CONNECT_FACTORY);
            Debug.Assert(afterFunc != null);
            StartCoroutine(ConnectFactoryCorutine(factoryName, (ResponseFactoryListDTO res) =>
            {
                afterFunc(res);
            }));
        }
    }

}