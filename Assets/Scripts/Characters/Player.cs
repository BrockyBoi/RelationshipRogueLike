using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainPlayer
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(PlayerModifiersComponent))]
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }
        public HealthComponent HealthComponent {  get; private set; }
        public PlayerModifiersComponent PlayerModifiersComponent { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            HealthComponent = GetComponent<HealthComponent>();
            PlayerModifiersComponent = GetComponent<PlayerModifiersComponent>();
        }
    }
}
