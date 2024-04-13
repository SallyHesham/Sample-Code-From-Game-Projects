using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SigilPoint : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{
    public int number;
    public SigilHandler handler;

    public void OnBeginDrag(PointerEventData eventData)
    {
        handler.beginDraw(number);
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        handler.endDraw();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        handler.addPoint(number);
    }
}
