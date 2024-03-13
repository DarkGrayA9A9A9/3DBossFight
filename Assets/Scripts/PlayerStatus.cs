using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    public float currentStamina;
    public float maxStamina;
    public float attack;

    public float runCost;
    public float slideCost;

    public Slider healthSlider;
    public Slider staminaSlider;
    public GameObject staminaSliderObj;

    public static PlayerStatus instance;

    void Awake()
    {
        if (PlayerStatus.instance == null)
            PlayerStatus.instance = this;
    }

    void Update()
    {
        StatusSetting();
        Run();
    }

    void LateUpdate()
    {
        hideStamina();
        StaminaRecovery();
    }

    void hideStamina()
    {
        if (currentStamina >= maxStamina)
            staminaSliderObj.SetActive(false);
        else
            staminaSliderObj.SetActive(true);
    }

    void StatusSetting()
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (currentStamina > maxStamina)
            currentStamina = maxStamina;

        healthSlider.value = currentHealth / maxHealth;
        staminaSlider.value = currentStamina / maxStamina;
    }

    void StaminaRecovery()
    {
        if (currentStamina < maxStamina && !PlayerController.instance.sliding && !PlayerController.instance.run && !PlayerController.instance.attacking)
            currentStamina += Time.deltaTime;
    }

    void Run()
    {
        if (PlayerController.instance.run && currentStamina > 0f)
            currentStamina -= Time.deltaTime * runCost;
    }

    public void Slide()
    {
        currentStamina -= slideCost;
    }

    public void Hit(float damage)
    {
        float random = Random.Range(0.5f, 1.5f); // 데미지 편차

        currentHealth -= damage * random;
    }
}
