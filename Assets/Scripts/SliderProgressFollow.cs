using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderProgressFollow : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    public Slider slider;

    private void Awake()
    {
        if (slider)
        {
            slider.onValueChanged.AddListener(OnSliderValueChange);
            var posX = slider.value * ((RectTransform) (slider.transform)).rect.width;
            ((RectTransform) (transform)).anchoredPosition = new Vector2(posX, 0);
        }
    }
    
    void OnSliderValueChange(float value)
    {
        var posX = (slider.value - 0.5f) * ((RectTransform) (slider.transform)).rect.width;
        ((RectTransform) (transform)).anchoredPosition = new Vector2(posX, 0);
    }

    // Update is called once per frame
   
}
