using UnityEngine;
using UnityEngine.Events;

namespace Entities.Player
{
    public class IkController : MonoBehaviour
    {
        public static UnityEvent OnGunGrab;
        public static UnityEvent OnGunDrop;
        
        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Animator animator;

        private float _ikWeight;
        private void OnEnable()
        {
            OnGunGrab = new UnityEvent();
            OnGunGrab.AddListener(SetIk);
            OnGunDrop = new UnityEvent();
            OnGunDrop.AddListener(UnsetIk);
        }

        private void SetIk() => _ikWeight = 1f;
        private void UnsetIk() => _ikWeight = 0f;

        private void OnAnimatorIK(int layerIndex)
        {
            Debug.Log(_ikWeight);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _ikWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
        }
    }
}
