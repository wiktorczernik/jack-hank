using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueMaster : MonoBehaviour
{
    public static DialogueMaster master;

    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;
    public List<AudioClip> dialogues;

    private AudioSource _source;

    public static bool isPlaying
    {
        get
        {
            if (!master) return false;
            if (!master._source) return false;
            return master._source.isPlaying;
        }
    }
    private bool oldIsPlaying = isPlaying;

    private void Update()
    {
        if (!_source) return;

        if (isPlaying != oldIsPlaying)
            if (isPlaying) onDialogueStart?.Invoke();
            else onDialogueEnd?.Invoke();
        oldIsPlaying = isPlaying;
    }

    private IEnumerator Start()
    {
        _source = GetComponent<AudioSource>();

        yield return new WaitUntil(() => master == null);
        master = this;
    }

    public static void ForceCallDialogue(int index)
    {
        master._source.clip = master.dialogues[index];
        master._source.Play();
    }
    public static void ForceCallDialogue(string name)
    {
        master._source.clip = master.dialogues.Find(b => b.name == name);
        master._source.Play();
    }
    public static void ForceCallDialogue(AudioClip clip)
    {
        master._source.clip = clip;
        master._source.Play();
    }
}
