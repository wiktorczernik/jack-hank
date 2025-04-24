using UnityEngine;

public class ExplosionMaster : MonoBehaviour
{
    private static ExplosionMaster _main;

    [Header("Prefabs")]
    [SerializeField] GameObject[] _explosionPrefabs;


    private void Awake()
    {
        _main = this;
    }
    
    public static void Create(ExplosionProperties properties, int explosionIndex = 0)
    {
        var inst = Instantiate(_main._explosionPrefabs[explosionIndex], properties.epicenterPosition, Quaternion.identity);
        Explosion exp = inst.GetComponent<Explosion>();
        exp.properties = properties;
        exp.Init();
    }
}
