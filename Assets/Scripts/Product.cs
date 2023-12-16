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

        private GameObject selfProductGameObject;
        private Rigidbody  selfProductRigidbody;

        private void Awake()
        {
            productId = -1;
            selfProductGameObject = this.gameObject;
            selfProductRigidbody  = this.gameObject.GetComponentInChildren< Rigidbody >();
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
                    isMixingCoating = (isSucceed ? ProcessResultStatus.SUCESS : ProcessResultStatus.FAIL);
                    break;
                case MachineType.PRESS_MACHINE:
                    isPressing = (isSucceed ? ProcessResultStatus.SUCESS : ProcessResultStatus.FAIL);
                    break;
                case MachineType.STACK_MACHINE:
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
