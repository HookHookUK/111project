using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Object_OnPointUpDown : MonoBehaviour, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private int num;
    [SerializeField] private Slider _slider;

    public void OnPointerClick(PointerEventData eventData)
    {
        _slider.value = 0;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _slider.value = 0;
    }
    public void SetSlider0()
    {
        _slider.normalizedValue = 0f;
    }

}
