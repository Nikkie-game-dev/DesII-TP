using System.Collections;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        [HideInInspector] public float force;

        public float damage;

        [SerializeField] private float selfDestroyTime;
        [SerializeField] private float destroyTimeAfterCollision;
        [SerializeField] private float minSpeedForSparks;
        [SerializeField] private float bodySlowDown;
        [SerializeField] private GameObject sparks;
        private Rigidbody _rb;
        private Coroutine _selfDestroy;

        private void OnEnable()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Fire()
        {
            StartCoroutine(Discharge());
            _selfDestroy = StartCoroutine(TimerDestroy(selfDestroyTime));
        }


        private IEnumerator Discharge()
        {
            yield return new WaitForFixedUpdate();
            _rb.AddForce(transform.forward * force, ForceMode.Impulse);
        }

        private IEnumerator TimerDestroy(float time)
        {
            yield return new WaitForSeconds(time);

           Debug.Log("Self destroy");
           Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_selfDestroy != null)
            {
                StopCoroutine(_selfDestroy);
            }

            if ((other.collider.CompareTag("Wall") || other.collider.CompareTag("Ground")) &&
                _rb.linearVelocity.magnitude > minSpeedForSparks)
            {
                sparks.SetActive(true);
                sparks.transform.position = transform.position;
                sparks.transform.Rotate(other.GetContact(0).normal);
                Debug.DrawRay(sparks.transform.position, other.GetContact(0).normal * 20, Color.red);
                sparks.GetComponent<ParticleSystem>().Play();
            }

            StartCoroutine(TimerDestroy(destroyTimeAfterCollision));
        }

        private void OnTriggerEnter(Collider other)
        {
            _rb.AddForce(-transform.forward * bodySlowDown, ForceMode.Impulse);
            other.gameObject.GetComponentInParent<Enemy.Stats>().ReceiveDamage(damage);
        }
    }
}