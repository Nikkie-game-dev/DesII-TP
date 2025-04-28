using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody _rb;
    private Model _model;
    [SerializeField] private float time;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Fire()
    {
        StartCoroutine(Discharge());
        StartCoroutine(SelfDestroy());
    }
    
    
    private IEnumerator Discharge()
    {
        yield return new WaitForFixedUpdate();
        _rb.AddForce(transform.forward * _model.Force, ForceMode.Impulse);
    }

    private IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(time);
        
        #if DEBUG
        print("Self destroy");
        #endif
        
        Destroy(gameObject);
    }
    
    public struct Model
    { 
        public float Force { get; }

        public Model(float force)
        {
            Force = force;
        }
        
    }
    
    public void SetModel(Model model) => _model = model;

    
}
