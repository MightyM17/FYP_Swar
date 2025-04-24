using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordGen : MonoBehaviour
{
    public Text displayText; 

    // public string[] phonemes = { "p", "h", "e", "t", "k", "l", "s", "tʃ", "d", "a" };
    string selectedPhoneme = StaticData.selectedPhoneme; // Default phoneme
    int index = 0;

    public PhonemeResultText phonemeResultText;
    public static Dictionary<string, List<string>> PhonemeWordMap = new Dictionary<string, List<string>>()
    {
        { "p", new List<string> { "पानी", "चंपक", "तोप", "पंखा", "सपना" } }, // init, medial, final
        { "h", new List<string> { "हाथी", "बहन", "सिंह", "महक", "हँस" } },
        { "e", new List<string> { "इनाम", "अरे", "फेर", "रेशम", "इटली" } },
        { "t", new List<string> { "ताल", "अंतर", "घंट", "तलवार", "सत" } },
        { "k", new List<string> { "किताब", "अंकुर", "बक", "काजू", "सड़क" } },
        { "l", new List<string> { "लड़का", "माला", "झील", "बालक", "लोटा" } },
        { "s", new List<string> { "संतरा", "कसक", "घास", "बस", "सुभाष" } },
        { "ʃ", new List<string> { "शेर", "रिश्ते", "नाश", "कुश", "शब्द" } },
        { "tʃ", new List<string> { "चूहा", "बच्चा", "कचरा", "पंच", "चोट" } },
        { "d", new List<string> { "दरवाजा", "अंदर", "गर्द", "डाल", "बद" } },
        { "a", new List<string> { "अनार", "सपना", "अमृत", "तप", "जनक" } }
    };

    void Start()
    {
        if (displayText != null)
        {
            var words = PhonemeWordMap[selectedPhoneme];
            string randomWord = words[index];
            displayText.text = randomWord;
            phonemeResultText.setResultText(randomWord);
        }
        else
        {
            Debug.LogError("Text component or words list is missing!");
        }
    }

    public void NextWord()
    {
        var words = PhonemeWordMap[selectedPhoneme];
        index = (index + 1) % words.Count;
        string randomWord = words[index];
        displayText.text = randomWord;
        phonemeResultText.setResultText(randomWord);
    }
}
