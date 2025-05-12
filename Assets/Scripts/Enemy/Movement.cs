using System;
using UnityEngine;

namespace Enemy
{
    public class Movement : Entity
    {
        [SerializeField] private Transform[] targets;
        [SerializeField] private bool searchPlayer;
        [SerializeField] private bool isStatic;
        [SerializeField] private float maxSpeed;

        private Vector3 _currentTarget;
        private int _index;
        private int _old;

        private void OnEnable()
        {
            if (isStatic)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else if (searchPlayer)
            {
                _currentTarget = Vector3.zero;
            }
            else
            {
                UpdateTarget(0);
            }

            SpeedLimit = maxSpeed;
        }

        private void FixedUpdate()
        {
            HorVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
            Move(_currentTarget);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Flag"))
            {
                GetNewTarget();
            }
            else if (searchPlayer && other.CompareTag("Player"))
            {
                UpdateTarget(0);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (searchPlayer && other.CompareTag("Player"))
            {
                _currentTarget = Vector3.zero;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (searchPlayer && other.CompareTag("Player"))
            {
                UpdateTarget(_index);
            }
        }

        private void GetNewTarget()
        {
            
            if (_index >= targets.Length - 1)
            {
                _index = 0;
            }
            else
            {
                _index++;
            }
            
            UpdateTarget(_index);
            
        }

        private void UpdateTarget(int index)
        {
            _currentTarget = (targets[index].position - transform.position).normalized;
            _currentTarget.y = 0;
        }
    }
}
