using Assets.Scripts.Config;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ProductQueue : SerializedMonoBehaviour
    {
        public ConveyorSystem conveyorSystem;
        [ReadOnly]public bool truckProductQueue;
        private Machine targetMachine;
        [SerializeField]
        private List<Product> products = new List<Product>();

        private void Awake()
        {
            targetMachine = gameObject.GetComponent<Machine>();
        }

        private void Start()
        {
            truckProductQueue = isTruckConvey();
        }

        public bool isTruckConvey()
        {
            return targetMachine.GetTruck();
        }

        public void StartDeliveryToTruck(Product product)
        {
            Debug.Assert(product != null);
            conveyorSystem.ShippingToTruck(product);
        }

        public void ArrivedDeliveryToTruck(Product product)
        {
            Debug.Assert(product != null);
            conveyorSystem.ShippingToTruck(product);
        }

        public void RestartConveyor(Product product)
        {
            Debug.Assert(product != null);
            conveyorSystem.ComeIntoBelt(product);
        }
        public void StartDeliveryToWaitingPoint(Product product)
        {
            Debug.Assert(product != null);
            conveyorSystem.ComeIntoBelt(product);
        }

        public void ArrivedDeliveryToWaitingPoint(Product product) {
            Debug.Assert(product != null);
            AddWaitingProduct(product);
        }

        public void StartDeliveryToMachine(Product product)
        {
            Debug.Assert(product != null);
            conveyorSystem.GoIntoMachine(product);
        }

        public void ArrivedDeliveryToMachine(Product product)
        {
            Debug.Assert(product != null);
            RemoveWaitingProduct(product);
            targetMachine.LoadingMachine(product);
        }

        public void AddWaitingProduct(Product product)
        {
            Debug.Assert(product != null);
            products.Add(product);
            targetMachine.UpdateWaitingText();
        }

        public Machine GetTargetMachine()
        {
            return targetMachine;
        }

        public int GetWaitingProductCount()
        {
            return products.Count;
        }

        public List<Product> GetWaitingProducts()
        {
            return products;
        }

        public void RemoveWaitingProduct(Product product)
        {
            products.Remove(product);
        }

        public Product GetNextProduct()
        {
            Product nextProduct = null;
            if (products.Count > 0)
            {
                nextProduct = products[0];
                RemoveWaitingProduct(nextProduct);
            }
            return nextProduct;
        }


    }
}
