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

    private void Start()
    {
        // nowInventory에 Init Inventory 복사
        foreach (KeyValuePair<string,float> item in Configration.Instance.init_Inventory)
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
        UpdateResourceObject();
        StartCoroutine(InputResourceCoroutine());
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
        //새로운 ProductObject인스턴스 생성
        GameObject newProductObject = Instantiate(ProductObject);
        //newProductObject를 HeiaraycyProductList 하위에 이동
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
