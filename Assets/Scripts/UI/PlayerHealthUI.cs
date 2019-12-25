using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Image healthFill;
    public TextMeshProUGUI healthText;

    private PlayerModel playerModel;

    // Start is called before the first frame update
    void Start()
    {
        playerModel = GameManager.Instance.playerModel;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerHealthUI();
    }

    private void UpdatePlayerHealthUI()
    {
        healthFill.fillAmount = playerModel.health.normalizedHealth;
        healthText.text = (Mathf.CeilToInt(playerModel.health.normalizedHealth * 100)).ToString();
    }
}