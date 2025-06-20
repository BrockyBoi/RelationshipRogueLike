using MainPlayer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MainPlayer.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _healthText;

        private void Start ()
        {
            HealthComponent healthComponent = Player.Instance.HealthComponent;

            OnHealthChange(0, healthComponent.Health);
            healthComponent.OnHealthChange += OnHealthChange;
            healthComponent.OnMaxHealthChange += OnMaxHealthCHange;
        }

        private void OnDestroy()
        {
            if (Player.Instance != null && Player.Instance.HealthComponent != null)
            {
                Player.Instance.HealthComponent.OnHealthChange -= OnHealthChange;
                Player.Instance.HealthComponent.OnMaxHealthChange -= OnMaxHealthCHange;
            }
        }

        private void OnHealthChange(int oldHealth, int newHealth)
        {
            _healthText.text = newHealth + " / " + Player.Instance.HealthComponent.MaxHealth;
        }

        private void OnMaxHealthCHange(int oldHealth, int newHealth)
        {
            OnHealthChange(0, Player.Instance.HealthComponent.Health);
        }
    }
}
