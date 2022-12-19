using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI staminaText = default;

    private void OnEnable()
    {
        FPSController.OnStaminaChange += UpdateStamina;
    }

    private void OnDisable()
    {
        FPSController.OnStaminaChange -= UpdateStamina;
    }

    private void Start()
    {
        UpdateStamina(100);
    }

    private void UpdateStamina (float currentStamina)
    {
        staminaText.text = currentStamina.ToString("00");
    }
}
