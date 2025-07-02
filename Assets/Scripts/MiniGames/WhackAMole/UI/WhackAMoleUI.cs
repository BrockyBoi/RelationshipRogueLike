using CustomUI;
using System.Collections;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

namespace WhackAMole.UI
{
    public class WhackAMoleUI : GameUI<WhackAMoleGenerator, WhackAMoleSolver>
    {
        public static WhackAMoleUI Instance { get; private set; }

        protected override WhackAMoleGenerator GameGenerator { get { return WhackAMoleGenerator.Instance; } }

        protected override WhackAMoleSolver GameSolver { get { return WhackAMoleSolver.Instance; } }

        bool _isRunningTakeDamageAnimation = false;

        [SerializeField]
        private float _takeDamageAnimTime = .5f;

        [SerializeField, Required]
        private TextMeshProUGUI _healthText;

        private void Awake()
        {
            Instance = this;
            HideUI();
        }

        protected override void Start()
        {
            base.Start();

            if (GameSolver)
            {
                GameSolver.OnCountChange += OnCountChange;
            }
        }

        protected override void OnDestroy()
        {
            if (GameSolver)
            {
                GameSolver.OnCountChange -= OnCountChange;
            }
        }

        protected override void OnGameStart()
        {
            base.OnGameStart();

            _healthText.enabled = true;
            SetEnemyCountText();
        }

        protected override void OnGameEnd()
        {
            base.OnGameEnd();

            
            _healthText.color = Color.white;
            _healthText.enabled = false;
        }

        private void OnCountChange()
        {
            SetEnemyCountText();

            if (!_isRunningTakeDamageAnimation)
            {
                StartCoroutine(FlashHealthTextRed());
            }
        }

        private void SetEnemyCountText()
        {
            _healthText.text = GameSolver.EnemiesBeaten + " / " + GameSolver.GameData.EnemiesNeededToBeat;
        }

        private IEnumerator FlashHealthTextRed()
        {
            if (!_isRunningTakeDamageAnimation)
            {
                _isRunningTakeDamageAnimation = true;
                float animTime = 0;
                float halfTime = _takeDamageAnimTime / 2;
                while (animTime < halfTime)
                {
                    animTime += Time.deltaTime;
                    _healthText.color = Color.Lerp(Color.white, Color.red, animTime / halfTime);
                    yield return null;
                }

                while (animTime < _takeDamageAnimTime)
                {
                    animTime += Time.deltaTime;
                    _healthText.color = Color.Lerp(Color.red, Color.white, (animTime - halfTime) / halfTime);
                    yield return null;
                }

                _healthText.color = Color.white;
                _isRunningTakeDamageAnimation = false;
            }
        }
    }
}
