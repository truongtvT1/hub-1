using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RandomNameAndCountry.Scripts
{
    public class RandomNameAndCountryPicker : MonoBehaviour
    {
        public static RandomNameAndCountryPicker Instance;
        [SerializeField] private List<Sprite> countries;
        private static List<string> m_namesList;
        private TextAsset m_textAsset;

        private void Awake()
        {
            Instance = this;
            m_textAsset = Resources.Load("TextFiles/names") as TextAsset;
            ReadTextFile();
            GetRandomPlayerInfo();
        }

        private void ReadTextFile()
        {
            m_namesList = m_textAsset.text.Split('\n').ToList();
        }

        public RandomPlayerInfo GetRandomPlayerInfo()
        {
            var randomPlayerInfo = new RandomPlayerInfo();
            if (countries.Count != 0) 
            {
                var countrySprite = countries[Random.Range(0, countries.Count)];
                randomPlayerInfo.countrySprite = countrySprite;
                var rawCountryName = countrySprite.name;
                var countryName = Regex.Replace(rawCountryName, "[^a-zA-Z]", "");
                countryName = ToUpperFirstLetter(countryName);
                randomPlayerInfo.countryName = countryName;
            }
            randomPlayerInfo.playerName = m_namesList[Random.Range(0, m_namesList.Count)];
            return randomPlayerInfo;
        }
        
        private string ToUpperFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }
    }

    public class RandomPlayerInfo
    {
        public string playerName;
        public string countryName;
        public Sprite countrySprite;

        public RandomPlayerInfo()
        {
            
        }
    }
}
