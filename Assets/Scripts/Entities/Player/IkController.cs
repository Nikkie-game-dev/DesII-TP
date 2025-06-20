using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Player
{
    public class IkController : MonoBehaviour
    {
        [FormerlySerializedAs("rightHand")] [SerializeField]
        private Transform rightHandHip;

        [FormerlySerializedAs("leftHand")] [SerializeField]
        private Transform leftHandHip;

        [SerializeField] private Transform rightHandAim;
        [SerializeField] private Transform leftHandAim;

        [CanBeNull] private Animator _animator;
        private float _ikWeight;
        private Transform _rightHand;
        private Transform _leftHand;

        private void OnEnable()
        {
            _animator = GetComponent<Animator>();
            
            Interaction.OnGunGrab += SetIkHip;
            Interaction.OnGunDrop += UnsetIkHip;
            Sight.OnScopeIn += SetScopeIk;
            Sight.OnScopeOut += UnsetScopeIk;
            
            _rightHand = rightHandHip;
            _leftHand = leftHandHip;
            
        }

        private void SetIkHip()
        {
            _ikWeight = 1f;
            _rightHand = rightHandHip;
            _leftHand = leftHandHip;
        }

        private void SetScopeIk()
        {
            _rightHand = rightHandAim;
            _leftHand = leftHandAim;
        }


        private void UnsetIkHip()
        {
            _ikWeight = 0f;
        }

        private void UnsetScopeIk()
        {
            _rightHand = rightHandHip;
            _leftHand = rightHandHip;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if(!_animator ) return;
            
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _ikWeight);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHand.position);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikWeight);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHand.position);
        }
    }
}