using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndiviualScreen : MonoBehaviour
{
    public Text hindiText;

    public Text phonemeText;

    public PhonemesMap phonemesMap;
    // Start is called before the first frame update
    void Start()
    {
        phonemeText.text = StaticData.selectedPhoneme;
        hindiText.text = "/" + phonemesMap.phonemeToHindi(StaticData.selectedPhoneme) + "/";
    }
}
