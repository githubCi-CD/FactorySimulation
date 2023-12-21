using Assets.Scripts.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
        private Guid       selfProductGuid;
        public Rigidbody  selfProductRigidbody;

        private void Awake()
        {
            productId = -1;
            selfProductGameObject = this.gameObject;
        }

        public void giveId(long id)
        {
            Debug.Assert(productId == -1);
            Guid guid = Guid.NewGuid();
            selfProductGuid = guid;
            productId = id;
        }

        public Guid GetProductGuid()
        {
            return selfProductGuid;
        }

        public void initProductGuid(Guid guid)
        {
            selfProductGuid = guid;
        }

        public long GetProductId()
        {
            return productId;
        }

        public GameObject GetProductGameObject()
        {
            return selfProductGameObject;
        }

        public ProcessType isGoodProduct()
        {
            if (isMixingCoating == ProcessResultStatus.FAIL)
                return ProcessType.MIXCOATING;
            else if (isPressing == ProcessResultStatus.FAIL)
                return ProcessType.PRESSING;
            else if (isStacking == ProcessResultStatus.FAIL)
                return ProcessType.STACKING;
            return ProcessType.NONE;
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
