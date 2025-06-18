using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class Bonus_GUI : MonoBehaviour
{
    [SerializeField] private Vector2 offsetFromCenterToRight;
    [SerializeField] private int circleRadius;
    [SerializeField] private int curveBeginningInCelsius;
    [Min(0)] [SerializeField] private int ticketsOffsetInCelsius;
    [TextArea] [SerializeField] private string passengerKilledMassage;
    [TextArea] [SerializeField] private string passengerPickedUpMassage;
    [TextArea] [SerializeField] private string largeSiteDestroyedMassage;
    [Range(0, 1)][SerializeField] private float maxOffsetFromCenterInPercents = 0.2f;
    
    [Header("Debug")]
    [SerializeField] private float bonusUIx;
    [SerializeField] private bool showCircle;
    
    
    private readonly PlayerBonusTypes[] _bigBonuses = { PlayerBonusTypes.Passenger, PlayerBonusTypes.LargeDestruction };

    private readonly PlayerBonusTypes[] _miniBonuses =
        { PlayerBonusTypes.Drift, PlayerBonusTypes.Flying, PlayerBonusTypes.DestructionCombo, PlayerBonusTypes.VehicleDestruction };

    private BigBonusBoard_GUI _bigBonusBoard;
    private LinkedList<Tuple<int, string>> _bigBonusesList;
    private BonusTicket_GUI[] _bonusTickets;
    private RectTransform[] _bonusTicketsRect;
    private RectTransform _rect;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        _bonusTickets = new BonusTicket_GUI[_miniBonuses.Length];
        _bonusTicketsRect = new RectTransform[_miniBonuses.Length];
        _bigBonusBoard = GetComponentInChildren<BigBonusBoard_GUI>();
        _bigBonusBoard.OnShowingEnded += ShowBigBonusBoard;
        _bigBonusesList = new LinkedList<Tuple<int, string>>();

        if (_bigBonusBoard == null) throw new Exception("There is no bonus board");
        if (_rect == null) throw new Exception("There is no rect");

        for (var i = 0; i < _miniBonuses.Length; i++)
        {
            var ticket = GetComponentsInChildren<BonusTicket_GUI>(true)
                .FirstOrDefault(c => c.BonusType == _miniBonuses[i]);

            if (ticket == null)
            {
                Debug.LogError($"BonusTicket of type '{_miniBonuses[i].ToString()}' was not found");
                return;
            }

            _bonusTickets[i] = ticket;
            _bonusTicketsRect[i] = ticket.GetComponent<RectTransform>();
        }

        UpdateTicketsPosition();
    }

    private void Update()
    {
        UpdateTicketsPosition();
    }

    public void ShowBonus(int bonus, PlayerBonusTypes bonusType)
    {
        if (bonusType == PlayerBonusTypes.DestructionCombo)
        {
            Debug.LogError("There is another way to show destruction combo bonus");
            return;
        }

        if (_bigBonuses.Any(bigBonus => bigBonus == bonusType)) ShowBigBonus(bonus, bonusType);
        else ShowMiniBonus(bonus, bonusType);
    }

    public void ShowComboBonus(PlayerBonusTypes bonusType, int bonus, int combo)
    {
        if (bonusType != PlayerBonusTypes.DestructionCombo && bonusType != PlayerBonusTypes.VehicleDestruction)
        {
            Debug.LogError($"Bonus '{bonusType}' is not combo bonus");
            return;
        }
        
        _bonusTickets.First(t => t.BonusType == bonusType)
            .ChangeBonusValueOn(bonus, combo);
    }

    private void UpdateTicketsPosition()
    {
        var playerXOnCanvas = GetPlayerXOnCanvas();
        
        _rect.anchoredPosition = new Vector2(playerXOnCanvas, _rect.anchoredPosition.y);
        
        for (var i = 0; i < _bonusTicketsRect.Length; i++)
        {
            var ticketRect = _bonusTicketsRect[i];

            if (playerXOnCanvas >= 0)
            {
                var angleOnCircle = (curveBeginningInCelsius + ticketsOffsetInCelsius * i) * Mathf.Deg2Rad;
                
                var x = (float)(Math.Cos(angleOnCircle) * circleRadius + offsetFromCenterToRight.x - circleRadius);
                var y = (float)(Math.Sin(angleOnCircle) * circleRadius + offsetFromCenterToRight.y);

                ticketRect.anchoredPosition = new Vector2(x, y);
                ticketRect.rotation = Quaternion.AngleAxis(angleOnCircle * Mathf.Rad2Deg, Vector3.forward);
                _bonusTickets[i].SetUsualDirection();
            }
            else
            {
                var angleOnCircle = (180 - curveBeginningInCelsius - ticketsOffsetInCelsius * i) * Mathf.Deg2Rad;
                
                var x = (float)(Math.Cos(angleOnCircle) * circleRadius - offsetFromCenterToRight.x + circleRadius);
                var y = (float)(Math.Sin(angleOnCircle) * circleRadius + offsetFromCenterToRight.y);
                
                ticketRect.anchoredPosition = new Vector2(x, y);

                var flippedRotation = -(curveBeginningInCelsius + ticketsOffsetInCelsius * i);
                
                ticketRect.rotation = Quaternion.AngleAxis(flippedRotation, Vector3.forward);
                _bonusTickets[i].SetReverseDirection();
            }
        }
    }

    private float GetPlayerXOnCanvas()
    {
        if (!Application.isPlaying) return bonusUIx;
        
        var cameraPos = Camera.main.transform.position;
        var cameraFov = Camera.main.fieldOfView;
        var cameraAspect = Camera.main.aspect;
        var cameraDir = Camera.main.transform.forward;
        var cameraRightDir = Camera.main.transform.right;
        
        
        // Oblicza rozmiary stożka widzenia kamery (frustum) w pozycji gracza
        var vectorToPlayer = GameManager.PlayerVehicle.gameObject.transform.position - cameraPos;
        var frustumDepthOnPlayerPos = Vector3.Dot(vectorToPlayer, cameraDir) * vectorToPlayer.magnitude;
        var frustumHeightOnPlayerPos = 2 * frustumDepthOnPlayerPos * Math.Tan(cameraFov / 2 * Mathf.Deg2Rad);
        var frustumWidthOnPlayerPos = frustumHeightOnPlayerPos * cameraAspect;
        
        // Oblicza wektor od centrum frustum do pozycji gracza
        var frustumCenter = frustumDepthOnPlayerPos * cameraDir;
        var vectorFromCenterToPlayer = vectorToPlayer - frustumCenter;
        
        // Za pomocą wektora od centrum frustum do gracza wyliczne jest przesunięcie w prawo od środka frustum i
        // przelicza ten odstęp na rozmiary całego ekranu
        var cosBetweenCenterAndPlayerPos = Vector3.Dot(vectorFromCenterToPlayer, cameraRightDir);
        var playerXOnScreen =
            (float)((cosBetweenCenterAndPlayerPos * vectorFromCenterToPlayer.magnitude) * (Screen.width / frustumWidthOnPlayerPos));
        
        // Ogranicza maksymalny dystans od środka ekranu
        var rangedX = Math.Sign(playerXOnScreen) * Math.Min(maxOffsetFromCenterInPercents * Screen.width, Math.Abs(playerXOnScreen));

        return rangedX;
    }

    private void ShowBigBonus(int bonus, PlayerBonusTypes bonusTypes)
    {
        var message = bonusTypes.ToString();

        if (bonusTypes == PlayerBonusTypes.LargeDestruction)
            message = largeSiteDestroyedMassage;
        else if (bonusTypes == PlayerBonusTypes.Passenger && bonus >= 0)
            message = passengerPickedUpMassage;
        else if (bonusTypes == PlayerBonusTypes.Passenger)
            message = passengerKilledMassage;

        _bigBonusesList.AddLast(new Tuple<int, string>(bonus, message));
        ShowBigBonusBoard();
    }

    private void ShowMiniBonus(int bonus, PlayerBonusTypes bonusType)
    {
        var ticket = _bonusTickets.First(ticket => ticket.BonusType == bonusType);

        ticket.ChangeBonusValueOn(bonus);
    }

    private void ShowBigBonusBoard()
    {
        if (_bigBonusBoard.IsShowing
            || _bigBonusesList.Count == 0) return;

        var dataToShow = _bigBonusesList.First.Value;
        _bigBonusesList.RemoveFirst();
        _bigBonusBoard.ShowBigBonus(dataToShow.Item1, dataToShow.Item2);
    }

    private void OnDrawGizmos()
    {
        if (!showCircle) return;
        
        Gizmos.color = Color.red;
        var playerXOnScreen = GetPlayerXOnCanvas();

        // Wyliczony środek i faktyczny są różne — nie wiem dokładnie dlaczego, ale to pomaga uzyskać prawidłowy wygląd
        var strangeOffset = 25;

        for (var i = 0f; i < Mathf.PI * 2; i += Mathf.PI / 36)
        {
            float x;
            
            if (playerXOnScreen >= 0)
            {
                x = (float) Math.Cos(i) * circleRadius / 2 - circleRadius / 2f + offsetFromCenterToRight.x / 2 + Screen.width / 2f;
            }
            else
            {
                x = (float) Math.Cos(i) * circleRadius / 2 + circleRadius / 2f - offsetFromCenterToRight.x / 2 + Screen.width / 2f;
            }
            
            var y = (float)Math.Sin(i) * circleRadius / 2 + offsetFromCenterToRight.y + Screen.height / 2f - strangeOffset;
            
            Gizmos.DrawSphere(new Vector3(x, y, 0), 3);
        }
    }
}