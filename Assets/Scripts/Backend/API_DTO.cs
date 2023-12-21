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
    /// GET_FACTORY_LIST ÇÏÀ§ °´Ã¼
    /// </summary>
    [Serializable]
    public class FactoryInfoDTO
    {
        public long id;
        public string name;
        public long income;
        public long outcome;
        public long asset;
        public long totalCount;
        public long successCount;
        public bool status;
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

    [Serializable]
    public class LoginLogoutBodyFormat
    {
        public string name;
    }

    [Serializable]
    public class ResponseLoginLogoutDto
    {
        public long id;
        public string name;
    }
    [Serializable]
    public class originInfo
    {
        public long id;
        public string name;
        public long price;
    }

    [Serializable]
    public class StorageOrigin
    {
        public long id;
        public long count;
        public long factoryId;
        public originInfo origin;
    }

    ///<summary>
    /// GET_STORAGE_INFO
    /// </summary>
    [Serializable]
    public class GetStorageInfoDTO
    {
        public List<StorageOrigin> storageList;
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
    /// REPORT_STOCK_CONSUME
    /// </summary>
    [Serializable]
    public class ReportStockConsumeBodyFormat
    {
        public long id;
        public int count;
    }

    ///<summary>
    /// REPORT_PRODUCT_SELL
    /// </summary>
    [Serializable]
    public class ReportProductSellDTO
    {
        public long factoryId;
        public long ProductId;
        public int result;
    }

    ///<summary>
    /// LOG_PRODUCT_FAULTY
    /// </summary>
    [Serializable]
    public class LogDTO
    {
        public string uuid;
        public long factoryId;
        public long productId;
        public string logType;
        public string time;
        public string status;
    }

    [Serializable]
    public class SearchResponse
    {
        public int took;
        public bool timed_out;
        public Shards _shards;
        public Hits hits;
    }

    [Serializable]
    public class Shards
    {
        public int total;
        public int successful;
        public int skipped;
        public int failed;
    }

    [Serializable]
    public class Hits
    {
        public Total total;
        public double max_score;
        public Hit[] hits;
    }

    [Serializable]
    public class Total
    {
        public int value;
        public string relation;
    }

    [Serializable]
    public class Hit
    {
        public string _index;
        public string _id;
        public double _score;
        public Source _source;
        public int[] sort;
    }

    [Serializable]
    public class Source
    {
        public int productId;
    }
}
