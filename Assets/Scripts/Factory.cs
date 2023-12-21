using Asset.Script.Backend;
using Assets.Scripts.Config;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class Factory : SerializedMonoBehaviour
    {
        [Header("FactoryMetaData")]
        [ReadOnly] public long FactoryId = -1;
        [ReadOnly] public string FactoryName;
        [ReadOnly] public long ProductCount;

        [Header("Inventory")]
        [ReadOnly] public WareHouse wareHouse;
        public Truck truck;

        [Header("Machine")]
        public List<Machine> machines = new List<Machine>();

        [Header("Product")]
        public List<Product> products = new List<Product>();

        [Header("Processing Console")]
        [ReadOnly] public List<ProductQueue> productQueues = new List<ProductQueue>();

        [Header("Visualize Asset")]
        public Dictionary<string, Material> machineVisualizeMaterial = new Dictionary<string, Material>();

        public APIHandler apiHandler;
        public FactoryStatistic factoryStatistic;

        static ProductQueue MinQueue(ProductQueue x, ProductQueue y)
        {
            if (x == null) return y;
            if (y == null) return x;
            return (x.GetWaitingProductCount() < y.GetWaitingProductCount()) ? x : y;
        }

        public long GetFactoryId()
        {
            return FactoryId;
        }

        public long GetProductCount()
        {
            return ProductCount;
        }
        public long IssuanceID()
        {
            ProductCount += 1;
            return ProductCount;
        }

        private void Start()
        {
            if (Configration.Instance.startAtFactoryMode == false)
            {
                FactoryId = Configration.Instance.factoryId;
                FactoryName = Configration.Instance.factoryName;
                ProductCount = Configration.Instance.totalCount;
            }
            else
            {
                FactoryId = 1;
                FactoryName = "TestFactory";
                ProductCount = 0;
            }
            if(Configration.Instance.standAloneMode == false)
            {
                APIHandler.Instance.GetMaxProductID(APIType.GET_MAX_PRODUCT_ID, FactoryId, InitFactoryProductId);
            }
            int IDStart = 1; 
            foreach (var machine in machines)
            {
                machine.SetMachineID(IDStart);
                IDStart += 1;
            }
            truck = GameObject.FindWithTag("Truck").GetComponent<Truck>();
            wareHouse = GameObject.FindWithTag("Warehouse").GetComponent<WareHouse>();
            factoryStatistic = GetComponent<FactoryStatistic>();
        }

        public bool InitFactoryProductId(long res)
        {
            ProductCount = res;
            return true;
        }

        public void GetOutFromFactory()
        {
            if(Configration.Instance.standAloneMode == false)
            {

                APIHandler.Instance.DisconnectFactory(APIType.DISCONNECT_FACTORY, FactoryName);
            }
        }

        public void StatisticArchive(statisticType type, float value = 0)
        {
            switch(type)
            {
                case statisticType.PRODUCT_START_COUNT:
                    factoryStatistic.AddProductStartCount();
                    break;
                case statisticType.PRODUCT_TEST_SUCCESS_COUNT:
                    factoryStatistic.AddProductTestSuccessCount();
                    break;
                case statisticType.PRODUCT_TEST_FAIL_COUNT:
                    factoryStatistic.AddProductTestFailCount();
                    break;
                case statisticType.MATERIAL_USAGE_ACTIVE_LIQUID:
                    factoryStatistic.AddUsageActiveLiquidCount(value);
                    break;
                case statisticType.MATERIAL_USAGE_NMP:
                    factoryStatistic.AddUsageNPMCount(value);
                    break;
                case statisticType.MATERIAL_USAGE_NEGATIVE_ELECTRODE:
                    factoryStatistic.AddUsageNegativeElectrodeCount(value);
                    break;
                case statisticType.MATERIAL_USAGE_ELECTROLYTIC:
                    factoryStatistic.AddUsageElectrolyticCount(value);
                    break;
                default:
                    Debug.Assert(false, "알수없는 통계 종류입니다.");
                    break;
            }
        }

        public ProductQueue GetMachineQueueByID(int machineID)
        {
            return productQueues.Find(x => x.GetTargetMachine().GetMachineID() == machineID);
        }

        public void MachineProcessComplete(Machine machine, Product product)
        {
            Debug.Assert(machine != null);
            Debug.Assert(product != null);
            ProductQueue nextPq = AssignNextMachine(machine.GetMachineType());
            nextPq.StartDeliveryToWaitingPoint(product);
        }

        public void TestProcessComplete(ProcessType isGood, Product product)
        {
            if(isGood == ProcessType.NONE)
            {
                ProductQueue truckPq = productQueues.Find(x => x.GetTargetMachine().GetMachineType() == MachineType.SHIPPING_TRUCK);
                truckPq.StartDeliveryToTruck(product);
                APIHandler.Instance.ProductSuccess(APIType.REPORT_PRODUCT_SUCCESS, FactoryId);
                APIHandler.Instance.LogProduceSuccess(LOGType.PRODUCT_TEST_SUCCESS, product.GetProductGuid(),FactoryId, product.GetProductId());
            }
            else
            {
                APIHandler.Instance.ProductFailure(APIType.REPORT_PRODUCT_FAULTY, FactoryId);
                APIHandler.Instance.LogProductFaulty(LOGType.PRODUCT_TEST_FAIL, product.GetProductGuid(), FactoryId, product.GetProductId(), isGood.ToString());
            }
        }

        public bool CheckResourceFromWareHouse(MachineType machine)
        {
            Debug.Assert(machine != MachineType.UNKNOWN_ERROR);
            switch (machine)
            {
                case MachineType.MIXCOATING_MACHINE:
                    if (wareHouse.CheckMixCoatingResource() == false)
                        return false;
                    break;
                case MachineType.PRESS_MACHINE:
                    if (wareHouse.CheckPressResource() == false)
                        return false;
                    break;
                case MachineType.STACK_MACHINE:
                    if (wareHouse.CheckStackingResource() == false)
                        return false;
                    break;
                case MachineType.TEST_MACHINE:
                    return true;
                case MachineType.SHIPPING_TRUCK:
                    return true;
                default:
                    Debug.Assert(false, "알수없는 기계의 종류입니다.");
                    break;
            }
            return true;
        }

        public bool UseResourceFromWareHouse(MachineType machine, Product product)
        {
            Debug.Assert(product != null);
            Debug.Assert(machine != MachineType.UNKNOWN_ERROR);
            switch (machine)
            {
                case MachineType.MIXCOATING_MACHINE:
                    if (wareHouse.CheckMixCoatingResource() == false)
                        return false;
                    wareHouse.UsingMixCoatingResource(product);
                    break;
                case MachineType.PRESS_MACHINE:
                    if (wareHouse.CheckPressResource() == false)
                        return false;
                    wareHouse.UsingPressingResource(product);
                    break;
                case MachineType.STACK_MACHINE:
                    if (wareHouse.CheckStackingResource() == false)
                        return false;
                    wareHouse.UsingStackingResource(product);
                    break;
                default:
                    Debug.Assert(false, "알수없는 기계의 종류입니다.");
                    break;
            }
            return true;
        }

        public bool CheckNextProductForMachine(Machine machine)
        {
            ProductQueue pq = GetMachineQueueByID(machine.GetMachineID());
            Debug.Assert(pq != null, "대기열이 없는 기계에 대해 다음 작업을 할 수 없습니다.");
            if(pq.GetWaitingProductCount() > 0)
            {
                return true;
            }
            return false;
        }

        public Product GetNextProductForMachine(Machine machine)
        {
            ProductQueue pq = GetMachineQueueByID(machine.GetMachineID());
            Debug.Assert(pq != null, "대기열이 없는 기계에 대해 다음 작업을 할 수 없습니다.");
            if (pq.GetWaitingProductCount() > 0)
            {
                Product nextProduct = pq.GetNextProduct();
                pq.StartDeliveryToMachine(nextProduct);
                return nextProduct;
            }
            return null;
        }

        public void ShippingToTruck(Product product)
        {
            Debug.Assert(product != null);
            truck.ShippingLuguage(product);
        }

        public ProductQueue GetTruckProductQueue()
        {
            List<ProductQueue> pq = new List<ProductQueue>();
            foreach(ProductQueue pqItem in productQueues)
            {
                if (pqItem.GetTargetMachine() == null)
                    continue;
                if (pqItem.GetTargetMachine().GetMachineType() == MachineType.SHIPPING_TRUCK)
                    pq.Add(pqItem);
            }
            Debug.Assert(pq.Count == 1, "트럭이 존재하지 않거나 여러대 존재합니다.");
            return pq[0];
        }

        public ProductQueue AssignNextMachine(MachineType beforeMachine)
        {
            //다음 작업을 수행할 기계의 종류를 확인한다.
            MachineType nextMachineType = MachineType.MIXCOATING_MACHINE;
            switch (beforeMachine)
            {
                case MachineType.MIXCOATING_MACHINE:
                    nextMachineType = MachineType.PRESS_MACHINE;
                    break;
                case MachineType.PRESS_MACHINE:
                    nextMachineType = MachineType.STACK_MACHINE;
                    break;
                case MachineType.STACK_MACHINE:
                    nextMachineType = MachineType.TEST_MACHINE;
                    break;
                case MachineType.TEST_MACHINE:
                    nextMachineType = MachineType.SHIPPING_TRUCK;
                    break;
                case MachineType.SHIPPING_TRUCK:
                    return null;
                default:
                    Debug.Assert(false, "다음 작업을 알수없는 오류가 발생했습니다.");
                    break;
            }

            //다음 작업을 수행할 수 있는 기계 중 가장 적은 대기열을 가진 기계대기열을 선택
            ProductQueue minNextMachineQueue = null;
            foreach(ProductQueue productQueue in productQueues)
            {
                if(productQueue.GetTargetMachine().GetMachineType() == nextMachineType)
                {
                    minNextMachineQueue = MinQueue(minNextMachineQueue, productQueue);
                }
            }

            return minNextMachineQueue;
        }

    }

}
