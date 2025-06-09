using System;
using System.Text;
using JackHank.Dialogs;
using TMPro;
using UnityEngine;

public class DialogTranscriptions_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text _transcriptionLabel;

    StringBuilder _textBuilder = new();

    private void OnEnable()
    {
        DialogPlayer.onTranscriptionBegin += ShowTranscription;
        DialogPlayer.onTranscriptionEnd += HideTranscription;
    }
    private void OnDisable()
    {
        DialogPlayer.onTranscriptionBegin -= ShowTranscription;
        DialogPlayer.onTranscriptionEnd -= HideTranscription;
    }

    private void ShowTranscription(VoicelineTranscription transcription)
    {
        _textBuilder.Clear();
        _textBuilder.Append(transcription.character.displayName);
        _textBuilder.Append(": ");
        _textBuilder.Append(transcription.text);

        _transcriptionLabel.text = _textBuilder.ToString();
    }
    private void HideTranscription(VoicelineTranscription transcription)
    {
        _transcriptionLabel.text = string.Empty;
    }
}
