using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendDemoData : MonoBehaviour
{
    public static API_DTO.ResponseFactoryListDTO responseFactoryListDemo()
    {
        API_DTO.ResponseFactoryListDTO responseFactoryListDTO = new API_DTO.ResponseFactoryListDTO();
        responseFactoryListDTO.factoryList = new List<API_DTO.FactoryInfoDTO>();

        API_DTO.FactoryInfoDTO factoryInfoDTO = new API_DTO.FactoryInfoDTO();
        factoryInfoDTO.factoryId = 1;
        factoryInfoDTO.factoryName = "factory1";
        responseFactoryListDTO.factoryList.Add(factoryInfoDTO);

        factoryInfoDTO = new API_DTO.FactoryInfoDTO();
        factoryInfoDTO.factoryId = 2;
        factoryInfoDTO.factoryName = "factory2";
        responseFactoryListDTO.factoryList.Add(factoryInfoDTO);

        factoryInfoDTO = new API_DTO.FactoryInfoDTO();
        factoryInfoDTO.factoryId = 3;
        factoryInfoDTO.factoryName = "factory3";
        responseFactoryListDTO.factoryList.Add(factoryInfoDTO);

        return responseFactoryListDTO;
    }
}
