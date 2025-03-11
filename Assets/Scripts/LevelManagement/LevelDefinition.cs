using LevelTask;
using UnityEngine;

namespace LevelManagement
{
    [CreateAssetMenu(menuName = "Level Definition", fileName = "New Definition")]
    public class LevelDefinition : ScriptableObject
    {
        [SerializeField] private int levelID;
        [SerializeField] private string sceneName;
        [SerializeField] private LevelDefinition[] lastLevels;
        [SerializeField] private LevelTaskDefinition[] levelTasks;

        public int LevelID => levelID;
       
        public string SceneName => sceneName;
        
        public LevelDefinition[] LastLevels => lastLevels.Clone() as LevelDefinition[];

        public LevelTaskDefinition[] LevelTasks => levelTasks.Clone() as LevelTaskDefinition[];
    }
}
