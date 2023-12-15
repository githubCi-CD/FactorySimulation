using Sirenix.OdinInspector;
using System;
using System.Collections;

using UnityEngine;

namespace Assets.Scripts
{
    public class Truck : SerializedMonoBehaviour
    {
        public int LuguageCount;
        public int MaxLuguageCount;

        private Factory factory;
        private bool isWaiting = true;
        private ProductQueue truckProductQueue;

        private void Start()
        {
            LuguageCount = 0;
            MaxLuguageCount = Configration.Instance.MaxLuguageCount;
            factory = GameObject.FindWithTag("Factory").GetComponent<Factory>();
            truckProductQueue = factory.GetTruckProductQueue();
            StartCoroutine(WaitingLauguage());
        }

        public bool ShippingLuguage(Product product)
        {
            LuguageCount++;
            if(LuguageCount == MaxLuguageCount)
            {
                TruckDeparture();
            }
            //정상품 API 호출

            isWaiting = true;
            factory.SellingProduct(product);
            return true;
        }

        public void TruckDeparture()
        {
            StopCoroutine(WaitingLauguage());
            LuguageCount = 0;
            Debug.Log("Truck Departure");
            //트럭출발 애니메이션

            //애니메이션 이후 다시 코루틴 시작
            return;
        }

        IEnumerator WaitingLauguage()
        {
            while (true)
            {
                if (isWaiting == false)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                
                if (truckProductQueue.GetWaitingProductCount() > 0)
                {
                    isWaiting = false;
                    Product product = truckProductQueue.GetNextProduct();
                    ShippingLuguage(product);
                }
                yield return new WaitForSeconds(1);
            }
        }
    }
}
