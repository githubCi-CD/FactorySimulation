using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Assets.Scripts;

public class Configration : SerializedMonoBehaviour
{
    private static Configration instance;
    private void Awake()
    {
        if(isFactorySceneSetting == true && startAtFactoryMode == false)
        {
            Destroy(this.gameObject);
        }
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        mixCoatingProcessFailRate = Possibility.getRandomFaultyPossiblility();
        pressingProcessFailRate = Possibility.getRandomFaultyPossiblility();
        stackingProcessFailRate = Possibility.getRandomFaultyPossiblility();
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
    public Dictionary<string, int> OriginNameToID = new Dictionary<string, int>();

    [Header("Statistic Setting")]
    public float mixCoatingProcessFailRate;
    public float mixCoatingProcessFailRateRange;
    public float pressingProcessFailRate;
    public float pressingProcessFailRateRange ;
    public float stackingProcessFailRate;
    public float stackingProcessFailRateRange;
    public float RandomPossibleRateMin;
    public float RandomPossibleRateMax;

    [Header("Truck Setting")]
    public int MaxLuguageCount = 25;

    [Header("Server Setting")]
    public string serverHost = "http://192.168.0.102";
    public string resourceManageAPIPort = "30081";
    public string factoryManageAPIPort = "30082";
    public string elasticServerHost = "http://192.168.0.100";
    public string elasticServerPort = "9200";
    public string logstashServerPort = "7744";
    public string elasticServerIndex = "factorylog";

    [Header("Monitoring Setting")]
    public float cameraSpeed = 1.0f;
    public float zoomSpeed = 1.0f;

    [Header("Development Setting")]
    public bool standAloneMode;
    public bool startAtFactoryMode;
    public bool isFactorySceneSetting;

    [HideInInspector]
    public long factoryId;
    [HideInInspector]
    public string factoryName;
    [HideInInspector]
    public long totalCount;
}
