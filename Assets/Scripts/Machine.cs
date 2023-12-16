using Sirenix.OdinInspector;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Config
{
    public class Machine : SerializedMonoBehaviour
    {
        [Header("Machine Setting")]
        public              int         machineID;
        public              bool        isMachineOn;
        public              MachineType machineType;
        [ReadOnly] public   float       loadingTime;
        [ReadOnly] public   float       processingTime;
        [ReadOnly] public   int         processingId;
        [ReadOnly] public   bool        isUnshippable;
        [ReadOnly] public   Truck       targetTruck;
        private             float       processingFailRate;
        private             float       processingFailRateRange;

        [Header("Processing Monitor Setting")]
        public MachineStatus    machineStatus = 0;
        public Product          nowProduct;
        public float            nowProcessingTime = 0;

        [HideInInspector]
        private Factory         factory;
        private void Start()
        {
            switch (machineType)
            {
                case MachineType.MIXCOATING_MACHINE:
                    loadingTime     = Configration.Instance.mixCoatingLoadingTime;
                    processingTime  = Configration.Instance.mixCoatingProcessingTime;
                    processingFailRate = Configration.Instance.mixCoatingProcessFailRate;
                    processingFailRateRange = Configration.Instance.mixCoatingProcessFailRateRange;
                    break;
                case MachineType.PRESS_MACHINE:
                    loadingTime     = Configration.Instance.pressingLoadingTime;
                    processingTime  = Configration.Instance.pressingProcessingTime;
                    processingFailRate = Configration.Instance.pressingProcessFailRate;
                    processingFailRateRange = Configration.Instance.pressingProcessFailRateRange;
                    break;
                case MachineType.STACK_MACHINE:
                    loadingTime     = Configration.Instance.stackingLoadingTime;
                    processingTime  = Configration.Instance.stackingProcessingTime;
                    processingFailRate = Configration.Instance.stackingProcessFailRate;
                    processingFailRateRange = Configration.Instance.stackingProcessFailRateRange;
                    break;
                case MachineType.TEST_MACHINE:
                    loadingTime     = Configration.Instance.testingProcessingTime;
                    processingTime  = Configration.Instance.testingLoadingTime;
                    break;
                default:
                    break;
            }
            factory = GameObject.FindWithTag("Factory").GetComponent<Factory>();
            if(machineType == MachineType.SHIPPING_TRUCK)
            {
                targetTruck = GetComponent<Truck>();
                isUnshippable = true;
            }
            WaitingLoad();
        }

        public void SetMachineID(int ID)
        {
            machineID = ID;
        }

        public int GetMachineID()
        {
            return machineID;
        }
        public Truck GetTruck()
        {
            return targetTruck;
        }
        private bool isTruckWaiting()
        {
            return isUnshippable;
        }

        public void GoodPorductArrivedToTruck(Product product)
        {
            Debug.Assert(machineType == MachineType.SHIPPING_TRUCK);
            factory.ShippingToTruck(product);
        }

        public MachineType GetMachineType()
        {
            return machineType;
        }

        public bool LoadingMachine(Product product)
        {
            machineStatus       = MachineStatus.LOADING;
            nowProduct          = product;
            nowProcessingTime   = 0;
            if(machineType != MachineType.TEST_MACHINE)
                factory.UseResourceFromWareHouse(machineType, product);
            StartCoroutine(MachineTakeTime(loadingTime, "로딩", ProcessMachine));
            return true;
        }

        public bool ProcessMachine()
        {
            Debug.Assert(machineType != MachineType.UNKNOWN_ERROR, "알수없는 기계의 종류입니다.");
            machineStatus       = MachineStatus.PROCESSING;

            StartCoroutine(MachineTakeTime(processingTime, machineType.ToString(), CompleteMachine));

            if(machineType != MachineType.TEST_MACHINE)
            {
                bool isSucceed = Possibility.isProcessingSuccess(processingFailRate, processingFailRateRange);
                nowProduct.ProcessIsComplete(machineType, isSucceed);
            }
            return true;
        }

        public bool WaitingLoad()
        {
            nowProduct = null;
            nowProcessingTime = -1;
            if (factory.CheckResourceFromWareHouse(machineType) == false)
            {
                UnloadingMachine();
                return false;
            }
            machineStatus = MachineStatus.WAITING;
            StartCoroutine(WaitingNextProduct());
            return true;
        }

        public void UnloadingMachine()
        {
            machineStatus = MachineStatus.UNLOADING;
            StartCoroutine(WaitingResourceFromWarehouse(WaitingLoad));
        }

        public bool CompleteMachine()
        {

            if (machineType == MachineType.TEST_MACHINE)
            {
                bool isGood = nowProduct.isGoodProduct();
                if (isGood == false)
                {
                    nowProduct.WasteProduct();
                }
                factory.TestProcessComplete(isGood, nowProduct);
                WaitingLoad();
                return true;
            }
            factory.MachineProcessComplete(this, nowProduct);
            WaitingLoad();

            return true;
        }


        IEnumerator MachineTakeTime(float time, String kind, Func<bool> afterFunc)
        {
            String unity_Log = "Machine ID:" + 
                                machineID + " => 제품 " + 
                                nowProduct + "에 대한 ";

            Debug.Log(unity_Log + kind + "작업을 시작합니다.");
            yield return new WaitForSeconds(time);
            Debug.Log(unity_Log + kind + "작업을 완료했습니다.");

            afterFunc();

            yield return null;
        }

        IEnumerator WaitingNextProduct()
        {
            while (true)
            {
                bool isWaiting = factory.CheckNextProductForMachine(this);
                if(isWaiting == true && machineStatus == MachineStatus.WAITING)
                {
                    factory.GetNextProductForMachine(this);
                    machineStatus = MachineStatus.LOADING;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        IEnumerator WaitingResourceFromWarehouse(Func<bool> waitLoad)
        {
            while (true)
            {
                if(factory.CheckResourceFromWareHouse(machineType) == true)
                {
                    waitLoad();
                    break;
                }
                yield return new WaitForSeconds(1);
            }
            yield return null;
        }
    }
}
