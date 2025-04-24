using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public class GameController : MonoBehaviour
{
    int score = 0;
    int total = 5;

    public Text originalPhonemeText;
    public Text spokenPhonemeText;
    public Text comparisonResultText;

    public VoskSpeechToText VoskSpeechToText;

    void Awake()
    {
        VoskSpeechToText.OnTranscriptionResult += ComparePhonemes;
    }

    private async void ComparePhonemes(string transcriptionJson)
    {
        await Task.Delay(1100);
        // You already set the texts somewhere else, so we parse them here
        List<string> expectedPhonemes = ExtractPhonemes(originalPhonemeText.text);
        List<string> spokenPhonemes = ExtractPhonemes(spokenPhonemeText.text);

        List<string> resultLines = new List<string>();
        int correctCount = 0;

        int compareCount = Mathf.Min(expectedPhonemes.Count, spokenPhonemes.Count);
        for (int i = 0; i < compareCount; i++)
        {
            string expected = expectedPhonemes[i];
            string spoken = spokenPhonemes[i];

            if (expected == spoken)
            {
                resultLines.Add($"✅ {expected}");
                correctCount++;
            }
            else
            {
                resultLines.Add($"❌ Expected {expected} → Heard {spoken}");
            }
        }

        // Extra expected or spoken phonemes (not aligned)
        if (expectedPhonemes.Count > spokenPhonemes.Count)
        {
            for (int i = spokenPhonemes.Count; i < expectedPhonemes.Count; i++)
            {
                resultLines.Add($"❌ Expected {expectedPhonemes[i]} → Heard [none]");
            }
        }
        else if (spokenPhonemes.Count > expectedPhonemes.Count)
        {
            for (int i = expectedPhonemes.Count; i < spokenPhonemes.Count; i++)
            {
                resultLines.Add($"⚠️ Extra: Heard {spokenPhonemes[i]}");
            }
        }

        // Count full-word match only
        if (correctCount == expectedPhonemes.Count && expectedPhonemes.Count == spokenPhonemes.Count)
        {
            score++;
            resultLines = new List<string> { "✅ Full match!" };
        }

        resultLines.Add($"Score: {score}/{total}");

        comparisonResultText.text = string.Join("\n", resultLines);
        Debug.Log($"Score: {score}/{total}");
    }

    private List<string> ExtractPhonemes(string rawText)
    {
        string[] possiblePrefixes = { "Phonemes Recognized:", "Phonemes Detected:" };

        foreach (var prefix in possiblePrefixes)
        {
            if (rawText.StartsWith(prefix))
            {
                string phonemePart = rawText.Substring(prefix.Length).Trim();
                var phonemes = phonemePart.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                return phonemes.ToList();
            }
        }

        // If no known prefix found, return empty list or log warning
        Debug.LogWarning("Unrecognized phoneme format: " + rawText);
        return new List<string>();
    }

}
