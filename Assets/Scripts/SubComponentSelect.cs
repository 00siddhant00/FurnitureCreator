using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class SubComponentSelect : MonoBehaviour
{
    private Button subComponentButton;

    private void Start()
    {
        subComponentButton = GetComponent<Button>();
        subComponentButton.onClick.AddListener(() => ItemManager.Instance.BuildItemChange(gameObject));
    }
}
