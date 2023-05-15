using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class ComponentSelect : MonoBehaviour
{
    private Button componentButton;

    private void Start()
    {
        componentButton = GetComponent<Button>();
        componentButton.onClick.AddListener(() => ItemManager.Instance.SelectComponent(gameObject.name));
    }
}
