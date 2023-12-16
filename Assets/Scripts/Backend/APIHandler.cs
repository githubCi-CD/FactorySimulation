using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
        CHECK_CONNECTION,
        GET_FACTORY_LIST,
        CONNECT_FACTORY,
        DISCONNECT_FACTORY,
        GET_STORAGE_INFO,
        GET_ORIGIN_INFO,
        GET_FACTORY_CAPITAL,
        REPORT_STOCK_CONSUME,
        LOG_PRODUCE_PRODUCT,
        REPORT_PRODUCT_SELL,
        LOG_PRODUCT_FAULTY
    }


    public class APIHandler : MonoBehaviour
    {
        private static APIHandler instance;
        private void Awake()
        {
            if (null == instance)
            {
                instance = this;

                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public static APIHandler Instance
        {
            get
            {
                if (null == instance)
                {
                    return null;
                }
                return instance;
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("인터넷 연결을 확인하세요.");
            }
        }
        public bool CheckConnection(APIType apiType, Func<bool, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.CHECK_CONNECTION);
            Debug.Assert(afterFunc != null);
            bool serverConnTest = true;
            StartCoroutine(CheckConnectionCorutine((bool res) =>
            {
                afterFunc(res);
                serverConnTest = res;
            }));
            return serverConnTest;
        }

        IEnumerator CheckConnectionCorutine(Action<bool> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/factory/list";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    callback(false);
                }
                else
                {
                    callback(true);
                }
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

            bool serverTest = true;
            yield return StartCoroutine(CheckConnectionCorutine((bool res) => { 
                if(res == false)
                {
                    callback(null);
                    serverTest = false;
                    return;
                }
            }));
            if(serverTest == false)
            {
                yield break;
            }

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

        public void ConnectFactory(APIType apiType, string factoryName, Func<ConnectFactoryDTO, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.CONNECT_FACTORY);
            Debug.Assert(afterFunc != null);
            StartCoroutine(ConnectFactoryCorutine(factoryName, (ConnectFactoryDTO res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator ConnectFactoryCorutine(string factoryName, Action<ConnectFactoryDTO> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/factory/connect?factoryName=" + factoryName;


            bool serverTest = true;
            yield return StartCoroutine(CheckConnectionCorutine((bool res) =>
            {
                if (res == false)
                {
                    callback(null);
                    serverTest = false;
                    return;
                }
            }));
            if (serverTest == false)
            {
                yield break;
            }
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log("네트워크 에러");
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Assert(result != null);
                    Debug.Log(result);

                    ConnectFactoryDTO res = JsonUtility.FromJson<ConnectFactoryDTO>(result);
                    callback(res);
                }
            }
        }

        public void GetStorageInfo(APIType apiType, long factoryID, long originID , Func<GetStorageInfoDTO, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.GET_STORAGE_INFO);
            Debug.Assert(afterFunc != null);
            StartCoroutine(GetStorageInfoCorutine(factoryID, originID, (GetStorageInfoDTO res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator GetStorageInfoCorutine(long factoryID, long originID, Action<GetStorageInfoDTO> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/factory/connect?factoryName=" + factoryID.ToString() + "&originID=" + originID;

            bool serverTest = true;
            yield return StartCoroutine(CheckConnectionCorutine((bool res) =>
            {
                if (res == false)
                {
                    callback(null);
                    serverTest = false;
                    return;
                }
            }));
            if (serverTest == false)
            {
                yield break;
            }

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log("네트워크 에러");
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Assert(result != null);
                    Debug.Log(result);

                    GetStorageInfoDTO res = JsonUtility.FromJson<GetStorageInfoDTO>(result);
                    callback(res);
                }
            }
        }

        public void GetOriginInfo(APIType apiType, string factoryName, Func<GetOriginInfoDTO, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.GET_ORIGIN_INFO);
            Debug.Assert(afterFunc != null);
            StartCoroutine(GetOriginInfoCorutine(factoryName, (GetOriginInfoDTO res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator GetOriginInfoCorutine(string factoryName, Action<GetOriginInfoDTO> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/factory/connect?factoryName=" + factoryName;

            bool serverTest = true;
            yield return StartCoroutine(CheckConnectionCorutine((bool res) =>
            {
                if (res == false)
                {
                    callback(null);
                    serverTest = false;
                    return;
                }
            }));
            if (serverTest == false)
            {
                yield break;
            }

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log("네트워크 에러");
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Assert(result != null);
                    Debug.Log(result);

                    GetOriginInfoDTO res = JsonUtility.FromJson<GetOriginInfoDTO>(result);
                    callback(res);
                }
            }
        }

        public void GetFactoryCapital(APIType apiType, string factoryName, Func<GetFactoryCapitalDTO, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.GET_FACTORY_CAPITAL);
            Debug.Assert(afterFunc != null);
            StartCoroutine(GetFactoryCapitalCorutine(factoryName, (GetFactoryCapitalDTO res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator GetFactoryCapitalCorutine(string factoryName, Action<GetFactoryCapitalDTO> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/factory/connect?factoryName=" + factoryName;

            bool serverTest = true;
            yield return StartCoroutine(CheckConnectionCorutine((bool res) =>
            {
                if (res == false)
                {
                    callback(null);
                    serverTest = false;
                    return;
                }
            }));
            if (serverTest == false)
            {
                yield break;
            }

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log("네트워크 에러");
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Assert(result != null);
                    Debug.Log(result);

                    GetFactoryCapitalDTO res = JsonUtility.FromJson<GetFactoryCapitalDTO>(result);
                    callback(res);
                }
            }
        }

        public void ReportStockConsume(APIType apiType, string factoryName, Func<ReportStockConsumeDTO, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.REPORT_STOCK_CONSUME);
            Debug.Assert(afterFunc != null);
            StartCoroutine(ReportStockConsumeCorutine(factoryName, (ReportStockConsumeDTO res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator ReportStockConsumeCorutine(string factoryName, Action<ReportStockConsumeDTO> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/factory/connect?factoryName=" + factoryName;

            bool serverTest = true;
            yield return StartCoroutine(CheckConnectionCorutine((bool res) =>
            {
                if (res == false)
                {
                    callback(null);
                    serverTest = false;
                    return;
                }
            }));
            if (serverTest == false)
            {
                yield break;
            }

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log("네트워크 에러");
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Assert(result != null);
                    Debug.Log(result);

                    ReportStockConsumeDTO res = JsonUtility.FromJson<ReportStockConsumeDTO>(result);
                    callback(res);
                }
            }
        }

        public void ReportProductSell(APIType apiType, string factoryName, Func<ReportProductSellDTO, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.REPORT_PRODUCT_SELL);
            Debug.Assert(afterFunc != null);
            StartCoroutine(ReportProductSellCorutine(factoryName, (ReportProductSellDTO res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator ReportProductSellCorutine(string factoryName, Action<ReportProductSellDTO> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/factory/connect?factoryName=" + factoryName;

            bool serverTest = true;
            yield return StartCoroutine(CheckConnectionCorutine((bool res) =>
            {
                if (res == false)
                {
                    callback(null);
                    serverTest = false;
                    return;
                }
            }));
            if (serverTest == false)
            {
                yield break;
            }

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log("네트워크 에러");
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Assert(result != null);
                    Debug.Log(result);

                    ReportProductSellDTO res = JsonUtility.FromJson<ReportProductSellDTO>(result);
                    callback(res);
                }
            }
        }

        public void LogProduceProductDTO(APIType apiType, string factoryName)
        {
            Debug.Assert(apiType == APIType.REPORT_PRODUCT_SELL);
            LogProduceProductDTO dto = new LogProduceProductDTO();
        }

        public void LogProductFaultyDTO(APIType apiType, string factoryName)
        {
            Debug.Assert(apiType == APIType.REPORT_PRODUCT_SELL);
            LogProductFaultyDTO dTO = new LogProductFaultyDTO();
        }
    }

}