using UnityEngine;

namespace Enemy
{
    public class Movement : Entity
    {
        [SerializeField] private Transform[] targets;
        [SerializeField] private bool searchPlayer;
        [SerializeField] private bool isStatic;
        [SerializeField] private float maxSpeed;
        [SerializeField] private Animator controller;

        private int _index;
        private int _old;
        private bool _shouldStop;

        private void OnEnable()
        {
            if (isStatic)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
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
            if (!_shouldStop)
            {
                Move(transform.forward);
            }

            controller?.SetFloat(Animator.StringToHash("Velocity"), HorVelocity.magnitude);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Flag") && targets[_index].transform.position == other.transform.position)
            {
                controller?.SetTrigger(Animator.StringToHash("Stop"));
                _shouldStop = true;
            }
            else if (searchPlayer && other.CompareTag("Player"))
            {
                UpdateTarget(0);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (searchPlayer && other.CompareTag("Player"))
            {
                UpdateTarget(_index);
            }
        }

        public void GetNewTarget()
        {
            if (_index >= targets.Length - 1)
            {
                _index = 0;
            }
            else
            {
                _index++;
            }

            _shouldStop = false;
            UpdateTarget(_index);
        }

        private void UpdateTarget(int index)
        {
            var target = (targets[index].transform.position - transform.position);
            target.y = 0;
            transform.rotation = Quaternion.LookRotation(target);
        }
    }
}