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
        }

        private void OnDestroy()
        {
            Player.Instance.HealthComponent.OnHealthChange -= OnHealthChange;
        }

        private void OnHealthChange(int oldHealth, int newHealth)
        {
            _healthText.text = "Health: " + newHealth + " / " + Player.Instance.HealthComponent.MaxHealth;
        }
    }
}
