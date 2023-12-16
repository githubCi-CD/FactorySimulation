using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets
{
    public class ConveyorSystem : SerializedMonoBehaviour
    {
        public ProductQueue targetProductQueue;
        public LineRenderer movingLineRenderer;
        public int StartIndex;
        public int WaitingIndex;
        public int EndIndex;
        [ReadOnly] public float Offset;
        [ReadOnly] public float Speed;   
        private int pointCount;
        private Vector3[] localPoints;


        private void Start()
        {
            Speed = Configration.Instance.conveyorSpeed;
        }
        public void conveyorPointSetting()
        {
            pointCount = movingLineRenderer.positionCount;
            localPoints = new Vector3[pointCount];
            movingLineRenderer.GetPositions(localPoints);
            WaitingIndex = EndIndex - 1;
        }

        public void ComeIntoBelt(Product product)
        {
            Debug.Assert(product != null);
            if(localPoints == null)
            {
                conveyorPointSetting();
            }
            product.gameObject.transform.position = localPoints[StartIndex];
            StartCoroutine(moveFollowLine(product.gameObject, StartIndex, WaitingIndex));
        }

        public void GoIntoMachine(Product product)
        {
            Debug.Assert(product != null);

            product.gameObject.transform.position = localPoints[WaitingIndex];
            StartCoroutine(moveFollowLine(product.gameObject, WaitingIndex, EndIndex));
        }

        public void ShippingToTruck(Product product)
        {
            Debug.Assert(product != null);
            if (localPoints == null)
            {
                conveyorPointSetting();
            }
            product.gameObject.transform.position = localPoints[StartIndex];
            StartCoroutine(moveFollowLine(product.gameObject, StartIndex, EndIndex));
        }

        public void StopConveyorBelt()
        {
            StopAllCoroutines();
        }   

        public void RestartConveyorBlet()
        {
            List<GameObject> products = new List<GameObject>();
            for (int i = 0; i < targetProductQueue.GetWaitingProductCount(); i++)
            {
                products.Add(targetProductQueue.GetWaitingProducts()[i].gameObject);
            }
            StartCoroutine(MoveConveyorInOrder(products));
        }

        IEnumerator MoveConveyorInOrder(List<GameObject> products)
        {
            foreach (var product in products)
            {
                StartCoroutine(moveFollowLine(product, StartIndex, WaitingIndex));
                yield return new WaitForSeconds(Offset);
            }
        }


        IEnumerator moveFollowLine(GameObject product, int startPoint, int endPoint)
        {
            int index = startPoint;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (index == endPoint)
                {
                    if(targetProductQueue.isTruckConvey() == true)
                    {
                        targetProductQueue.ArrivedDeliveryToWaitingPoint(product.GetComponent<Product>());
                        yield break;
                    }

                    if (endPoint == WaitingIndex)
                    {
                        targetProductQueue.AddWaitingProduct(product.GetComponent<Product>());
                    }
                    else if (endPoint == EndIndex)
                    {
                        targetProductQueue.ArrivedDeliveryToMachine(product.GetComponent<Product>());
                    }
                    yield break;
                }
                else
                {
                    product.transform.position = Vector3.MoveTowards(product.transform.position, localPoints[index + 1], Speed * Time.deltaTime);
                    product.transform.LookAt(localPoints[index + 1]);
                    if (product.transform.position == localPoints[index + 1])
                    {
                        index++;
                    }
                }
            }
        }
    }
}
