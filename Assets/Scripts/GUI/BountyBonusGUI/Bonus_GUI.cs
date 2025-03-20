using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class Bonus_GUI : MonoBehaviour
{
    [SerializeField] private Vector2 offsetFromCenterToRight;
    [SerializeField] private int circleRadius;
    [Min(0)][SerializeField] private int curveBeginningInCelsius;
    [Min(0)][SerializeField] private int ticketsOffsetInCelsius;
    
    private readonly PlayerBonusTypes[] _miniBonuses =
        { PlayerBonusTypes.Drift, PlayerBonusTypes.Flying, PlayerBonusTypes.DestructionCombo };

    private readonly PlayerBonusTypes[] _bigBonuses = { PlayerBonusTypes.Passenger };
    private BonusTicket_GUI[] _bonusTickets;
    private RectTransform[] _bonusTicketsRect;

    public void ShowBonus(int bonus, PlayerBonusTypes bonusType)
    {
        if (bonusType == PlayerBonusTypes.DestructionCombo)
            throw new Exception("There is another way to show destruction combo bonus");
        
        if (_bigBonuses.Any(bigBonus => bigBonus == bonusType)) return;
        
        var ticket = _bonusTickets.First(ticket => ticket.BonusType == bonusType);

        ticket.ChangeBonusValueOn(bonus);
    }

    public void ShowDestructionComboBonus(int bonus, int combo)
    {
        _bonusTickets.First(t => t.BonusType == PlayerBonusTypes.DestructionCombo)
            .ChangeBonusValueOn(bonus, combo);
    }
    
    private void Start()
    {
        _bonusTickets = new BonusTicket_GUI[_miniBonuses.Length];
        _bonusTicketsRect = new RectTransform[_miniBonuses.Length];
        
        for (var i = 0; i < _miniBonuses.Length; i++)
        {
            var ticket = GetComponentsInChildren<BonusTicket_GUI>(true)
                .FirstOrDefault(c => c.BonusType == _miniBonuses[i]);

            if (ticket == null)
                throw new Exception($"BonusTicket of type '{_miniBonuses[i].ToString()}' was not found");

            _bonusTickets[i] = ticket;
            _bonusTicketsRect[i] = ticket.GetComponent<RectTransform>();
        }
        
        UpdateTicketsPosition();
    }

    private void Update()
    {
        if (Application.isPlaying) return;
        
        UpdateTicketsPosition();
    }

    private void UpdateTicketsPosition()
    {
        for (var i = 0; i < _bonusTicketsRect.Length; i++)
        {
            var rect = _bonusTicketsRect[i];
            var angleOnCurveInCelsius = curveBeginningInCelsius + ticketsOffsetInCelsius * i;
            var angleOnCurveInRadians = angleOnCurveInCelsius * (math.PI / 180);
            var x = (float)(Math.Cos(angleOnCurveInRadians) * circleRadius + offsetFromCenterToRight.x - circleRadius + rect.rect.width / 2);
            var y = (float)(Math.Sin(angleOnCurveInRadians) * circleRadius + offsetFromCenterToRight.y);
            
            rect.anchoredPosition = new Vector2(x, y);
            rect.rotation = Quaternion.AngleAxis(angleOnCurveInCelsius, Vector3.forward);
        }
    }
}

