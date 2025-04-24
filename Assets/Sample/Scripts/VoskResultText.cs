using UnityEngine;
using UnityEngine.UI;
public class VoskResultText : MonoBehaviour 
{
    public VoskSpeechToText VoskSpeechToText;
    public Text ResultText;

    void Awake()
    {
        VoskSpeechToText.OnTranscriptionResult += OnTranscriptionResult;
    }

    private void OnTranscriptionResult(string obj)
    {
        Debug.Log(obj);
        // ResultText.text = "Recognized: ";
        var result = new RecognitionResult(obj);
        ResultText.text = result.Phrases[0].Text;
        // for (int i = 0; i < result.Phrases.Length; i++)
        // {
        //     ResultText.text += result.Phrases[0].Text;
        // }
    }
}
