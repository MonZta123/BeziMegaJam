using UnityEngine;

namespace BossBehaviours.PattyBoss
{
    public class SwitchScript : MonoBehaviour
    {
        [SerializeField]
        private Material redMaterial;
        
        [SerializeField]
        private Material greenMaterial;
        
        [SerializeField]
        private MeshRenderer meshRenderer;

        public bool IsOn { get; private set; }

        public void SetRandomColor()
        {
            IsOn = Random.Range(0, 2) == 0;
            
            SetColor();
        }

        private void Start()
        {
            SwitchManager.Instance.RegisterSwitch(this);
        }
        
        private void SetColor()
        {
            if (IsOn)
            {
                meshRenderer.material = greenMaterial;
            }
            else
            {
                meshRenderer.material = redMaterial;
            }
        }

        private bool _locked;
        
        public void OnTriggerEnter(Collider other)
        {
            if (_locked)
                return;
            
            IsOn = !IsOn;
            SetColor();
        }

        public void Lock(bool value)
        {
            _locked = value;
        }
    }
}
