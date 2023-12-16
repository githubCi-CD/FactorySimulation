using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Cameras{
    CENTER_CAM,
    MACHINE1_CAM,
    MACHINE1_2_CORNER_CAM,
    MACHINE2_3,
    MACHINE3_4_CORNER_CAM,
    MACHINE_TESTER_CAM,
    MACHINE_TOP_CAM,
    MACHINE_WIDE_CAM

}
public class CCTV : MonoBehaviour
{
    public CameraInfo[] cameraList;

    private int cameraIndex = 1;
    // Start is called before the first frame update
    void Update()
    {
        if(isLeftBtnDown == true)
        {
            cameraList[cameraIndex].TurnLeft(true);
        }
        else
        {
            cameraList[cameraIndex].TurnLeft(false);
        }
        if(isRightBtnDown == true)
        {
            cameraList[cameraIndex].TurnRight(true);
        }
        else
        {
            cameraList[cameraIndex].TurnRight(false);
        }
        if(isUpBtnDown == true)
        {
            cameraList[cameraIndex].TurnUp(true);
        }
        else
        {
            cameraList[cameraIndex].TurnUp(false);
        }
        if(isDownBtnDown == true)
        {
            cameraList[cameraIndex].TurnDown(true);
        }
        else
        {
            cameraList[cameraIndex].TurnDown(false);
        }
        if(isZoomInBtnDown == true)
        {
            cameraList[cameraIndex].ZoomIn(true);
        }
        else
        {
            cameraList[cameraIndex].ZoomIn(false);
        }
        if(isZoomOutBtnDown == true)
        {
            cameraList[cameraIndex].ZoomOut(true);
        }
        else
        {
            cameraList[cameraIndex].ZoomOut(false);
        }

    }

    private bool isLeftBtnDown = false;
    private bool isRightBtnDown = false;
    private bool isUpBtnDown = false;
    private bool isDownBtnDown = false;
    private bool isZoomInBtnDown = false;
    private bool isZoomOutBtnDown = false;
    public void LeftBtnDown()
    {
        isLeftBtnDown = true;
    }
    public void LeftBtnUp()
    {
        isLeftBtnDown = false;
    }

    public void RightBtnDown()
    {
        isRightBtnDown = true;
    }
    public void RightBtnUp()
    {
        isRightBtnDown = false;
    }

    public void UpBtnDown()
    {
        isUpBtnDown = true;
    }
    public void UpBtnUp()
    {
        isUpBtnDown = false;
    }

    public void DownBtnDown()
    {
        isDownBtnDown = true;
    }
    public void DownBtnUp()
    {
        isDownBtnDown = false;
    }

    public void ZoomInBtnDown()
    {
        isZoomInBtnDown = true;
    }
    public void ZoomInBtnUp()
    {
        isZoomInBtnDown = false;
    }

    public void ZoomOutBtnDown()
    {
        isZoomOutBtnDown = true;
    }
    public void ZoomOutBtnUp()
    {
        isZoomOutBtnDown = false;
    }

    public void NextCamera()
    {
        int idx = 0;
        cameraIndex++;
        if(cameraIndex >= cameraList.Length)
        {
            cameraIndex = 0;
        }
        foreach(CameraInfo camera in cameraList)
        {
            if(idx == cameraIndex)
            {
                camera.CameraOnOff(true);
            }
            else
            {
                camera.CameraOnOff(false);
            }
            idx += 1;
        }
    }   

    public void PreviousCamera()
    {
        int idx = 0;
        cameraIndex--;
        if(cameraIndex < 0)
        {
            cameraIndex = cameraList.Length - 1;
        }
        foreach(CameraInfo camera in cameraList)
        {
            if(idx == cameraIndex)
            {
                camera.CameraOnOff(true);
            }
            else
            {
                camera.CameraOnOff(false);
            }
            idx += 1;
        }
    }

    public string nowViweingCameraName()
    {
        return cameraList[cameraIndex].cameraName;
    }
}
