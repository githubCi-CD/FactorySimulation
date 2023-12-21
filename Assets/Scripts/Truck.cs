using Asset.Script.Backend;
using Assets.Scripts.Config;
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
        public AnimationClip DepartureClip;

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
            if (LuguageCount == MaxLuguageCount)
            {
                TruckDeparture();
            }

            isWaiting = true;
            SellingProduct(product);
            return true;
        }

        public void SellingProduct(Product product)
        {
            Destroy(product.gameObject);
        }

        public bool CheckApplySell(API_DTO.ReportProductSellDTO res)
        {
            Debug.Assert(res.factoryId == factory.GetFactoryId());
            if(res.result == (int)ProcessResultStatus.SUCESS)
            {
                return true;
            }
            else
            {
                Debug.Assert(false,"서버에 보고가 누락되었습니다.");
                return false;
            }
        }

        public void TruckDeparture()
        {
            StopCoroutine(WaitingLauguage());
            LuguageCount = 0;
            Debug.Log("Truck Departure");
            //트럭출발 애니메이션

            StartCoroutine(TruckAnime());
            return;
        }

        IEnumerator TruckAnime()
        {
            Animation anime = gameObject.GetComponent<Animation>();
            anime.Play(DepartureClip.name);
            yield return new WaitForSeconds(DepartureClip.length);
            StartCoroutine(WaitingLauguage());
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
