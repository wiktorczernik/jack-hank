using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(TriggerEventEmitter))]
public class DialogueTrigger : MonoBehaviour
{
    TriggerEventEmitter emitter;
    public string dialogueName;
    public int dialogueIndex;
    public AudioClip dialogue;

    public DialogueSearchMethod targetDialogue;

    private void Awake()
    {
        emitter = GetComponent<TriggerEventEmitter>();
    }

    void Enter(Collider collider)
    {
        if (!collider.transform.parent) return;
        if (!collider.transform.parent.TryGetComponent(out PlayerVehicle vehicle)) return;

        if (!DialogueMaster.master) return;
        switch (targetDialogue)
        {
            case DialogueSearchMethod.index:
                DialogueMaster.ForceCallDialogue(dialogueIndex); break;
            case DialogueSearchMethod.name:
                DialogueMaster.ForceCallDialogue(name); break;
            case DialogueSearchMethod.argument:
                DialogueMaster.ForceCallDialogue(dialogue); break;
        }
    }

    private void OnEnable()
    {
        emitter.OnEnter.AddListener(Enter);
    }
    private void OnDisable()
    {
        emitter.OnEnter.RemoveListener(Enter);
    }

    public enum DialogueSearchMethod
    {
        index,
        name,
        argument
    }
}
