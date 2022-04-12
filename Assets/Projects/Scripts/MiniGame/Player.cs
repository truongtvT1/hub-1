using System.Collections;
using System.Collections.Generic;
using RandomNameAndCountry.Scripts;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer flag;
    public TextMeshProUGUI name, country;
    
    void Start()
    {
        var playerInfo = RandomNameAndCountryPicker.Instance.GetRandomPlayerInfo();
        flag.sprite = playerInfo.countrySprite;
        name.text = playerInfo.playerName;
        country.text = playerInfo.countryName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
