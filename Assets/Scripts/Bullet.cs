using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody _rb;
    private Model _model;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Fire();
    }

    private void Fire()
    {
        _rb.AddForce(transform.forward * _model.Force, ForceMode.Impulse);
    }
    
    public struct Model
    { 
        public float Force { get; private set; }
        
    }
    
    public void SetModel(Model model) => _model = model;

    
}
