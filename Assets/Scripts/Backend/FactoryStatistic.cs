using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryStatistic : MonoBehaviour
{
    [ReadOnly] public int ProductStartCount = 0;
    [ReadOnly] public int TestSuccessCount = 0;
    [ReadOnly] public int TestFailCount = 0;
    [ReadOnly] public float UsageActiveLiquidCount = 0;
    [ReadOnly] public float UsageNPMCount = 0;
    [ReadOnly] public float UsageNegativeElectrodeCount = 0;
    [ReadOnly] public float UsageElectrolyticCount = 0;

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

    public void AddUsageActiveLiquidCount(float usage)
    {
        UsageActiveLiquidCount += usage;
    }

    public void AddUsageNPMCount(float usage)
    {
        UsageNPMCount += usage;
    }

    public void AddUsageNegativeElectrodeCount(float usage)
    {
        UsageNegativeElectrodeCount += usage;
    }

    public void AddUsageElectrolyticCount(float usage)
    {
        UsageElectrolyticCount += usage;
    }
}
