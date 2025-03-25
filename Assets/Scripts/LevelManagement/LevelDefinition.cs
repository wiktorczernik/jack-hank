using LevelTask;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelManagement
{
    [CreateAssetMenu(menuName = "Level Definition", fileName = "New Definition")]
    public class LevelDefinition : ScriptableObject
    {
        [SerializeField] private int levelID;
        [SerializeField] private string sceneName;
        [SerializeField] private LevelDefinition[] lastLevels;
        [SerializeField] private LevelTaskDefinition[] levelTasks;
        [Header("Max values for scale levels: ")]
        [SerializeField] private int e;
        [SerializeField] private int d;
        [SerializeField] private int c;
        [SerializeField] private int b;
        [SerializeField] private int a;
        [SerializeField] private int s;

        public int LevelID => levelID;
       
        public string SceneName => sceneName;
        
        public LevelDefinition[] LastLevels => lastLevels.Clone() as LevelDefinition[];

        public LevelTaskDefinition[] LevelTasks => levelTasks.Clone() as LevelTaskDefinition[];

        public int[] ScaleLimits => new int[] { e, d, c, b, a, s };
    }
}
