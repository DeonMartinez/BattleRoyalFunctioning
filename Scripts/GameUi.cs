using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUi : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI playerInfoText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI grenadeText;
    public TextMeshProUGUI winText;
    public Image winBackground;

    private PlayerController player;

    // instance 
    public static GameUi instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void Initialize(PlayerController localPlayer)
    {
        player = localPlayer;
        healthBar.maxValue = player.maxHP;
        healthBar.value = player.curHP;
        UpdatePlayerInfoText();
        UpdateAmmoText();
        UpdateGrenadeText();
    }

    public void UpdateHealthBar()
    {
        healthBar.value = player.curHP;
    }

    public void UpdatePlayerInfoText()
    {
        playerInfoText.text = "<b>Alive:</b> " + GameManager.instance.alivePlayers + "\n<b>Kills:</b> " + player.kills;
    }

    public void UpdateAmmoText()
    {
        ammoText.text = player.weapon.curAmmo + " / " + player.weapon.maxAmmo;
    }

    void UpdateGrenadeText()
    {
        
    }
    public void SetWinText(string winnerName)
    {
        winBackground.gameObject.SetActive(true);
        winText.text = winnerName + " wins";
    }
}
