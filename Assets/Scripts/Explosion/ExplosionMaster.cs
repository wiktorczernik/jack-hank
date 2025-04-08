using UnityEngine;

public class ExplosionMaster : MonoBehaviour
{
    private static ExplosionMaster _main;

    [Header("Prefabs")]
    [SerializeField] GameObject _largeExplosionPrefab;


    private void Awake()
    {
        _main = this;
    }
    
    public static void Create(ExplosionProperties properties)
    {
        var inst = Instantiate(_main._largeExplosionPrefab, properties.epicenterPosition, Quaternion.identity);
        Explosion exp = inst.GetComponent<Explosion>();
        exp.properties = properties;
        exp.Init();
    }
}
