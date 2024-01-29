using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIController : MonoBehaviour
{
    public CharacterBattleController characterData;

    public Transform health;
    public Transform mana;

    Image healthBar;
    Image manaBar;

    TextMeshProUGUI healthText;
    TextMeshProUGUI manaText;

    private void Start()
    {
        healthBar = health.GetComponent<Image>();
        manaBar = mana.GetComponent<Image>();

        healthText = health.Find("Text").GetComponent<TextMeshProUGUI>();
        manaText = mana.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        int currentHealth = characterData.currentPoint.health;
        int maximumHealth = characterData.maximumPoint.health;
        healthText.text = $"Health: {currentHealth}/{maximumHealth}";
        healthBar.color = Color.Lerp(Color.red, Color.green, currentHealth/(float)maximumHealth);
        manaText.text = $"Mana: {characterData.currentPoint.mana}/{characterData.maximumPoint.mana}";
    }
}
