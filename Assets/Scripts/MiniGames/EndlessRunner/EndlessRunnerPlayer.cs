using CatchingButterflies;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent (typeof(Rigidbody2D))]
    public class EndlessRunnerPlayer : MiniGameGameObject<EndlessRunnerSolver, EndlessRunnerGenerator>
    {
        [SerializeField, Required]
        private SpriteRenderer _spriteRenderer;

        [SerializeField, Required]
        private Rigidbody2D _rb2D;

        [SerializeField]
        private float _baseJumpForce = 100;

        [SerializeField]
        private float _timeCanIncreaseJumpHeight = 1.5f;

        [SerializeField, Range(0, 1)]
        private float _extraJumpForceMultiplier = .02f;

        [SerializeField, Range(0, 1)]
        private float _amountPerFrameToIncreaseGravity = .02f;

        private float _timeStartJump = -1;

        private bool _canContinueCurrentJump = false;

        private bool _prevInAir = false;

        private void Update()
        {
            if (IsOnGround() && Input.GetMouseButtonDown(0))
            {
                StartJump();
            }

            if (!IsOnGround())
            {
                if (Input.GetMouseButton(0) && _canContinueCurrentJump)
                {
                    AttemptContinueCurrentJump();
                }
                else if (!_canContinueCurrentJump)
                {
                    _rb2D.gravityScale += _amountPerFrameToIncreaseGravity;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                _canContinueCurrentJump = false;
            }

            if (IsOnGround() && _prevInAir)
            {
                OnPlayerLanded();
            }

            _prevInAir = IsOnGround();
        }

        private void StartJump()
        {
            _rb2D.AddForce(Vector2.up * _baseJumpForce);
            _timeStartJump = Time.time;
            _canContinueCurrentJump = true;
            _prevInAir = true;
        }
        
        private void AttemptContinueCurrentJump()
        {
            float currentTime = Time.time;
            float lastTimeCanJump = _timeStartJump + _timeCanIncreaseJumpHeight;
            if (currentTime < lastTimeCanJump)
            {
                float extraForce = Mathf.Lerp(_baseJumpForce * _extraJumpForceMultiplier, 0, currentTime / lastTimeCanJump);
                _rb2D.AddForce(Vector2.up * extraForce);
            }
            else
            {
                _canContinueCurrentJump = false;
            }
        }

        private bool IsOnGround()
        {
            float distance = .15f;
            Vector3 startPos = transform.position + (Vector3.down * ((_spriteRenderer.size.y / 2) + .1f));
            Debug.DrawLine(startPos, startPos + Vector3.down * distance, Color.blue, .1f);
            if (Physics2D.Raycast(startPos, Vector2.down, distance, LayerMask.NameToLayer("Ground")))
            {
                return true;
            }

            return false;
        }

        private void OnPlayerLanded()
        {
            _rb2D.gravityScale = 1;
        }

        private bool CanJump()
        {
            return IsOnGround();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            EndlessRunnerCollectable collectable = collision.gameObject.GetComponent<EndlessRunnerCollectable>();
            if (collectable)
            {
                collectable.CollectItem();
            }
        }
    }
}
