using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class ItemDataUpdate : MonoBehaviour
    {

        int itemId;
        public ItemManager itemM;

        // Update is called once per frame
        void Update()
        {
            itemId = 0;
            foreach (var item in itemM.itemDatas)
            {
                // Update the tutorial ID
                item.itemId = itemId;

                itemId++;
            }
        }
    }
}