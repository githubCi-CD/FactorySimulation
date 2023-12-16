using Assets.Scripts.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Product : MonoBehaviour
    {
        [ReadOnly] public long                   productId = -1;
        [ReadOnly] public ProcessResultStatus   isMixingCoating = ProcessResultStatus.NONE;
        [ReadOnly] public ProcessResultStatus   isPressing      = ProcessResultStatus.NONE;
        [ReadOnly] public ProcessResultStatus   isStacking      = ProcessResultStatus.NONE;
        public Material[] processGameObj;
        public GameObject finalBattery;
        public GameObject insideProduct;

        private GameObject selfProductGameObject;
        public Rigidbody  selfProductRigidbody;

        private void Awake()
        {
            productId = -1;
            selfProductGameObject = this.gameObject;
        }

        public void giveId(long id)
        {
            Debug.Assert(productId == -1);
            productId = id;
        }

        public long GetProductId()
        {
            return productId;
        }
        public void initProductId(int id)
        {
            productId = id;
        }

        public GameObject GetProductGameObject()
        {
            return selfProductGameObject;
        }

        public bool isGoodProduct()
        {
            return (
                    isMixingCoating == ProcessResultStatus.SUCESS &&
                    isPressing == ProcessResultStatus.SUCESS &&
                    isStacking == ProcessResultStatus.SUCESS
                );
        }

        public void WasteProduct()
        {
            selfProductRigidbody.isKinematic = false;
            selfProductRigidbody.AddForce(new Vector3(1, 1, 0) * 100);
            StartCoroutine(WaitAndDestory());
        }

        public void ProcessIsComplete(MachineType processType, bool isSucceed)
        {
            switch(processType)
            {
                case MachineType.MIXCOATING_MACHINE:
                    insideProduct.GetComponent<MeshRenderer>().material = processGameObj[0];
                    isMixingCoating = (isSucceed ? ProcessResultStatus.SUCESS : ProcessResultStatus.FAIL);
                    break;
                case MachineType.PRESS_MACHINE:
                    insideProduct.GetComponent<MeshRenderer>().material = processGameObj[1];
                    isPressing = (isSucceed ? ProcessResultStatus.SUCESS : ProcessResultStatus.FAIL);
                    break;
                case MachineType.STACK_MACHINE:
                    insideProduct.SetActive(false);
                    finalBattery.SetActive(true);
                    isStacking = (isSucceed ? ProcessResultStatus.SUCESS : ProcessResultStatus.FAIL);
                    break;
                case MachineType.TEST_MACHINE:
                    break;
            }
        }
        public ProcessResultStatus isMixCoatinged()
        {
            return isMixingCoating;
        }
        public ProcessResultStatus isPressed()
        {
            return isPressing;
        }
        public ProcessResultStatus isStacked()
        {
            return isStacking;
        }

        IEnumerator WaitAndDestory()
        {
            yield return new WaitForSeconds(3.0f);
            Destroy(this.gameObject);
        }
    }
}
