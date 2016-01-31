using System;
using UnityEngine;

namespace Assets.Scripts.PlayerClasses
{
    public class PlayerInformation : MonoBehaviour
    {
        [Serializable]
        public class PlayerStats
        {
            public string playerName;

            [Range(0, 10000)]
            public float maxHealth = 100;

            private float currentHealth;
            public float CurrentHealth
            {
                get
                {
                    return currentHealth;
                }
                set
                {
                    currentHealth = Mathf.Clamp(value, 0f, maxHealth);
                }
            }

            public void Init()
            {
                currentHealth = maxHealth;
            }
        }

        public PlayerStats stats = new PlayerStats();

        private void Awake()
        {
            stats.Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                DamagePlayer(10);
            if (stats.CurrentHealth <= 0)
                stats.Init();
        }

        public void DamagePlayer(int damage)
        {
            stats.CurrentHealth -= damage;
        }
    }
}
