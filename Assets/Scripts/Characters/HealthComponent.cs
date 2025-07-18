using Characters;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainPlayer
{
    public class HealthComponent : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        public int Health {  get; private set; }
        public bool IsDead { get {  return Health <= 0; } }

        private bool _isInvincible = false;

        [SerializeField]
        private int _maxHealth = 10;
        public int MaxHealth { get { return _maxHealth; } }

        public System.Action OnDeath;
        public System.Action<int, int> OnHealthChange;
        public System.Action<int, int> OnMaxHealthChange;

        [Title("Sentiment Thresholds")]
        [SerializeField]
        private int _happinessSentimentThreshold = 10;
        [SerializeField]
        private int _neutralSentimentThreshold = 7;
        [SerializeField]
        private int _annoyedSentimentThreshold = 4;
        [SerializeField]
        private int _angrySentimentThreshold = 2;

        private void Start()
        {
            ResetHealth();
        }

        public void SetInvincibility(bool isInvincible)
        {
            _isInvincible = isInvincible;
        }

        public void ChangeHealth(int amountToChange)
        {
            if (amountToChange < 0 && !_isInvincible)
            {
                RemoveHealth(amountToChange);
            }
            else if (amountToChange > 0)
            {
                HealHealth(amountToChange);   
            }
        }

        public void ChangeMaxHealth(int amountToChange)
        {
            if (amountToChange != 0)
            {
                int maxHealth = _maxHealth;
                _maxHealth += amountToChange;
                OnMaxHealthChange?.Invoke(maxHealth, _maxHealth);

                SetHealth(amountToChange > 0 ? Health + amountToChange : Health);
            }
        }

        private void RemoveHealth(int healthToRemove)
        {
            SetHealth(Health - Mathf.Abs(healthToRemove));
        }

        private void HealHealth(int healthToHeal)
        {
            SetHealth(Health + healthToHeal);
        }

        public void ResetHealth()
        {
            SetHealth(_maxHealth);
        }

        private void SetHealth(int health)
        {
            int previousHealth = Health;
            int newHealth = Mathf.Clamp(health, 0, _maxHealth);

            if (previousHealth != newHealth)
            {
                Health = newHealth;
                OnHealthChange?.Invoke(previousHealth, newHealth);

                if (Health <= 0)
                {
                    Die();
                }
            }
        }

        private void Die()
        {
            OnDeath?.Invoke();
        }

        public ECharacterSentiment GetCharacterSentiment()
        {
            if (Health <= _angrySentimentThreshold)
            {
                return ECharacterSentiment.Angry;
            }

            if (Health <= _annoyedSentimentThreshold)
            {
                return ECharacterSentiment.Annoyed;
            }

            if (Health <= _neutralSentimentThreshold)
            {
                return ECharacterSentiment.Neutral;
            }

            return ECharacterSentiment.Happy;
        }
    }
}
