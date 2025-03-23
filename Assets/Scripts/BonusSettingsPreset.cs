using UnityEngine;

[CreateAssetMenu(fileName = "BonusSettingsPreset", menuName = "BonusSettingsPreset")]
public class BonusSettingsPreset : ScriptableObject
{
    [SerializeField] private float comboMaxIntervalInSeconds;
    [SerializeField] private int driftBonus;
    [SerializeField] private float driftIntervalInSeconds;
    [SerializeField] private int flyingBonus;
    [SerializeField] private float flyingIntervalInSeconds;
    
    public float ComboMaxIntervalInSeconds => comboMaxIntervalInSeconds;
    public int DriftBonus => driftBonus;
    public float DriftIntervalInSeconds => driftIntervalInSeconds;
    public int FlyingBonus => flyingBonus;
    public float FlyingIntervalInSeconds => flyingIntervalInSeconds;

}