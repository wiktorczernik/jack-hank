using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class PlayerBonusDetector : MonoBehaviour
{
    public void Prepare()
    {
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

    private void OnPassengerHit(SmashableEntity smashable)
    {
        GameManager.ChangePlayerBonusBy(-((PickupablePassenger)smashable).bountyPointsPenalty, PlayerBonusTypes.Passenger);
        Debug.Log("Passenger was hit! Oh no!");
    }
    
    private void OnPassengerPickup(TriggerEventEmitter trigger, PickupablePassenger passenger)
    {
        GameManager.ChangePlayerBonusBy(passenger.bountyPointsReward, PlayerBonusTypes.Passenger);
        // OnBigBounty?.Invoke("FRIEND CAUGHT", passenger.bountyPointsPenalty);
    }
    
    private void OnHitSmashable(SmashableEntity smashable)
    {
        GameManager.ChangePlayerBonusBy(smashable.bountyPointsReward, PlayerBonusTypes.DestructionCombo, 1);
    }
}
