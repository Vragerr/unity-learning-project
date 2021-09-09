using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;
    PlayerStats stats;
    public void SetMaxHealth(int health)
    {
        text.text = $"{health}";
        slider.maxValue = health;
        slider.value = health;
    }
    public void SetHealth(int health)
    {
        slider.value = health;
        text.text = $"{health}";
    }

}
