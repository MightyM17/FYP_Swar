using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PhonemesMap : MonoBehaviour
{
    // public string word;
    
    private readonly Dictionary<string, string> PhonemeMap = new Dictionary<string, string>{
        { "अ", "ə" },
        { "आ", "a:" },
        { "इ", "ɪ" },
        { "ई", "i:" },
        { "उ", "ʊ" },
        { "ऊ", "u:" },
        { "ऋ", "ɻ̩" },
        { "ए", "e:" },
        { "ऐ", "ɛ:" },
        { "ओ", "o:" },
        { "औ", "ɔ:" },
        //{ "अँ", "ə̃" },
        //{ "अः", "əɦə" },
        { "अं", "əm" },
        //{ "ऑ", "ɒ" },

        // Matras
        { "ा", "a:" },
        { "ि", "ɪ" },
        { "ी", "i:" },
        { "ु", "ʊ" },
        { "ू", "u:" },
        { "ृ", "ɻ̩" },
        { "े", "e:" },
        { "ै", "ɛ:" },
        { "ो", "o:" },
        { "ौ", "ɔ:" },
        //{ "ँ", "ə̃" },
        //{ "ː", "əɦə" },
        { "ं", "əm" },
        { "ॆ", "e:" },

        { "क", "k" },
        { "ख", "kʰ" },
        { "ग", "g" },
        { "घ", "gʰ" },
        { "ङ", "ŋ" },
        { "च", "tʃ" },
        { "छ", "tʃʰ" },
        { "ज", "dʒ" },
        { "झ", "dʒʰ" },
        { "ञ", "ɲ" },
        { "ट", "ʈ" },
        { "ठ", "ʈʰ" },
        { "ड", "ɖ" },
        { "ढ", "ɖʰ" },
        { "ण", "ɳ" },
        { "त", "t" },
        { "थ", "tʰ" },
        { "द", "d̪" },
        { "ध", "d̪ʰ" },
        { "न", "n" },
        { "प", "p" },
        { "फ", "pʰ" },
        { "ब", "b" },
        { "भ", "bʰ" },
        { "म", "m" },
        { "य", "j" },
        { "र", "ɾ" },
        { "ल", "l" },
        { "व", "ʋ" },
        { "स", "s" },
        { "श", "ʃ" },
        { "ष", "ʂ" },
        { "ह", "ɦ" },
        { "ळ", "ɭ̆ɭ̆" }
    };


    public List<string> ProcessText(string inputText)
    {
        List<string> phonemes = new List<string>();

        foreach (char character in inputText)
        {
            string charString = character.ToString();

            if (PhonemeMap.ContainsKey(charString))
            {
                phonemes.Add(PhonemeMap[charString]);
            }
            else
            {
                phonemes.Add(charString);
            }
        }

        return phonemes;
    }

    public string PhonemesToString(List<string> phonemes)
    {
        StringBuilder result = new StringBuilder();
        foreach (string phoneme in phonemes)
        {
            result.Append(phoneme + " ");
        }
        return result.ToString().Trim();
    }

    // Example usage
    public string lesgo(string word)
    {
        List<string> phonemeList = ProcessText(word);
        string result = PhonemesToString(phonemeList);
        Debug.Log(result);
        return result;
    }

    public string phonemeToHindi(string phoneme)
    {
        foreach (var kvp in PhonemeMap)
        {
            if (kvp.Value == phoneme)
            {
                return kvp.Key;
            }
        }
        return null; // or handle the case where no match is found
    }
    public static readonly Dictionary<string, int> IpaToNumber = new Dictionary<string, int>
    {
        { "a", 10 },
        { "æ", 11 },
        { "ʌ", 12 },
        { "ɔ", 13 },
        { "aʊ", 14 },
        { "ə", 15 },
        { "ɑɪ", 16 },
        { "b", 17 },
        { "ʧ", 18 },
        { "d", 19 },
        { "ð", 20 },
        { "ɛ", 21 },
        { "ɚ", 22 },
        { "e", 23 },
        { "f", 24 },
        { "g", 25 },
        { "ɦ", 26 },
        { "ɪ", 27 },
        { "i", 28 },
        { "ʤ", 29 },

        { "k", 30 },
        { "l", 31 },
        { "m", 32 },
        { "n", 33 },
        { "ŋ", 34 },
        { "o", 35 },
        { "ɔɪ", 36 },
        { "p", 37 },
        { "r", 38 },
        { "s", 39 },
        { "ʃ", 40 },
        { "t", 41 },
        { "ɵ", 42 },
        { "ʊ", 43 },
        { "u", 44 },
        { "v", 45 },
        { "w", 46 },
        { "j", 47 },
        { "z", 48 },
        { "ʒ", 49 }
    };

    public int GetPhonemeNumber(string ipa)
    {
        if (IpaToNumber.TryGetValue(ipa, out int number))
        {
            return number;
        }

        Debug.LogWarning($"IPA phoneme '{ipa}' not found in map.");
        return -1; // Return -1 if not found
    }
}
