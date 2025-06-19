using UnityEngine;
namespace Entities.Player
{
    public class IkController : MonoBehaviour
    {
        
        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHandAim;
        [SerializeField] private Transform leftHandAim;
        [SerializeField] private Animator animator;

        private float _ikWeight;
        private Transform _rightHand;
        private Transform _leftHand;

        private void OnEnable()
        {
            Interaction.OnGunGrab += SetIk;
            Interaction.OnGunDrop += UnsetIk;
        }

        private void SetIk(Interaction.GunPosition position)
        {
            _ikWeight = 1f;
            
            switch (position)
            {
                case Interaction.GunPosition.Hip:
                    _rightHand = rightHand;
                    _leftHand = leftHand;
                    break;
                
                case Interaction.GunPosition.Shoulder:
                    _rightHand = rightHandAim;
                    _leftHand = leftHandAim;
                    break;
                
                default:
                    Debug.LogError("No Gun position!");
                    break;
            }
        }

        private void UnsetIk()
        {
            _ikWeight = 0f;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _ikWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHand.position);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHand.position);
        }
    }
}