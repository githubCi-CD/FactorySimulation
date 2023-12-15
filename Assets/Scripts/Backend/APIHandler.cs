using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Asset.Script.Backend
{
    public enum APIType
    {
        GET_FACTORY_LIST,
        BUILD_NEW_FACTORY,
    }


    public class APIHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("���ͳ� ������ Ȯ���ϼ���.");
            }
        }

        public T APIRequest<T>(APIType.)
        {
            return default(T);
        }
    }

}