using Dialogue;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame.Results
{
    [Serializable]
    public abstract class GameCompletionResult
    {
        public StandardDialogueObject PotentialPlayerDialogue;

        public GameHealthResult HealthResult;

        public virtual void ApplyEffects()
        {
            HealthResult.ApplyEffect();
        }
    }

    [Serializable]
    public abstract class GameResult
    {
        public abstract void ApplyEffect();
    }

    [Serializable]
    public class GameHealthResult : GameResult
    {
        [Range(-10, 10)]
        public int HealthAmountToChange = 0;

        public override void ApplyEffect()
        {
            int healthToChange = HealthAmountToChange;
            if (healthToChange != 0)
            {
                MainPlayer.Player.Instance.HealthComponent.ChangeHealth(healthToChange);
            }
        }
    }
}
