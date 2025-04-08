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
    
    public static void Create(Vector3 worldPos, Quaternion rot, float force, float impactRad, float busDist, float shakeIntensity)
    {
        var inst = Instantiate(_main._largeExplosionPrefab, worldPos, rot);
        Explosion exp = inst.GetComponent<Explosion>();
        exp.force = force;
        exp.impactRadius = impactRad;
        exp.busDistance = busDist;
        exp.intensity = shakeIntensity;
        exp.Init();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Create(new Vector3(60, 79, 500), Quaternion.identity, 2500, 10, 300, 0.5f);
        }
    }
}
