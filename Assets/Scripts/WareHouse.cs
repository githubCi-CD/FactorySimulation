using Asset.Script.Backend;
using Assets;
using Assets.Scripts;
using Assets.Scripts.Config;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WareHouse : SerializedMonoBehaviour
{
    public ConveyorSystem StartConvey;
    public GameObject ProductObject;
    public GameObject HeiaraycyProductList;

    public Dictionary<string, GameObject> LiquidResourceObject = new Dictionary<string, GameObject>();
    public GameObject[] NegativeElectrodeBox;
    [ReadOnly] public Dictionary<string, float> nowInventory = new Dictionary<string, float>();
    [ReadOnly] public Factory factory;

    private void Start()
    {
        factory = GameObject.FindWithTag("Factory").GetComponent<Factory>();
        // nowInventory�� Init Inventory ����
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
            APIHandler.Instance.GetStorageInfo(APIType.GET_STORAGE_INFO, factory.GetFactoryId(), 1, ApplyServerRes);
        }
        if (Configration.Instance.standAloneMode == false)
        {
            long activeLiquidID = Configration.Instance.OriginNameToID[IngredientType.ACTIVE_LIQUID.ToString()];
            long nmpID = Configration.Instance.OriginNameToID[IngredientType.NMP.ToString()];
            long negativeElectrodeID = Configration.Instance.OriginNameToID[IngredientType.NEGATIVE_ELECTRODE.ToString()];
            long electrolytiIDc = Configration.Instance.OriginNameToID[IngredientType.ELECTROLYTIC.ToString()];
            APIHandler.Instance.GetStorageInfo(APIType.GET_STORAGE_INFO, factory.GetFactoryId(), activeLiquidID, ApplyServerRes);
        }


        UpdateResourceObject();
        StartCoroutine(InputResourceCoroutine());
    }

    public bool ApplyServerRes(API_DTO.GetStorageInfoDTO res)
    {
        Debug.Assert(res.factoryId == factory.FactoryId);
        long resOrigin = res.originId;

        long activeLiquidID = Configration.Instance.OriginNameToID[IngredientType.ACTIVE_LIQUID.ToString()];
        long nmpID = Configration.Instance.OriginNameToID[IngredientType.NMP.ToString()];
        long negativeElectrodeID = Configration.Instance.OriginNameToID[IngredientType.NEGATIVE_ELECTRODE.ToString()];
        long electrolytiIDc = Configration.Instance.OriginNameToID[IngredientType.ELECTROLYTIC.ToString()];
        if(resOrigin == activeLiquidID)
        {
            nowInventory[IngredientType.ACTIVE_LIQUID.ToString()] = res.count;
        }else if(resOrigin == nmpID)
        {
            nowInventory[IngredientType.NMP.ToString()] = res.count;
        }else if(resOrigin == negativeElectrodeID)
        {
            nowInventory[IngredientType.NEGATIVE_ELECTRODE.ToString()] = res.count;
        }else if(resOrigin == electrolytiIDc)
        {
            nowInventory[IngredientType.ELECTROLYTIC.ToString()] = res.count;
        }
        UpdateResourceObject();
        return true;
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
            if(idx <= boxCount)
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

    public void InputResource()
    {
        //���ο� ProductObject�ν��Ͻ� ����
        GameObject newProductObject = Instantiate(ProductObject);
        //newProductObject�� HeiaraycyProductList ������ �̵�
        newProductObject.transform.SetParent(HeiaraycyProductList.transform);
        StartConvey.ComeIntoBelt(newProductObject.GetComponent<Product>());
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
        string nmp = IngredientType.NMP.ToString();
        float activeLiquidUsage = Configration.Instance.mixCoatingInputResource[activeLiquid];
        float nmpUsage = Configration.Instance.mixCoatingInputResource[nmp];

        nowInventory[IngredientType.ACTIVE_LIQUID.ToString()] -= activeLiquidUsage;
        nowInventory[IngredientType.NMP.ToString()] -= nmpUsage;
        UpdateResourceObject();
        return;
    }

    public void UsingPressingResource(Product product)
    {
        Debug.Assert(product != null);

        string negativeElectrode = IngredientType.NEGATIVE_ELECTRODE.ToString();
        float negativeElectrodeUsage = Configration.Instance.pressingInputResource[negativeElectrode];

        nowInventory[IngredientType.NEGATIVE_ELECTRODE.ToString()] -= negativeElectrodeUsage;
        UpdateResourceObject();
        return;
    }

    public void UsingStackingResource(Product product)
    {
        Debug.Assert(product != null);

        string electrolytic = IngredientType.ELECTROLYTIC.ToString();
        float electrolyticUsage = Configration.Instance.pressingInputResource[electrolytic];

        nowInventory[IngredientType.ELECTROLYTIC.ToString()] -= electrolyticUsage;
        UpdateResourceObject();
        return;
    }

    IEnumerator InputResourceCoroutine()
    {
        while (true)
        {
            InputResource();
            yield return new WaitForSeconds(Configration.Instance.inputInterval);
        }
    }
}
