using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
public class PhonemeResultText : MonoBehaviour 
{
    public VoskSpeechToText VoskSpeechToText;
    public PhonemesMap PhonemesMap;
    public Text inputText;
    public Text resultText;

    void Awake()
    {
        VoskSpeechToText.OnTranscriptionResult += OnTranscriptionResult;
    }

    private async void OnTranscriptionResult(string obj)
    {
        await Task.Delay(1000);
        resultText.text = "Phonemes Recognized: ";
        var result = new RecognitionResult(obj);
        Debug.Log(inputText.text);
        resultText.text += PhonemesMap.lesgo(inputText.text);
    }

    public void setResultText(string text)
    {
        string converted = PhonemesMap.lesgo(inputText.text);
        resultText.text = "Phonemes Recognized: " + converted;
    }
}
