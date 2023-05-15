using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public class ItemData
    {
        public string name;

        [ReadOnly]
        public int itemId;

        public bool selected = false;

        public GameObject componentIcon;
        public List<Component> components;
    }

    [System.Serializable]
    public class Component
    {
        public string name;
        public bool selected;
        public List<Sprite> subComponents;
    }
}
