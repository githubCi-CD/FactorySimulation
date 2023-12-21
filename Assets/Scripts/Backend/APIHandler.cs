using Assets.Scripts.Config;
using System;
using System.Collections;
using System.Security.Cryptography;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
        REPORT_PRODUCT_SUCCESS,
        REPORT_PRODUCT_FAULTY,
        REPORT_STOCK_CONSUME,
        LOG_PRODUCE_PRODUCT,
        LOG_PRODUCE_FAULTY,
        GET_MAX_PRODUCT_ID
    }

    public enum LOGType
    {
        PRODUCT_START,
        PRODUCT_TEST_SUCCESS,
        PRODUCT_TEST_FAIL,
        ORIGIN_USAGE_ACTIVE_LIQUID,
        ORIGIN_USAGE_NMP,
        ORIGIN_USAGE_NEGATIVE_ELECTRODE,
        ORIGIN_USAGE_ELECTROLYTIC

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

        /// <summary>
        /// 일단 사용하지 않음
        /// </summary>
        /// <param name="apiType"></param>
        /// <param name="afterFunc"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 일단 사용하지 않음
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IEnumerator CheckConnectionCorutine(Action<bool> callback)
        {
            string host = Configration.Instance.serverHost;
            string fPort = Configration.Instance.factoryManageAPIPort;
            string rPort = Configration.Instance.resourceManageAPIPort;
            string fUrl = host + ":" + fPort;
            string rUrl = host + ":" + rPort;

            bool factoryTest = false;
            bool resourceTest = false;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(fUrl))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    factoryTest = true;
                }
            }
            using (UnityWebRequest webRequest = UnityWebRequest.Get(rUrl))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    resourceTest = true;
                }
            }
            if (factoryTest == true && resourceTest == true)
            {
                callback(true);
            }
            else
            {
                callback(false);
            }
        }

        public void GetMaxProductID(APIType apiType, long factoryId, Func<long, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.GET_MAX_PRODUCT_ID);
            Debug.Assert(afterFunc != null);

            StartCoroutine(GetMaxProductIDCorutine(factoryId, (long res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator GetMaxProductIDCorutine(long factoryId, Action<long> callback)
        {
            string host = Configration.Instance.elasticServerHost;
            string port = Configration.Instance.elasticServerPort;
            string index = Configration.Instance.elasticServerIndex;
            string url = host + ":" + port + "/" + index + "/_search";

            UnityWebRequest webRequest = new UnityWebRequest(url, "GET");
            string bodyFormat = "{\"query\": " +
                "{\"term\": {\"factoryId\": " + factoryId.ToString() 
                + "}},\"sort\": [{\"productId\": {\"order\": \"desc\"}}],\"size\": 1, \"_source\": [\"productId\"]}";
            Debug.Log(bodyFormat);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyFormat);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                SearchResponse res = JsonUtility.FromJson<SearchResponse>(webRequest.downloadHandler.text);
                if(res.hits.hits.Length == 0)
                {
                    callback(0);
                }
                Debug.Log("제품 시작 번호를 가져왔습니다. Start ProductId : " + res.hits.hits[0]._source.productId);
                callback(res.hits.hits[0]._source.productId);
            }
            else
            {
                Debug.Assert(false, "로그인 실패");
                callback(-1);
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
            string url = host + ":" + port + "/api/v1/factory";

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
                    string result = "{\"factoryList\":" + webRequest.downloadHandler.text + "}";
                    Debug.Assert(result != null);
                    Debug.Log(result);

                    ResponseFactoryListDTO dto = new ResponseFactoryListDTO();

                    dto = JsonUtility.FromJson<ResponseFactoryListDTO>(result);

                    ResponseFactoryListDTO res = JsonUtility.FromJson<ResponseFactoryListDTO>(result);
                    callback(res);
                }
            }
        }

        public void ConnectFactory(APIType apiType, string factoryName, Func<API_DTO.ResponseLoginLogoutDto, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.CONNECT_FACTORY);
            Debug.Assert(afterFunc != null);
            StartCoroutine(ConnectFactoryCorutine(factoryName, (API_DTO.ResponseLoginLogoutDto res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator ConnectFactoryCorutine(string factoryName, Action<API_DTO.ResponseLoginLogoutDto> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/login";

            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            LoginLogoutBodyFormat loginLogoutBodyFormat = new LoginLogoutBodyFormat();
            loginLogoutBodyFormat.name = factoryName;
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(loginLogoutBodyFormat));
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                API_DTO.ResponseLoginLogoutDto res = JsonUtility.FromJson<API_DTO.ResponseLoginLogoutDto>(webRequest.downloadHandler.text);
                Debug.Log("로그인에 성공했습니다. FactoryName : " + res.name);
                callback(res);
            }
            else
            {
                Debug.Assert(false, "로그인 실패");
                callback(null);
            }
        }

        public void DisconnectFactory(APIType apiType, string factoryName)
        {
            Debug.Assert(apiType == APIType.DISCONNECT_FACTORY);
            StartCoroutine(DisconnectFactoryCorutine(factoryName));
        }

        IEnumerator DisconnectFactoryCorutine(string factoryName)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/logout";

            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            LoginLogoutBodyFormat loginLogoutBodyFormat = new LoginLogoutBodyFormat();
            loginLogoutBodyFormat.name = factoryName;
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(loginLogoutBodyFormat));
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if(webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("로그아웃 완료");
            }
            else
            {
                Debug.Log("로그아웃 실패");
            }

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
        }

        public void GetStorageSpecificOrigin(APIType apiType, long factoryID, long originId, Func<StorageOrigin, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.GET_STORAGE_INFO);
            Debug.Assert(afterFunc != null);
            StartCoroutine(GetStorageSpecificOriginCorutine(factoryID, originId, (StorageOrigin res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator GetStorageSpecificOriginCorutine(long factoryID, long originId, Action<StorageOrigin> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.resourceManageAPIPort;
            string url = host + ":" + port + "/api/v1/storage/findOne?factoryId=" + factoryID.ToString() + "?originId=" + originId.ToString();
            Debug.Log(url);

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Log(result);

                    StorageOrigin res = JsonUtility.FromJson<StorageOrigin>(result);
                    callback(res);
                }
                else
                {
                    callback(null);
                    Debug.Log("네트워크 에러");
                }
            }
        }

        public void GetStorageInfo(APIType apiType, long factoryID, Func<GetStorageInfoDTO, bool> afterFunc)
        {
            Debug.Assert(apiType == APIType.GET_STORAGE_INFO);
            Debug.Assert(afterFunc != null);
            StartCoroutine(GetStorageInfoCorutine(factoryID, (GetStorageInfoDTO res) =>
            {
                afterFunc(res);
            }));
        }

        IEnumerator GetStorageInfoCorutine(long factoryID, Action<GetStorageInfoDTO> callback)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.resourceManageAPIPort;
            string url = host + ":" + port + "/api/v1/storage?factoryId=" + factoryID.ToString();
            Debug.Log(url);

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string result = "{\"storageList\":" + webRequest.downloadHandler.text + "}";
                    Debug.Assert(result != null);
                    Debug.Log(result);

                    GetStorageInfoDTO res = JsonUtility.FromJson<GetStorageInfoDTO>(result);
                    callback(res);
                }
                else
                {
                    callback(null);
                    Debug.Log("네트워크 에러");
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

        public void ProductSuccess(APIType apiType, long factoryId)
        {
            Debug.Assert(apiType == APIType.REPORT_PRODUCT_SUCCESS);
            StartCoroutine(FactoryProductSuccessCorutine(factoryId));
        }

        IEnumerator FactoryProductSuccessCorutine(long factoryId)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/api/v1/factory/"+ factoryId.ToString() + "/success";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("물건 팔기 성공");
                }
                else
                {
                    Debug.Log("Network Error");
                }
            }
        }

        public void ProductFailure(APIType apiType, long factoryId)
        {
            Debug.Assert(apiType == APIType.REPORT_PRODUCT_FAULTY);
            StartCoroutine(FactoryProductFailureCorutine(factoryId));
        }

        IEnumerator FactoryProductFailureCorutine(long factoryId)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/api/v1/factory/" + factoryId.ToString() + "/failure";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("물건 폐기 성공");
                }
                else
                {
                    Debug.Log("Network Error");
                }
            }
        }

        public void ReportStockConsume(APIType apiType, long factoryId, long OriginId, int usage)
        {
            Debug.Assert(apiType == APIType.REPORT_STOCK_CONSUME);
            StartCoroutine(ReportStockConsumeCorutine(factoryId, OriginId, usage));
        }

        IEnumerator ReportStockConsumeCorutine(long factoryId, long OriginId, int usage)
        {
            string host = Configration.Instance.serverHost;
            string port = Configration.Instance.factoryManageAPIPort;
            string url = host + ":" + port + "/api/v1/factory/"+factoryId + "/useOrigin";


            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            ReportStockConsumeBodyFormat reportStockConsumeBodyFormat = new ReportStockConsumeBodyFormat()
            {
                id = OriginId,
                count = usage
            };
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(reportStockConsumeBodyFormat));
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("원자재 사용에 성공했습니다.");
            }
            else
            {
                Debug.Assert(false, "로그아웃 실패");
            }

        }

        public void LogProduceProduct(LOGType logType, Guid guid, long factoryId, long productId)
        {
            Debug.Assert(logType == LOGType.PRODUCT_START);
            LogDTO dto = new LogDTO();
            dto.uuid = guid.ToString();
            dto.factoryId = factoryId;
            dto.productId = productId;
            dto.logType = LOGType.PRODUCT_START.ToString();
            DateTime dateTime = DateTime.Now;
            dto.time = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            dto.status = "null";
            StartCoroutine(SendLogCorutine(dto));
        }

        public void LogProductFaulty(LOGType logType, Guid guid, long factoryId, long productId, string where)
        {
            Debug.Assert(logType == LOGType.PRODUCT_TEST_FAIL);
            LogDTO dto = new LogDTO();
            dto.uuid = guid.ToString();
            dto.factoryId = factoryId;
            dto.productId = productId;
            dto.logType = LOGType.PRODUCT_TEST_FAIL.ToString();
            DateTime dateTime = DateTime.Now;
            dto.time = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            dto.status = where;
            StartCoroutine(SendLogCorutine(dto));
        }

        public void LogProduceSuccess(LOGType logType, Guid guid, long factoryId, long productId)
        {
            Debug.Assert(logType == LOGType.PRODUCT_TEST_SUCCESS);
            LogDTO dto = new LogDTO();
            dto.uuid = guid.ToString();
            dto.factoryId = factoryId;
            dto.productId = productId;
            dto.logType = LOGType.PRODUCT_TEST_SUCCESS.ToString();
            DateTime dateTime = DateTime.Now;
            dto.time = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            dto.status = "null";
            StartCoroutine(SendLogCorutine(dto));
        }

        public void OriginUsageMaterial(LOGType logType, IngredientType ingredient , Guid guid, long factoryId, long productId, String consumeCount)
        {
            LogDTO dto = new LogDTO();
            dto.uuid = guid.ToString();
            dto.factoryId = factoryId;
            dto.productId = productId;
            if(ingredient == IngredientType.ACTIVE_LIQUID)
            {
                Debug.Assert(logType == LOGType.ORIGIN_USAGE_ACTIVE_LIQUID);
                dto.logType = LOGType.ORIGIN_USAGE_ACTIVE_LIQUID.ToString();
            }
            else if (ingredient == IngredientType.NMP)
            {
                Debug.Assert(logType == LOGType.ORIGIN_USAGE_NMP);
                dto.logType = LOGType.ORIGIN_USAGE_NMP.ToString();
            }
            else if (ingredient == IngredientType.NEGATIVE_ELECTRODE)
            {
                Debug.Assert(logType == LOGType.ORIGIN_USAGE_NEGATIVE_ELECTRODE);
                dto.logType = LOGType.ORIGIN_USAGE_NEGATIVE_ELECTRODE.ToString();
            }
            else if (ingredient == IngredientType.ELECTROLYTIC)
            {
                Debug.Assert(logType == LOGType.ORIGIN_USAGE_ELECTROLYTIC);
                dto.logType = LOGType.ORIGIN_USAGE_ELECTROLYTIC.ToString();
            }
            DateTime dateTime = DateTime.Now;
            dto.time = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            dto.status = consumeCount;
            StartCoroutine(SendLogCorutine(dto));
        }

        IEnumerator SendLogCorutine(LogDTO dto)
        {
            string host = Configration.Instance.elasticServerHost;
            string port = Configration.Instance.logstashServerPort;
            string url = host + ":" + port;

            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(dto));
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("로그기록 성공" + dto.logType);
            }
            else
            {
                Debug.Assert(false, "로그기록 실패");
            }

        }
    }

}