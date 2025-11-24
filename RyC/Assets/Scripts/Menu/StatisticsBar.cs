using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatisticsBar : MonoBehaviour
{
    public TMP_Text barText;
    public Image bar;

    float value, maxValue = 100;

    private void Start()
    {
        //value = maxValue;
        value = 50;
    }

    private void Update()
    {
        barText.text = "" + value;
        BarFiller();
    }

    public void BarFiller()
    {
        bar.fillAmount = value / maxValue;
    }
}
