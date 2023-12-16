using Assets.Scripts;
using Assets.Scripts.Config;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CCTVInfoText : MonoBehaviour
{
    string InfoDisplay1 = "�ڿ� ��� �ܷ�" +
        "\n 1.Ȱ���� : {0}L  ({1})"
        + "\n 2.NMP    : {2}L  ({3})"
        + "\n 3.������ : {4}�� ({5})"
        + "\n 4.���ؾ� : {6}L  ({7})";

    string InfoDisplay2 = "���� ��⿭" +
        "\n 1.�ͽ����� ���� : {0}��"
        + "\n 2.������ ����   : {1}��"
        + "\n 3.����ŷ ����   : {2}��"
        + "\n 4.�׽�Ʈ ����   : {3}��";

    string InfoDisplay3 = "���� ������ǥ" +
        "\n 1.�������� ���� : {0}��"
        + "\n 2.�������� ���� : {1}��"
        + "\n 3.�ҷ�ǰ   ���� : {2}��"
        + "\n 4.Ʈ������ ���� : {3}��";

    string InfoDisplay5 = "���� �ڿ� ��뷮" +
        "\n 1.Ȱ���� : {0}L"
        + "\n 2.NMP    : {1}L"
        + "\n 3.������ : {2}��"
        + "\n 4.���ؾ� : {3}L";

    [ReadOnly] public Factory factory;
    [ReadOnly] public WareHouse wareHouse;
    [ReadOnly] public FactoryStatistic factoryStatistic;
    [ReadOnly] public CCTV Monitoring;
    public TextMeshProUGUI InfoText;
    public TextMeshProUGUI CAMText;
    private Dictionary<string, float> beforeInventory = new Dictionary<string, float>()
    {
        {"ACTIVE_LIQUID", 0},
        {"NMP", 0},
        {"NEGATIVE_ELECTRODE", 0},
        {"ELECTROLYTIC", 0}
    };

    public string makeInfoDisplay1()
    {
        string activeLiquid = IngredientType.ACTIVE_LIQUID.ToString();
        string npm = IngredientType.NMP.ToString();
        string negativeEletrode = IngredientType.NEGATIVE_ELECTRODE.ToString();
        string electrolytic = IngredientType.ELECTROLYTIC.ToString();

        string str = string.Format(InfoDisplay1, 
            wareHouse.nowInventory[activeLiquid],     (wareHouse.nowInventory[activeLiquid] - beforeInventory[activeLiquid]),
            wareHouse.nowInventory[npm],              (wareHouse.nowInventory[npm] - beforeInventory[npm]),
            wareHouse.nowInventory[negativeEletrode], (wareHouse.nowInventory[negativeEletrode] - beforeInventory[negativeEletrode]),
            wareHouse.nowInventory[electrolytic],     (wareHouse.nowInventory[electrolytic] - beforeInventory[electrolytic])
            );

        foreach (KeyValuePair<string, float> kvp in wareHouse.nowInventory)
        {
            beforeInventory[kvp.Key] = kvp.Value;
        }

        return str;
    }

    private void Start()
    {
        factory = GameObject.FindWithTag("Factory").GetComponent<Factory>();
        wareHouse = GameObject.FindWithTag("Warehouse").GetComponent<WareHouse>();
        factoryStatistic = GameObject.FindWithTag("Factory").GetComponent<FactoryStatistic>();
        Monitoring = GameObject.FindWithTag("CCTV").GetComponent<CCTV>();
        StartCoroutine(textDisplayStart());
        StartCoroutine(UpdateCAMTEXT());
    }

    public string makeInfoDisplay2()
    {
        int mixCoatingLength = -1;
        int pressLength = -1;
        int stackLength = -1;
        int testLength = -1;
        foreach (ProductQueue pq in factory.productQueues)
        {
            MachineType machineType = pq.GetTargetMachine().GetMachineType();
            switch(machineType)
            {
                case MachineType.MIXCOATING_MACHINE:
                    mixCoatingLength = pq.GetWaitingProductCount();
                    break;
                case MachineType.PRESS_MACHINE:
                    pressLength = pq.GetWaitingProductCount();
                    break;
                case MachineType.STACK_MACHINE:
                    stackLength = pq.GetWaitingProductCount();
                    break;
                case MachineType.TEST_MACHINE:
                    testLength = pq.GetWaitingProductCount();
                    break;
            }
        }
        return string.Format(InfoDisplay2, 
            mixCoatingLength.ToString(), 
            pressLength.ToString(), 
            stackLength.ToString(), 
            testLength.ToString()
        );
    }

    public string makeInfoDisplay3()
    {
        return string.Format(InfoDisplay3,
            factoryStatistic.ProductStartCount,
            factoryStatistic.TestSuccessCount,
            factoryStatistic.TestFailCount,
            factory.truck.LuguageCount
            );
    }

    public string makeInfoDisplay5()
    {
        return string.Format(InfoDisplay5,
            factoryStatistic.UsageActiveLiquidCount,
            factoryStatistic.UsageNPMCount,
            factoryStatistic.UsageNegativeElectrodeCount,
            factoryStatistic.UsageElectrolyticCount
            );
    }

    IEnumerator textDisplayStart()
    {
        int index = 0;
        while (true)
        {
            switch (index)
            {
                case 0:
                    InfoText.text = makeInfoDisplay2();
                    break;
                case 1:
                    InfoText.text = makeInfoDisplay1();
                    break;
                case 2:
                    InfoText.text = makeInfoDisplay3();
                    break;
                case 3:
                    InfoText.text = makeInfoDisplay5();
                    break;
            }
            yield return new WaitForSeconds(7.0f);
            index = (index + 1) % 4;
        }
    }

    IEnumerator UpdateCAMTEXT()
    {
        while (true)
        {
            CAMText.text = Monitoring.nowViweingCameraName();
            yield return new WaitForEndOfFrame();
        }
    }
}
