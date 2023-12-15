using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryStatistic : MonoBehaviour
{

    [ReadOnly] public int ProductID;
    [ReadOnly] public int ProductStartCount;
    [ReadOnly] public int TestSuccessCount;
    [ReadOnly] public int TestFailCount;
    [ReadOnly] public Dictionary<string, float> MaterialUsage= new Dictionary<string, float>()
    {
        { "ActiveLiquid", 0 },
        { "NMP", 0 },
        { "NegativeElectrode", 0 },
        { "Electrolytic", 0 },
    };

    private void InitFactoryStatistic(int ProductID, int ProductStartCount, int TestSuccessCount, int TestFailCount)
    {
        this.ProductID = ProductID;
        this.ProductStartCount = ProductStartCount;
        this.TestSuccessCount = TestSuccessCount;
        this.TestFailCount = TestFailCount;
    }

    public int GiveProductID()
    {
        ProductID += 1;
        return ProductID;
    }

    public void AddProductStartCount()
    {
        ProductStartCount += 1;
    }

    public void AddProductTestSuccessCount()
    {
        TestSuccessCount += 1;
    }

    public void AddProductTestFailCount()
    {
        TestFailCount += 1;
    }

    public void AccmulateActiveLiquid(FlagsAttribute )

}
