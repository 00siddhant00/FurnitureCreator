using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HolderSelection : MonoBehaviour
    {
        public bool selected = false;
        public bool locked = false;
        public bool ItemHolder = false;
        // Use this for initialization

        private Button selectButton;

        private void Start()
        {
            selectButton = GetComponent<Button>();
            selectButton.onClick.AddListener(() => selected = true);
        }
    }
}