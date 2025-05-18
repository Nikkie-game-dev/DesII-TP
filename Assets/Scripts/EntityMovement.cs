using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour
{ 
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected float acceleration;
    [SerializeField] protected bool onGround;
    protected Vector2 HorVelocity;
    protected float SpeedLimit;

    private void OnCollisionEnter(Collision other)
    {
        if(other.collider.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.collider.CompareTag("Ground"))
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
    

}
