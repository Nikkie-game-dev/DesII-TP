using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [FormerlySerializedAs("prefabricatedBullet")] [SerializeField]
    private Bullet prefabBullet;

    [SerializeField] private Transform tip;
    [SerializeField] private float power;
    //TODO: add a Model serialized field

    [ContextMenu("Fire")]
    public void Fire()
    {
        var newBullet = Instantiate(prefabBullet, tip.position, tip.rotation);
        newBullet.gameObject.SetActive(true);
        newBullet.SetModel(new Bullet.Model(power));
        newBullet.Fire();
    }
}