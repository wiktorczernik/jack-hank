using System;
using UnityEngine;

namespace LevelTask
{
    public class CounterTracker : LevelTaskTracker
    {
        private CounterLevelTask _task;
        private int _collectedAmount;

        public int CollectedAmount => _collectedAmount;

        public override bool IsComplete()
        {
            return _collectedAmount >= _task.AmountToCollect;
        }

        protected override LevelTaskTracker Initialize(LevelTaskDefinition task)
        {
            if (task is not CounterLevelTask counterTask) throw new NotSupportedException();

            _task = counterTask;
            GameManager.PlayerVehicle.vehicleCollision.OnEnter.AddListener(HandlePlayerCollision);

            return this;
        }

        private void HandlePlayerCollision(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent<SmashableEntity>(out var smashableEntity) 
                && smashableEntity.SmashableType != _task.SmashableType) return;
            
            _collectedAmount++;
        }
    }
}


