using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainPlayer
{
    [RequireComponent(typeof(HealthComponent))]
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }
        public HealthComponent HealthComponent {  get; private set; }

        private void Awake()
        {
            Instance = this;
            HealthComponent = GetComponent<HealthComponent>();
        }
    }
}
