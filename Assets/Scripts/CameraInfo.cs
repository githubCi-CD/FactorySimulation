using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInfo : MonoBehaviour
{
    public string cameraName;
    private GameObject internalCAM;
    private Camera inCAM;
    private bool left, right, up, down, zoomIn, zoomOut;
    private float moveSpeed;
    private float zoomSpeed;

    // Start is called before the first frame update
    void Start()
    {
        internalCAM = this.gameObject.transform.GetChild(0).gameObject;
        inCAM = internalCAM.GetComponent<Camera>();
        moveSpeed = Configration.Instance.cameraSpeed;
        zoomSpeed = Configration.Instance.zoomSpeed;
    }

    public void CameraOnOff(bool isOn)
    {
        if(isOn == true)
        {
            inCAM.enabled = true;
        }
        else
        {
            inCAM.enabled = false;
        }
    }

    public void TurnUp(bool isClick)
    {
        up = isClick;
    }
    public void TurnDown(bool isClick)
    {
        down = isClick;
    }
    public void TurnLeft(bool isClick)
    {
        left = isClick;
    }
    public void TurnRight(bool isClick)
    {
        right = isClick;
    }

    public void ZoomIn(bool isClick)
    {
        zoomIn = isClick;
    }
    public void ZoomOut(bool isClick)
    {
        zoomOut = isClick;
    }

    // Update is called once per frame
    void Update()
    {
        if(left == true)
        {
            gameObject.transform.Rotate(0, -moveSpeed, 0);

        }
        else if(right == true)
        {
            gameObject.transform.Rotate(0, moveSpeed, 0);

        }
        else if (up == true)
        {
            internalCAM.transform.Rotate(-moveSpeed, 0, 0);
        }
        else if (down == true)
        {
            internalCAM.transform.Rotate(moveSpeed, 0, 0);
        }
        else if (zoomIn == true)
        {
            if(inCAM.fieldOfView > 1)
                inCAM.fieldOfView -= zoomSpeed;
        }
        else if (zoomOut == true)
        {
            if (inCAM.fieldOfView < 80)
                inCAM.fieldOfView += zoomSpeed;
        }
    }
}
