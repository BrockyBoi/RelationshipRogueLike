using Dialogue;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame.Results
{
    [Serializable]
    public abstract class GameCompletionResult
    {
        public List<StandardDialogueObject> DialogueResponses;
        public DialogueHealthResult HealthResult;

        public virtual void ApplyEffects()
        {
            int healthToChange = HealthResult.HealthAmountToChange;
            if (healthToChange != 0)
            {
                MainPlayer.Player.Instance.HealthComponent.ChangeHealth(healthToChange);
            }
        }
    }

    [Serializable]
    public class DialogueHealthResult
    {
        [Range(-10, 10)]
        public int HealthAmountToChange = 0;
    }
}
