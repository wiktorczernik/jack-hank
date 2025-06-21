using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BonusSystem : MonoBehaviour
{
    [SerializeField] private BonusSettingsPreset settings;

    private int _destructionCombo = 1;
    private float _lastHitTime;
    private int _hitBonusPool;

    private Dictionary<PlayerBonusTypes, InUpdateBonus> _inUpdateBonuses;

    [SerializeField]
    private VehiclePhysics _playerVehicleController;

    private void InitializeInUpdateBonuses()
    {
        _inUpdateBonuses = new Dictionary<PlayerBonusTypes, InUpdateBonus>()
        {
            { PlayerBonusTypes.Drift, new InUpdateBonus(settings.DriftIntervalInSeconds, settings.DriftBonus,
                CheckIsDrifting)},
            { PlayerBonusTypes.Flying, new InUpdateBonus(settings.FlyingIntervalInSeconds, settings.FlyingBonus,
                CheckIsFlying)},
        };
    }
    
    private IEnumerator Start()
    {
        InitializeInUpdateBonuses();

        yield return new WaitUntil(() => GameManager.PlayerVehicle != null);

        _playerVehicleController = GameManager.PlayerVehicle.GetComponentInChildren<VehiclePhysics>();
        
        foreach (var smashable in FindObjectsByType<SmashableEntity>(FindObjectsSortMode.None))
        {
            if (smashable.TryGetComponent<Pickupable>(out var pickup))
                continue;
            
            smashable.OnHit.AddListener(OnHitSmashable);
        }
    }

    private void Update()
    {
        if (_playerVehicleController == null) return;

        foreach (var (bonusType, bonusState) in _inUpdateBonuses)
        {
            var isAfterBonus = Time.time - bonusState.LastBonusTime >= bonusState.TimeIntervalInSeconds;
            var isBonusTaking = bonusState.CheckBonus(bonusState);

            switch (isAfterBonus)
            {
                case true when isBonusTaking:
                    bonusState.LastBonusTime = Time.time;
                    bonusState.BonusPool += bonusState.Bonus;
                    
                    GameManager.UpdateBonus(bonusState.Bonus, bonusType, bonusState.BonusPool);
                    break;
                case true:
                    bonusState.BonusPool = 0;
                    break;
            }
        }
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
    
    private bool CheckIsDrifting(InUpdateBonus bonusState) => _playerVehicleController.isDrifting || _playerVehicleController.isDriftingRight;
    private bool CheckIsFlying(InUpdateBonus bonusState) => _playerVehicleController.wheels.All(w => !w.CheckGround());

    private class InUpdateBonus
    {
        public readonly float TimeIntervalInSeconds;
        public readonly Func<InUpdateBonus, bool> CheckBonus;
        public readonly int Bonus;
        
        public float LastBonusTime;
        public int BonusPool;

        public InUpdateBonus(float timeIntervalInSeconds, int bonus, Func<InUpdateBonus, bool> checkBonus)
        {
            TimeIntervalInSeconds = timeIntervalInSeconds;
            CheckBonus = checkBonus;
            Bonus = bonus;
        }
    }
}
