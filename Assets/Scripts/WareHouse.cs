using Asset.Script.Backend;
using Assets;
using Assets.Scripts;
using Assets.Scripts.Config;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static API_DTO;

public class WareHouse : SerializedMonoBehaviour
{
    public ConveyorSystem StartConvey;
    public GameObject ProductObject;
    public GameObject HeiaraycyProductList;
    public Machine FirstInputMachine;

    public Dictionary<string, GameObject> LiquidResourceObject = new Dictionary<string, GameObject>();
    public GameObject[] NegativeElectrodeBox;
    [ReadOnly] public Dictionary<string, float> nowInventory = new Dictionary<string, float>();
    [ReadOnly] public Factory factory;

    private void Start()
    {
        factory = GameObject.FindWithTag("Factory").GetComponent<Factory>();
        // nowInventory¿¡ Init Inventory º¹»ç
        foreach (KeyValuePair<string, float> item in Configration.Instance.init_Inventory)
        {
            if (nowInventory.ContainsKey(item.Key))
            {
                if (nowInventory[item.Key] != item.Value)
                {
                    nowInventory[item.Key] = item.Value;
                }
            }
            else
            {
                nowInventory.Add(item.Key, item.Value);
            }
        }
        if (Configration.Instance.standAloneMode == false)
        {
            long activeLiquidID = Configration.Instance.OriginNameToID[IngredientType.ACTIVE_LIQUID.ToString()];
            long nmpID = Configration.Instance.OriginNameToID[IngredientType.NMP.ToString()];
            long negativeElectrodeID = Configration.Instance.OriginNameToID[IngredientType.NEGATIVE_ELECTRODE.ToString()];
            long electrolytiIDc = Configration.Instance.OriginNameToID[IngredientType.ELECTROLYTIC.ToString()];
            APIHandler.Instance.GetStorageInfo(APIType.GET_STORAGE_INFO, factory.GetFactoryId(), ApplyServerRes);
        }


        UpdateResourceObject();
        StartCoroutine(InputResourceCoroutine());
        StartCoroutine(PeriodicStorageUpdate());
    }
    public bool ApplyServerRes(API_DTO.GetStorageInfoDTO res)
    {
        if(res == null)
        {
            SetZeroNowInventory();
            return false;
        }

        long activeLiquidID = Configration.Instance.OriginNameToID[IngredientType.ACTIVE_LIQUID.ToString()];
        long nmpID = Configration.Instance.OriginNameToID[IngredientType.NMP.ToString()];
        long negativeElectrodeID = Configration.Instance.OriginNameToID[IngredientType.NEGATIVE_ELECTRODE.ToString()];
        long electrolytiIDc = Configration.Instance.OriginNameToID[IngredientType.ELECTROLYTIC.ToString()];

        foreach (StorageOrigin stock in res.storageList)
        {
            Debug.Assert(stock.factoryId == factory.GetFactoryId());
            if (stock.origin.id == activeLiquidID)
            {
                nowInventory[IngredientType.ACTIVE_LIQUID.ToString()] = stock.count;
            }
            else if (stock.origin.id == nmpID)
            {
                nowInventory[IngredientType.NMP.ToString()] = stock.count;
            }
            else if (stock.origin.id == negativeElectrodeID)
            {
                nowInventory[IngredientType.NEGATIVE_ELECTRODE.ToString()] = stock.count;
            }
            else if (stock.origin.id == electrolytiIDc)
            {
                nowInventory[IngredientType.ELECTROLYTIC.ToString()] = stock.count;
            }
        }
        UpdateResourceObject();
        return true;
    }

    public void SetZeroNowInventory()
    {
          foreach (KeyValuePair<string, float> item in Configration.Instance.init_Inventory)
        {
            if (nowInventory.ContainsKey(item.Key))
            {
                if (nowInventory[item.Key] != 0)
                {
                    nowInventory[item.Key] = 0;
                }
            }
            else
            {
                nowInventory.Add(item.Key, 0);
            }
        }   
    }

    public void InputResource()
    {
        GameObject newProductObject = Instantiate(ProductObject);
        newProductObject.transform.SetParent(HeiaraycyProductList.transform);
        Product newProduct = newProductObject.GetComponent<Product>();
        newProduct.giveId(factory.IssuanceID());
        factory.StatisticArchive(statisticType.PRODUCT_START_COUNT);
        APIHandler.Instance.LogProduceProduct(LOGType.PRODUCT_START, newProduct.GetProductGuid(), factory.GetFactoryId(), newProduct.GetProductId());
        StartConvey.ComeIntoBelt(newProductObject.GetComponent<Product>());
    }

    IEnumerator InputResourceCoroutine()
    {
        while (true)
        {
            if(FirstInputMachine.GetMachineStatus() == MachineStatus.WAITING)
            {
                InputResource();
            }
            yield return new WaitForSeconds(Configration.Instance.inputInterval);
        }
    }

    public void UpdateResourceObject()
    {
        foreach (KeyValuePair<string,GameObject> item in LiquidResourceObject)
        {
            item.Value.GetComponent<LiquidTank>().UpdateLiquidTank(Configration.Instance.max_Inventory[item.Key], nowInventory[item.Key]);
        }

        int maxElect = (int)Configration.Instance.max_Inventory[IngredientType.NEGATIVE_ELECTRODE.ToString()];
        int nowElect = (int)nowInventory[IngredientType.NEGATIVE_ELECTRODE.ToString()];
        float percent = ((float)nowElect / (float)maxElect) * 100;
        int boxCount = ((int)percent) / 20;
        int idx = 0;
        foreach (GameObject item in NegativeElectrodeBox)
        {
            if(idx < boxCount)
            {
                item.SetActive(true);
            }
            else
            {
                item.SetActive(false);
            }
            idx += 1;
        }
    }

    public bool CheckMixCoatingResource()
    {

        string activeLiquid = IngredientType.ACTIVE_LIQUID.ToString();
        string nmp = IngredientType.NMP.ToString();
        float activeLiquidUsage = Configration.Instance.mixCoatingInputResource[activeLiquid];
        float nmpUsage = Configration.Instance.mixCoatingInputResource[nmp];

        if (nowInventory[IngredientType.ACTIVE_LIQUID.ToString()] < activeLiquidUsage)
            return false;

        if (nowInventory[IngredientType.NMP.ToString()] < nmpUsage)
            return false;
        return true;
    }

    public bool CheckPressResource()
    {
        string negativeElectrode = IngredientType.NEGATIVE_ELECTRODE.ToString();
        float negativeElectrodeUsage = Configration.Instance.pressingInputResource[negativeElectrode];

        if (nowInventory[IngredientType.NEGATIVE_ELECTRODE.ToString()] < negativeElectrodeUsage)
            return false;
        return true;
    }

    public bool CheckStackingResource()
    {
        string electrolytic = IngredientType.ELECTROLYTIC.ToString();
        float electrolyticUsage = Configration.Instance.stackingInputResource[electrolytic];

        if (nowInventory[IngredientType.ELECTROLYTIC.ToString()] < electrolyticUsage)
            return false;
        return true;
    }


    public void UsingMixCoatingResource(Product product)
    {
        Debug.Assert(product != null);

        string activeLiquid = IngredientType.ACTIVE_LIQUID.ToString();
        long activeLiquidID = Configration.Instance.OriginNameToID[activeLiquid];
        string nmp = IngredientType.NMP.ToString();
        long nmpID = Configration.Instance.OriginNameToID[nmp];
        float activeLiquidUsage = Configration.Instance.mixCoatingInputResource[activeLiquid];
        float nmpUsage = Configration.Instance.mixCoatingInputResource[nmp];

        factory.StatisticArchive(statisticType.MATERIAL_USAGE_ACTIVE_LIQUID, activeLiquidUsage);
        factory.StatisticArchive(statisticType.MATERIAL_USAGE_NMP, nmpUsage);
        APIHandler.Instance.OriginUsageMaterial(LOGType.ORIGIN_USAGE_ACTIVE_LIQUID, IngredientType.ACTIVE_LIQUID, product.GetProductGuid(), factory.GetFactoryId(), product.GetProductId(), activeLiquidUsage.ToString());
        APIHandler.Instance.OriginUsageMaterial(LOGType.ORIGIN_USAGE_NMP, IngredientType.NMP, product.GetProductGuid(), factory.GetFactoryId(), product.GetProductId(), nmpUsage.ToString());

        if (Configration.Instance.standAloneMode == true)
        {
            nowInventory[IngredientType.ACTIVE_LIQUID.ToString()] -= activeLiquidUsage;
            nowInventory[IngredientType.NMP.ToString()] -= nmpUsage;
        }
        else
        {
            APIHandler.Instance.ReportStockConsume(APIType.REPORT_STOCK_CONSUME, factory.GetFactoryId(), activeLiquidID, (int)activeLiquidUsage);
            APIHandler.Instance.ReportStockConsume(APIType.REPORT_STOCK_CONSUME, factory.GetFactoryId(), nmpID, (int)nmpUsage);
        }

        UpdateResourceObject();
        return;
    }

    public void UsingPressingResource(Product product)
    {
        Debug.Assert(product != null);

        string negativeElectrode = IngredientType.NEGATIVE_ELECTRODE.ToString();
        long negativeElectrodeID = Configration.Instance.OriginNameToID[negativeElectrode];
        float negativeElectrodeUsage = Configration.Instance.pressingInputResource[negativeElectrode];

        factory.StatisticArchive(statisticType.MATERIAL_USAGE_NEGATIVE_ELECTRODE, negativeElectrodeUsage);
        APIHandler.Instance.OriginUsageMaterial(LOGType.ORIGIN_USAGE_NEGATIVE_ELECTRODE, IngredientType.NEGATIVE_ELECTRODE, product.GetProductGuid(), factory.GetFactoryId(), product.GetProductId(), negativeElectrodeUsage.ToString());

        if (Configration.Instance.standAloneMode == true)
        {
            nowInventory[IngredientType.NEGATIVE_ELECTRODE.ToString()] -= negativeElectrodeUsage;
        }
        else
        {
            APIHandler.Instance.ReportStockConsume(APIType.REPORT_STOCK_CONSUME, factory.GetFactoryId(), negativeElectrodeID, (int)negativeElectrodeUsage);
        }

        UpdateResourceObject();
        return;
    }

    public void UsingStackingResource(Product product)
    {
        Debug.Assert(product != null);

        string electrolytic = IngredientType.ELECTROLYTIC.ToString();
        long electrolyticID = Configration.Instance.OriginNameToID[electrolytic];
        float electrolyticUsage = Configration.Instance.stackingInputResource[electrolytic];

        factory.StatisticArchive(statisticType.MATERIAL_USAGE_ELECTROLYTIC, electrolyticUsage);
        APIHandler.Instance.OriginUsageMaterial(LOGType.ORIGIN_USAGE_ELECTROLYTIC, IngredientType.ELECTROLYTIC, product.GetProductGuid(), factory.GetFactoryId(), product.GetProductId(), electrolyticUsage.ToString());

        if (Configration.Instance.standAloneMode == true)
        {
            nowInventory[IngredientType.ELECTROLYTIC.ToString()] -= electrolyticUsage;
        }
        else
        {
            APIHandler.Instance.ReportStockConsume(APIType.REPORT_STOCK_CONSUME, factory.GetFactoryId(), electrolyticID, (int)electrolyticUsage);
        }

        UpdateResourceObject();
        return;
    }

    IEnumerator PeriodicStorageUpdate()
    {
        while(true)
        {
             APIHandler.Instance.GetStorageInfo(APIType.GET_STORAGE_INFO, factory.GetFactoryId(), ApplyServerRes);
            yield return new WaitForSeconds(5.0f);
        }
    }
}
