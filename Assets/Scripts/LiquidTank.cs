using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidTank : MonoBehaviour
{
    private GameObject TankInside;
    private CapsuleCollider capsuleCollider;

    private void Start()
    {
        TankInside = transform.GetChild(0).gameObject;
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    public void UpdateLiquidTank(float max_tank, float now_tank)
    {
        float tankHeight = capsuleCollider.height;
        float nowInsideScaleXZ = TankInside.transform.localScale.z;
        float maxInsideTankHeight = nowInsideScaleXZ;
        float nowInsideTankHeight = maxInsideTankHeight * (now_tank / max_tank);
        if (nowInsideTankHeight > maxInsideTankHeight) 
        {
            nowInsideTankHeight = maxInsideTankHeight;
        }
        TankInside.transform.localScale = new Vector3(nowInsideScaleXZ, nowInsideTankHeight, nowInsideScaleXZ);

        float heightOffset = (tankHeight - nowInsideTankHeight) / 4;
        TankInside.transform.localPosition = new Vector3(0, -heightOffset, 0);

    }
}