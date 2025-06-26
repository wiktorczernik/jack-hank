using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BonusSystem : MonoBehaviour
{
    [SerializeField] private BonusSettingsPreset settings;
    [SerializeField] private VehiclePhysics _playerVehicleController;

    private Dictionary<PlayerBonusTypes, InUpdateBonus> _inUpdateBonuses;
    private Dictionary<PlayerBonusTypes, ComboBonus> _comboBonuses;

    private void InitializeInUpdateBonuses()
    {
        _inUpdateBonuses = new Dictionary<PlayerBonusTypes, InUpdateBonus>()
        {
            { PlayerBonusTypes.Drift, new InUpdateBonus(settings.DriftIntervalInSeconds, settings.DriftBonus,
                CheckIsDrifting)},
            { PlayerBonusTypes.Flying, new InUpdateBonus(settings.FlyingIntervalInSeconds, settings.FlyingBonus,
                CheckIsFlying)},
        };
        _comboBonuses = new Dictionary<PlayerBonusTypes, ComboBonus>()
        {
            { PlayerBonusTypes.DestructionCombo, new ComboBonus(1) },
            { PlayerBonusTypes.VehicleDestruction, new ComboBonus(1) }
        };
    }
    
    private IEnumerator Start()
    {
        InitializeInUpdateBonuses();

        yield return new WaitUntil(() => GameManager.PlayerVehicle != null);

        _playerVehicleController = GameManager.PlayerVehicle.GetComponentInChildren<VehiclePhysics>();
        
        foreach (var smashable in FindObjectsByType<SmashableEntity>(FindObjectsSortMode.None))
        {
            UnityAction<SmashableEntity> onHit;

            if (smashable.TryGetComponent<Pickupable>(out Pickupable pickupable))
                continue;

            onHit = (smashable) => OnComboBonus(smashable, PlayerBonusTypes.DestructionCombo);

            smashable.OnHit.AddListener(onHit);
        }

        foreach (var vehicle in FindObjectsByType<SmashableEntityAnalogForVehicle>(FindObjectsSortMode.None))
        {
            vehicle.onHit.AddListener((smashable) => OnComboBonus(smashable, PlayerBonusTypes.VehicleDestruction));
        }
    }

    private void Update()
    {
        if (_playerVehicleController == null) return;

        foreach (var (bonusType, bonusState) in _inUpdateBonuses)
        {
            var isAfterBonus = Time.time - bonusState.LastBonusTime >= bonusState.timeIntervalInSeconds;
            var isBonusTaking = bonusState.checkBonus(bonusState);

            switch (isAfterBonus)
            {
                case true when isBonusTaking:
                    bonusState.LastBonusTime = Time.time;
                    bonusState.BonusPool += bonusState.bonus;
                    
                    GameManager.UpdateBonus(bonusState.bonus, bonusType, bonusState.BonusPool);
                    break;
                case true:
                    bonusState.BonusPool = 0;
                    break;
            }
        }
    }

    public void OnComboBonus(SmashableEntity smashable, PlayerBonusTypes bonusType)
    {
        var comboBonusState = _comboBonuses[bonusType];

        if (Time.time - comboBonusState.LastBonusTime <= comboBonusState.timeIntervalInSeconds) comboBonusState.combo++;
        else comboBonusState.combo = 1;

        var bonus = smashable.bountyPointsReward * comboBonusState.combo;
        comboBonusState.BonusPool += bonus;
        
        GameManager.UpdateCombo(bonusType, bonus, comboBonusState.combo, comboBonusState.BonusPool);
        
        comboBonusState.LastBonusTime = Time.time;
    }
    
    public void OnComboBonus(SmashableEntityAnalogForVehicle smashableEntityAnalogFor, PlayerBonusTypes bonusType)
    {
        var comboBonusState = _comboBonuses[bonusType];

        if (Time.time - comboBonusState.LastBonusTime <= comboBonusState.timeIntervalInSeconds) comboBonusState.combo++;
        else comboBonusState.combo = 1;

        var bonus = smashableEntityAnalogFor.bountyPointsReward * comboBonusState.combo;
        comboBonusState.BonusPool += bonus;
        
        GameManager.UpdateCombo(bonusType, bonus, comboBonusState.combo, comboBonusState.BonusPool);
        
        comboBonusState.LastBonusTime = Time.time;
    }
    
    private bool CheckIsDrifting(InUpdateBonus bonusState) => _playerVehicleController.isDrifting || _playerVehicleController.isDriftingRight;
    private bool CheckIsFlying(InUpdateBonus bonusState) => _playerVehicleController.wheels.All(w => !w.CheckGround());

    private class InUpdateBonus
    {
        public readonly float timeIntervalInSeconds;
        public readonly Func<InUpdateBonus, bool> checkBonus;
        public readonly int bonus;
        
        public float LastBonusTime;
        public int BonusPool;

        public InUpdateBonus(float timeIntervalInSeconds, int bonus, Func<InUpdateBonus, bool> checkBonus)
        {
            this.timeIntervalInSeconds = timeIntervalInSeconds;
            this.checkBonus = checkBonus;
            this.bonus = bonus;
        }
    }

    private class ComboBonus
    {
        public readonly float timeIntervalInSeconds;
        
        public int combo;
        public float LastBonusTime;
        public int BonusPool;

        public ComboBonus(float timeIntervalInSeconds)
        {
            this.timeIntervalInSeconds = timeIntervalInSeconds;
        }
    }
}
