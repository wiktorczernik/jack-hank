using UnityEngine;
using UnityEngine.Events;

public class BonusSystem : MonoBehaviour
{
    [SerializeField] private BonusSettingsPreset settings;

    private int _destructionCombo = 1;
    private float _lastHitTime;
    private int _hitBonusPool;
    private float _lastDriftTime;
    private int _driftBonusPool;
    private float _lastFlyingTime;
    private int _flyingBonusPool;
    
    private CarController _playerCar;
    
    private void Start()
    {
        _playerCar = GameManager.PlayerVehicle.GetComponent<CarController>();
        GameManager.PlayerVehicle.onPickupPassenger.AddListener(OnPassengerPickup);
        
        foreach (var smashable in FindObjectsByType<SmashableEntity>(FindObjectsSortMode.None))
        {
            UnityAction<SmashableEntity> onHit;
            
            switch (smashable)
            {
                case PickupablePassenger passenger:
                    passenger.StartLookingForPlayerVehicle(GameManager.PlayerVehicle);
                    onHit = OnPassengerHit;
                    break;
                default:
                    onHit = OnHitSmashable;
                    break;
            }
            
            smashable.OnHit.AddListener(onHit);
        }
    }

    private void Update()
    {
        var isAfterDrift = Time.time - _lastDriftTime >= settings.DriftIntervalInSeconds;
        var isDrifting = _playerCar.isDrifting || _playerCar.isDriftingRight;
        
        switch (isAfterDrift)
        {
            case true when isDrifting:
                _lastDriftTime = Time.time;
                _driftBonusPool += settings.DriftBonus;

                GameManager.UpdateBonus(settings.DriftBonus, PlayerBonusTypes.Drift, _driftBonusPool);
                break;
            case true:
                _driftBonusPool = 0;
                break;
        }
    }

    private void OnPassengerHit(SmashableEntity smashable)
    {
        GameManager.UpdateBonus(-((PickupablePassenger)smashable).bountyPointsPenalty, PlayerBonusTypes.Passenger);
        Debug.Log("Passenger was hit! Oh no!");
    }
    
    private void OnPassengerPickup(TriggerEventEmitter trigger, PickupablePassenger passenger)
    {
        GameManager.UpdateBonus(passenger.bountyPointsReward, PlayerBonusTypes.Passenger);
        // OnBigBounty?.Invoke("FRIEND CAUGHT", passenger.bountyPointsPenalty);
    }
    
    private void OnHitSmashable(SmashableEntity smashable)
    {
        if (Time.time - _lastHitTime <= settings.ComboMaxIntervalInSeconds) _destructionCombo++;
        else _destructionCombo = 1;

        var bonus = smashable.bountyPointsReward * _destructionCombo;
        _hitBonusPool += bonus;
        
        GameManager.UpdateDestructionCombo(bonus, _destructionCombo, _hitBonusPool);

        _lastHitTime = Time.time;
    }
}
