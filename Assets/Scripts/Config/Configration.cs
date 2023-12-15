using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Configration : SerializedMonoBehaviour
{
    private static Configration instance;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static Configration Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }


    [Header("Machine Setting")]
    public float mixCoatingProcessingTime;
    public float mixCoatingLoadingTime;
    public float pressingProcessingTime;
    public float pressingLoadingTime;
    public float stackingProcessingTime;
    public float stackingLoadingTime;
    public float testingProcessingTime;
    public float testingLoadingTime;

    [Header("Factory Setting")]
    public float conveyorSpeed;
    public float inputInterval;

    [Header("Warehouse Setting")]
    public Dictionary<string, float> init_Inventory = new Dictionary<string, float>();
    public Dictionary<string, float> max_Inventory = new Dictionary<string, float>();
    public Dictionary<string, float> mixCoatingInputResource = new Dictionary<string, float>();
    public Dictionary<string, float> pressingInputResource = new Dictionary<string, float>();
    public Dictionary<string, float> stackingInputResource = new Dictionary<string, float>();

    [Header("Statistic Setting")]
    public float mixCoatingProcessFailRate;
    public float mixCoatingProcessFailRateRange;
    public float pressingProcessFailRate;
    public float pressingProcessFailRateRange ;
    public float stackingProcessFailRate;
    public float stackingProcessFailRateRange;

    [Header("Truck Setting")]
    public int MaxLuguageCount = 25;

    [Header("Server Setting")]
    public string serverHost = "http://192.168.0.102:";
    public string resourceManageAPIPort = "30081";
    public string factoryManageAPIPort = "30082";
}
