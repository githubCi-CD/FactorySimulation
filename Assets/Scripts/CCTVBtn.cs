using Assets.Scripts.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CCTVBtn : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public CCTVBtn_type btn;
    public bool btnFlag = false;
    private CCTV cctv;
    private void Start()
    {
        cctv = GameObject.FindWithTag("CCTV").GetComponent<CCTV>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(btn == CCTVBtn_type.LEFT)
        {
            cctv.LeftBtnDown();
        }
        else if(btn == CCTVBtn_type.RIGHT)
        {
            cctv.RightBtnDown();
        }
        else if(btn == CCTVBtn_type.UP)
        {
            cctv.UpBtnDown();
        }
        else if(btn == CCTVBtn_type.DOWN)
        {
            cctv.DownBtnDown();
        }
        else if(btn == CCTVBtn_type.ZOOM_IN)
        {
            cctv.ZoomInBtnDown();
        }
        else if(btn == CCTVBtn_type.ZOOM_OUT)
        {
            cctv.ZoomOutBtnDown();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (btn == CCTVBtn_type.LEFT)
        {
            cctv.LeftBtnUp();
        }
        else if (btn == CCTVBtn_type.RIGHT)
        {
            cctv.RightBtnUp();
        }
        else if (btn == CCTVBtn_type.UP)
        {
            cctv.UpBtnUp();
        }
        else if (btn == CCTVBtn_type.DOWN)
        {
            cctv.DownBtnUp();
        }
        else if (btn == CCTVBtn_type.ZOOM_IN)
        {
            cctv.ZoomInBtnUp();
        }
        else if (btn == CCTVBtn_type.ZOOM_OUT)
        {
            cctv.ZoomOutBtnUp();
        }
    }
}
