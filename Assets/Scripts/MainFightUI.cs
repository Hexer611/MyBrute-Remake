using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Unity.VisualScripting;

public class MainFightUI : MonoBehaviour
{
    private static MainFightUI _instance;
    public static MainFightUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MainFightUI>();

            return _instance;
        }
    }
    [SerializeField] private MainFightScene mainFightScene;
    [SerializeField] private TMP_Text player1Text;
    [SerializeField] private TMP_Text player2Text;
    [SerializeField] private Transform player1WeaponsContent;
    [SerializeField] private Transform player2WeaponsContent;
    [SerializeField] private Image weaponExample;

    public void RefreshUI()
    {
        if (mainFightScene.players.Count == 0)
            return;

        player1Text.text = mainFightScene.players[0].currentHP.ToString();
        player2Text.text = mainFightScene.players[1].currentHP.ToString();

        for (int i = 0; i< player1WeaponsContent.transform.childCount; i++)
            Destroy(player1WeaponsContent.GetChild(i).gameObject);
        for (int i = 0; i < player2WeaponsContent.transform.childCount; i++)
            Destroy(player2WeaponsContent.GetChild(i).gameObject);

        foreach (var weapon in mainFightScene.players[0].weapons)
        {
            var newWeapon = Instantiate(weaponExample, player1WeaponsContent);
            newWeapon.sprite = weapon.weaponSprite;
        }

        foreach (var weapon in mainFightScene.players[1].weapons)
        {
            var newWeapon = Instantiate(weaponExample, player2WeaponsContent);
            newWeapon.sprite = weapon.weaponSprite;
        }
    }
}
