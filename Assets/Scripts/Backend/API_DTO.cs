using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class API_DTO : MonoBehaviour
{

    //GET_FACTORY_LIST,
    //CONNECT_FACTORY,
    //DISCONNECT_FACTORY,
    //GET_STORAGE_INFO,
    //GET_ORIGIN_INFO_LIST,
    //GET_FACTORY_CAPITAL,
    //REPORT_USAGE_ORIGIN_ACTIVELIQUID,
    //REPORT_USAGE_ORIGIN_NPM,
    //REPORT_USAGE_ORIGIN_NEGATIVE_ELECTRODE,
    //REPORT_USAGE_ORIGIN_ELECTROLYTIC,
    //REPORT_PRODUCE_PRODUCT,
    //REPORT_PRODUCT_SELL,
    //REPORT_PRODUCT_FAULTY

    /// <summary>
    /// GET_FACTORY_LIST «œ¿ß ∞¥√º
    /// </summary>
    [Serializable]
    public class FactoryInfoDTO
    {
        public long factoryId;
        public string factoryName;
    }

    /// <summary>
    /// GET_FACTORY_LIST DTO
    /// </summary>
    [Serializable]
    public class ResponseFactoryListDTO
    {
        public List<FactoryInfoDTO> factoryList;
    }

    [Serializable]
    public class ConnectFactoryDTO
    {
        public long factoryID;
        public s
    }
}
