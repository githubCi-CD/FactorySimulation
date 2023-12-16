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
    //REPORT_STOCK_CONSUME,
    //LOG_PRODUCE_PRODUCT,
    //REPORT_PRODUCT_SELL,
    //LOG_PRODUCT_FAULTY

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

    /// <summary>
    /// CONNECT_FACTORY
    /// </summary>
    [Serializable]
    public class ConnectFactoryDTO
    {
        public long id; // factoryId
        public string name;
        public long income;
        public long outcome;
        public long asset;
        public int totalCount;
        public int successCount;
        public bool status;
    }

    ///<summary>
    /// GET_STORAGE_INFO
    /// </summary>
    [Serializable]
    public class GetStorageInfoDTO
    {
        public long factoryId;
        public long originId;
        public int count;
    }

    ///<summary>
    /// GET_ORIGIN_INFO
    /// </summary>
    [Serializable]
    public class GetOriginInfoDTO
    {
        public long id; // factoryId
        public string name;
        public long price;
    }

    ///<summary>
    /// GET_FACTORY_CAPITAL
    /// </summary>
    [Serializable]
    public class GetFactoryCapitalDTO
    {
        public long id; // factoryId
        public string name;
        public long price;
    }

    ///<summary>
    /// REPORT_STOCK_CONSUME
    /// </summary>
    [Serializable]
    public class ReportStockConsumeDTO
    {
        public long factoryId;
        public int count;
        public long originId;
    }

    ///<summary>
    /// REPORT_PRODUCT_SELL
    /// </summary>
    [Serializable]
    public class ReportProductSellDTO
    {
        public long factoryId;
        public long productId;
        public int count;
    }

    ///<summary>
    /// LOG_PRODUCE_PRODUCT
    /// </summary>
    [Serializable]
    public class LogProduceProductDTO
    {
        public long factoryId;
        public long productId;
    }

    ///<summary>
    /// LOG_PRODUCT_FAULTY
    /// </summary>
    [Serializable]
    public class LogProductFaultyDTO
    {
        public long factoryId;
        public long productId;
        public int faultyProcess;
    }
}
