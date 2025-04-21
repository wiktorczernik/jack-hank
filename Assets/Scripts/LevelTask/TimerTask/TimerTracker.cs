using System;

namespace LevelTask
{
    public class TimerTracker : LevelTaskTracker
    {
        private TimerLevelTask _task;
        
        public override bool IsComplete()
        {
            return GameManager.RunInfo.Time >= _task.TimerDurationInSeconds * 1000;
        }    
        
        protected override LevelTaskTracker Initialize(LevelTaskDefinition task)
        {
            if (task is not TimerLevelTask timerTask) throw new NotSupportedException();

            _task = timerTask;

            return this;
        }
    }
}

