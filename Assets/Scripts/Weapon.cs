using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [FormerlySerializedAs("prefabricatedBullet")] [SerializeField]
    private Bullet prefabBullet;

    [SerializeField] private Transform tip;
    
    private Bullet.Model _model;
    
    [ContextMenu("Fire")]
    public void Fire()
    {
        var newBullet = Instantiate(prefabBullet, tip.position, tip.rotation);
        newBullet.SetModel(_model);
    }
}