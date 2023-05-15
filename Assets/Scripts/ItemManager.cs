using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ReadOnlyAttribute : PropertyAttribute { }

namespace Assets.Scripts
{
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance;

        public string currentItemIndex;

        [Header("Item Datas")]
        [Space(10)]
        public ItemData[] itemDatas;

        [Space(30)]
        [Header("Hierarchy Components")]
        [SerializeField] private GameObject componentData;
        [SerializeField] private GameObject componentSubData;
        [SerializeField] private GameObject componentColorData;
        [SerializeField] private GameObject build;

        [SerializeField] private GameObject buildHolder;
        [SerializeField] private GameObject ItemChoose;

        [Space(12)]
        [Header("Prefabs")]
        [SerializeField] private GameObject componentPrefab;
        [SerializeField] private GameObject displayItemPrefab;

        [Space(15)]
        [Header("Color Theme")]

        [Header("Color Theme - Component")]

        public Color colorComponentDataedBackground;
        public Color colorComponentBackground;
        public Color colorComponentDataed;
        public Color colorComponentDeafultSelected;

        [Header("Color Theme - Sub-Component")]
        public Color colorSubComponentBackground;
        public Color colorSubComponentSelectedBackground;


        // Initialization of Data
        void Awake()
        {
            Instance = this;
            InitializeData();

            for (int i = 0; i < buildHolder.transform.childCount; i++)
            {
                if (i > 5)
                {
                    buildHolder.transform.GetChild(i).GetComponent<HolderSelection>().locked = true;
                }
            }
        }

        //Initialization all the data and creats inital build and default visual datas from ItemData
        public void InitializeData()
        {
            GenerateComponents();
            GenerateSubComponents();
            GenerateItemBuild();
            GetRandomColor();
        }

        public void BuildHolderSave()
        {
            for (int i = 0; i < buildHolder.transform.childCount; i++)
            {
                if (buildHolder.transform.GetChild(i).GetComponent<HolderSelection>().selected && !buildHolder.transform.GetChild(i).GetComponent<HolderSelection>().locked)
                {
                    for (int j = 0; j < buildHolder.transform.GetChild(i).GetChild(0).transform.childCount; j++)
                    {
                        Destroy(buildHolder.transform.GetChild(i).GetChild(0).GetChild(j).gameObject);
                    }
                    var buildIcon = Instantiate(build, buildHolder.transform.GetChild(i).GetChild(0).transform);
                    buildIcon.transform.localPosition = new Vector3(0f, -20f, -1f);
                    buildIcon.transform.localScale = new Vector3(50f, 50f, 50f);

                    buildIcon.name = currentItemIndex.ToString();

                    buildHolder.transform.GetChild(i).GetComponent<HolderSelection>().selected = false;
                }
            }

            foreach (var item in itemDatas)
            {
                item.selected = false;
            }
        }

        public void ChooseItemInitiate()
        {
            StartCoroutine(ChooseItem());
        }

        IEnumerator ChooseItem()
        {
            yield return new WaitForSeconds(0.01f);

            foreach (var item in itemDatas)
            {
                item.selected = false;
            }

            for (int i = 0; i < ItemChoose.transform.childCount; i++)
            {
                //print(ItemChoose.transform.GetChild(i).gameObject.name + " : " + ItemChoose.transform.GetChild(i).GetComponent<HolderSelection>().selected);
                if (ItemChoose.transform.GetChild(i).GetComponent<HolderSelection>().selected)
                {
                    itemDatas[i].selected = true;
                    ItemChoose.transform.GetChild(i).GetComponent<HolderSelection>().selected = false;
                }
            }

            build.transform.localPosition = Vector3.zero;
            build.transform.localScale = new Vector3(108f, 108f, 108f);

            for (int i = 0; i < buildHolder.transform.childCount; i++)
            {
                if (buildHolder.transform.GetChild(i).GetComponent<HolderSelection>().selected && !buildHolder.transform.GetChild(i).GetComponent<HolderSelection>().locked)
                {
                    if (buildHolder.transform.GetChild(i).GetChild(0).GetChild(0).childCount > 0)
                    {
                        LoadItemIndexBuild(buildHolder.transform.GetChild(i).GetChild(0).GetChild(0).name);
                    }
                    else
                    {
                        LoadItemIndexBuild("00010/#FFFFFF");
                    }
                }
            }

            foreach (var item in itemDatas)
            {
                if (item.selected)
                {
                    for (int i = 0; i < item.components.Count; i++)
                    {
                        SetBuildHight(item.components[i], build.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite);
                    }
                }
            }
        }

        //Creats a Index Code for item build to save and load data (this functions is used for saving)
        public void SetItemIndex()
        {
            string itemIndex = string.Empty;

            foreach (var item in itemDatas)
            {
                if (item.selected)
                {
                    itemIndex += item.itemId;

                    for (int i = 0; i < item.components.Count; i++)
                    {
                        for (int j = 0; j < item.components[i].subComponents.Count; j++)
                        {
                            try
                            {
                                if (item.components[i].subComponents[j].name == build.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite.name)
                                {
                                    itemIndex += j;
                                    break;
                                }
                            }
                            catch (NullReferenceException)
                            {
                                itemIndex += 0;
                                break;
                            }
                        }
                    }
                }
            }

            var rgbColor = build.transform.GetChild(0).GetComponent<SpriteRenderer>().color;

            string hexColor = ColorUtility.ToHtmlStringRGB(rgbColor);

            currentItemIndex = itemIndex + "/#" + hexColor;
            PlayerPrefs.SetString("ItemIndex", currentItemIndex);
        }

        public void LoadItemIndexBuild(string itemBuildIndex)
        {
            //string itemBuildIndex = PlayerPrefs.GetString("ItemIndex", "00010/#FFFFFF");
            string[] itemIndex = itemBuildIndex.Split('/');
            int[] buildIndex = itemIndex[0].Select(c => int.Parse(c.ToString())).ToArray();

            foreach (var item in itemDatas)
            {
                if (buildIndex[0] != item.itemId) continue;

                for (int i = 0; i < item.components.Count; i++)
                {
                    build.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = item.components[i].subComponents[buildIndex[i + 1]].name == "none" ? null : item.components[i].subComponents[buildIndex[i + 1]];

                    SetBuildHight(item.components[i], build.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite);

                    Color rgbColor;
                    if (ColorUtility.TryParseHtmlString(itemIndex[1], out rgbColor))
                    {
                        build.transform.GetChild(i).GetComponent<SpriteRenderer>().color = rgbColor;
                    }
                    else
                    {
                        Debug.LogError("Invalid hex color format: " + itemIndex[1]);
                    }
                }
            }

            print(currentItemIndex);
            SetItemIndex();
        }

        public void GenerateDeafultBuild()
        {
            foreach (var item in itemDatas)
            {
                if (item.selected)
                {
                    for (int i = 0; i < item.components.Count; i++)
                    {
                        build.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = item.components[i].subComponents[item.components[i].subComponents[0].name == "none" ? 1 : 0];
                        SetBuildHight(item.components[i], build.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite);
                    }
                }
            }

            SetItemIndex();
        }

        //Generates Random Builds | used in Shuffle button
        public void GetRandomComponents()
        {
            int index = 0;

            foreach (var item in itemDatas)
            {
                if (!item.selected) continue;


                foreach (var comp in item.components)
                {
                    build.transform.GetChild(index).GetComponent<SpriteRenderer>().sprite = GetRandomeSubCompSprite(comp);

                    if (comp.name != "Bottom")
                    {
                        index++;
                        continue;
                    }
                    SetBuildHight(comp, build.transform.GetChild(index).GetComponent<SpriteRenderer>().sprite);
                    index++;
                }
                GetRandomColor();
            }
        }

        //Returns Random Sub Component Sprites by processing desired data
        Sprite GetRandomeSubCompSprite(Component comp)
        {
            var sprite = comp.subComponents[UnityEngine.Random.Range(0, comp.subComponents.Count)];
            if (sprite.name == "none") sprite = null;

            return sprite;
        }

        //Assigns Random Color to Build | Used with shuffle and when first open build
        void GetRandomColor()
        {
            var color = componentColorData.transform.GetChild(UnityEngine.Random.Range(0, componentColorData.transform.childCount)).GetComponent<RawImage>().color;
            for (int i = 0; i < build.transform.childCount; i++)
            {
                build.transform.GetChild(i).GetComponent<SpriteRenderer>().color = color;
            }
            SetItemIndex();
        }

        //Assigns color based on the choosen color | used and call by specific color button
        public void ColorChange(RawImage img)
        {
            for (int i = 0; i < build.transform.childCount; i++)
            {
                build.transform.GetChild(i).GetComponent<SpriteRenderer>().color = img.color;
            }
            SetItemIndex();
        }

        //Changes the hight of the entire build based on different bottom hights to make it align indefinetly
        void SetBuildHight(Component comp, Sprite sprite)
        {
            if (itemDatas[0].name != "Chair") return;
            if (comp.name != "Bottom") return;

            if (sprite.name.Contains("Off"))
            {
                build.transform.position = new Vector3(-5.5f, 0.2f, 0f);
            }
            else if (sprite.name.Contains("Wing"))
            {
                build.transform.position = new Vector3(-5.5f, -0.4f, 0f);
            }
            else if (sprite.name.Contains("Wood"))
            {
                build.transform.position = new Vector3(-5.5f, 0f, 0f);
            }
        }

        //Generates and initializes the item build components and assigns the default sub components based on the components of specified item
        public void GenerateItemBuild()
        {
            if (build.transform.childCount > 0)
            {
                for (int i = 0; i < build.transform.childCount; i++)
                {
                    Destroy(build.transform.GetChild(i).gameObject);
                }
            }

            int index = 0;

            foreach (var item in itemDatas)
            {
                if (!item.selected) continue;

                foreach (var comp in itemDatas[0].components)
                {
                    var buildComponent = Instantiate(displayItemPrefab, build.transform);

                    buildComponent.name = comp.name;

                    if (index == 1)
                    {
                        buildComponent.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    }
                    else if (index == 2)
                    {
                        buildComponent.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    }

                    buildComponent.GetComponent<SpriteRenderer>().sprite = comp.subComponents[comp.subComponents[0].name == "none" ? 1 : 0];

                    index++;

                    SetBuildHight(comp, buildComponent.GetComponent<SpriteRenderer>().sprite);
                }
            }

            SetItemIndex();
        }

        //Call when sub component is changed | Assigns selected sub component of the selected main component
        public void BuildItemChange(GameObject part)
        {
            Sprite sprite = part.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;

            for (int i = 0; i < componentSubData.transform.childCount; i++)
            {
                componentSubData.transform.GetChild(i).GetComponent<RawImage>().color = colorSubComponentBackground;
            }

            foreach (var item in itemDatas)
            {
                if (!item.selected) continue;

                for (int i = 0; i < build.transform.childCount; i++)
                {
                    if (part.name.Contains(item.components[i].name))
                    {
                        build.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = sprite.name == "none" ? null : sprite;
                        part.transform.GetComponent<RawImage>().color = colorSubComponentSelectedBackground;
                        SetBuildHight(item.components[i], sprite);
                    }
                }
            }
            SetItemIndex();
        }

        //Visualize the selection of main components
        public void SelectComponent(string name)
        {
            foreach (var item in itemDatas)
            {
                if (!item.selected) continue;

                var comp = item.components;
                for (int i = 0; i < componentData.transform.childCount; i++)
                {
                    if (comp[i].name == name)
                    {
                        componentData.transform.GetChild(i).GetComponent<RawImage>().color = colorComponentDataedBackground;
                        comp[i].selected = true;
                    }
                    else
                    {
                        componentData.transform.GetChild(i).GetComponent<RawImage>().color = colorComponentBackground;
                        comp[i].selected = false;
                    }
                }
                GenerateSubComponents();
            }
        }

        //Generates sub component of the selected main component and displays in its container also aligns the sub components UI
        public void GenerateSubComponents()
        {
            if (componentSubData.transform.childCount > 0)
            {
                for (int i = 0; i < componentSubData.transform.childCount; i++)
                {
                    Destroy(componentSubData.transform.GetChild(i).gameObject);
                }
            }

            foreach (var item in itemDatas)
            {
                if (!item.selected) continue;

                foreach (var comp in item.components)
                {
                    if (comp.selected)
                    {
                        int index = 0;
                        foreach (var subComp in comp.subComponents)
                        {
                            var subComponent = Instantiate(componentPrefab, componentSubData.transform);

                            Destroy(subComponent.GetComponent<ComponentSelect>());

                            subComponent.name = comp.name + "_" + index;
                            subComponent.GetComponent<RawImage>().color = colorSubComponentBackground;

                            var sc = subComponent.transform.GetChild(0);
                            sc.GetComponent<SpriteRenderer>().sprite = subComp;

                            switch (comp.name)
                            {
                                case "Top":
                                    sc.transform.localPosition -= new Vector3(0, 100, 0);
                                    break;
                                case "Hands":
                                    if (sc.GetComponent<SpriteRenderer>().sprite.name == "none") break;
                                    sc.transform.localPosition -= new Vector3(0, 30, 0);
                                    break;
                                case "Bottom":
                                    sc.transform.localPosition += new Vector3(0, 100, 0);
                                    break;
                            }
                            index++;
                        }
                    }
                }
            }
        }

        //Generates Main Components based on ItemData components and assings it to its container/parent
        public void GenerateComponents()
        {
            bool selectFirst = true;
            foreach (var item in itemDatas)
            {
                if (!item.selected) continue;

                foreach (var comp in item.components)
                {
                    var component = Instantiate(componentPrefab, componentData.transform);
                    Destroy(component.GetComponent<SubComponentSelect>());
                    component.name = comp.name;
                    component.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                    if (selectFirst)
                    {
                        component.GetComponent<RawImage>().color = colorComponentDataedBackground;
                        comp.selected = true;
                    }
                    else
                    {
                        component.GetComponent<RawImage>().color = colorComponentBackground;
                        comp.selected = false;
                    }

                    selectFirst = false;

                    Destroy(component.transform.GetChild(0).gameObject);

                    var icon = Instantiate(item.componentIcon, component.transform);
                    for (int i = 0; i < icon.transform.childCount; i++)
                    {
                        if (comp.name == icon.transform.GetChild(i).name)
                        {
                            icon.transform.GetChild(i).GetComponent<RawImage>().color = colorComponentDataed;
                        }
                        else
                        {
                            icon.transform.GetChild(i).GetComponent<RawImage>().color = colorComponentDeafultSelected;
                        }
                    }
                }
            }
        }
    }
}