using UnityEngine;

namespace LevelTask
{
    public abstract class LevelTaskDefinition : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private string description;

        public string Title => title;
        public string Description => description;
    }
}

