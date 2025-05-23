using System.Collections;
using Services;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected float acceleration;
    [SerializeField] protected bool onGround;
    [SerializeField] private float slideRate;
    [SerializeField] private float jumpSlide;
    protected Vector2 HorVelocity;
    protected float SpeedLimit;
    protected Service Service;

    private bool _shouldStop;

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            onGround = true;
            if (_shouldStop)
            {
                StartCoroutine(StopAfterJump());
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    protected void Move(Vector3 move)
    {
        if (!onGround) return;

        rb.AddForce(new Vector3(move.x, 0f, move.z) * acceleration, ForceMode.Force);

        if (HorVelocity.magnitude >= SpeedLimit)
        {
            rb.AddForce(new Vector3(move.x, 0f, move.z) * -acceleration, ForceMode.Force);
        }
    }

    protected IEnumerator Stop()
    {
        yield return new WaitForFixedUpdate();

        if (onGround)
        {
            rb.AddForce(
                new Vector3(HorVelocity.normalized.x, 0f, HorVelocity.normalized.y) *
                -(HorVelocity.magnitude - slideRate), ForceMode.Impulse);
        }
        else
        {
            _shouldStop = true;
        }
    }

    private IEnumerator StopAfterJump()
    {
        yield return new WaitForFixedUpdate();

        rb.AddForce(
            new Vector3(HorVelocity.normalized.x, 0f, HorVelocity.normalized.y) *
            -(HorVelocity.magnitude - jumpSlide), ForceMode.Impulse);
    }
}