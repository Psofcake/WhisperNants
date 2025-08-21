using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemSelector))]
public class ItemSelectorEditor : Editor
{
    private ItemSelector itemSelector;
    private ItemDatabase itemDatabase;
    private SerializedProperty selectedCategoryProp;
    private SerializedProperty selectedTypeProp;
    private SerializedProperty selectedItemProp;

    private void OnEnable()
    {
        itemSelector = (ItemSelector)target;
        selectedCategoryProp = serializedObject.FindProperty("selectedCategory");
        selectedTypeProp = serializedObject.FindProperty("selectedType");
        selectedItemProp = serializedObject.FindProperty("selectedItem");

        if (itemSelector != null && itemSelector.itemDatabase != null)
        {
            // AssetDatabase를 통해 ItemDatabase를 에셋으로 로드
            itemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>("Assets/1. GH_Test/MyAssets/ItemDatabase.asset");

            // 만약 Resources 폴더에 있다면 Resources.Load를 사용할 수 있음
            // itemDatabase = Resources.Load<ItemDatabase>("ItemDatabase");

            if (itemDatabase != null)
            {
                itemSelector.itemDatabase = itemDatabase;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ItemDatabase가 설정되었는지 확인
        if (itemSelector.itemDatabase == null)
        {
            EditorGUILayout.HelpBox("ItemDatabase is not assigned!", UnityEditor.MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
            return;
        }
        
        //EditorGUILayout.PropertyField(selectedCategoryProp);
        
        // 카테고리 선택 (드롭다운)
        List<string> categoryNames = new List<string>();
        foreach (var category in itemSelector.itemDatabase.itemCategories)
        {
            categoryNames.Add(category.categoryName);  // 카테고리 이름을 리스트로 추가
        }
        int selectedCategoryIndex = Mathf.Max(0, itemSelector.itemDatabase.itemCategories.IndexOf(itemSelector.selectedCategory));
        selectedCategoryIndex = EditorGUILayout.Popup("Item Category", selectedCategoryIndex, categoryNames.ToArray());

        if (selectedCategoryIndex >= 0 && selectedCategoryIndex < itemSelector.itemDatabase.itemCategories.Count)
        {
            itemSelector.selectedCategory = itemSelector.itemDatabase.itemCategories[selectedCategoryIndex];  // 선택된 카테고리 할당
        }

        
        
        if (itemSelector.selectedCategory != null)
        {   // ItemType 선택 (두 번째 드롭다운)
            List<string> typeNames = new List<string>();
            foreach (var itemType in itemSelector.selectedCategory.itemTypes)
            {
                typeNames.Add(itemType.typeName);  // 해당 카테고리에 속하는 ItemType 이름 추가
            }

            // 아이템 종류 선택 (드롭다운)
            int selectedTypeIndex = Mathf.Max(0, itemSelector.selectedCategory.itemTypes.IndexOf(itemSelector.selectedType));
            selectedTypeIndex = EditorGUILayout.Popup("Item Type", selectedTypeIndex, typeNames.ToArray());

            if (selectedTypeIndex >= 0 && selectedTypeIndex < itemSelector.selectedCategory.itemTypes.Count)
            {
                itemSelector.selectedType = itemSelector.selectedCategory.itemTypes[selectedTypeIndex];
            }

            // Item 선택 (세 번째 드롭다운)
            if (itemSelector.selectedType != null)
            {
                List<string> itemNames = new List<string>();
                foreach (var item in itemSelector.selectedType.items)
                {
                    itemNames.Add(item.itemName); // 해당 ItemType에 속하는 아이템 이름 추가
                }

                // 아이템 선택 (드롭다운)
                int selectedItemIndex = Mathf.Max(0, itemSelector.selectedType.items.IndexOf(itemSelector.selectedItem));
                selectedItemIndex = EditorGUILayout.Popup("Item", selectedItemIndex, itemNames.ToArray());

                if (selectedItemIndex >= 0 && selectedItemIndex < itemSelector.selectedType.items.Count)
                {
                    itemSelector.selectedItem = itemSelector.selectedType.items[selectedItemIndex];
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
        
        // 선택된 아이템의 정보를 인스펙터에서 출력
        if (itemSelector.selectedItem != null)
        {
            EditorGUILayout.LabelField("ID: ", itemSelector.selectedItem.id.ToString());
            EditorGUILayout.LabelField("Description: ", itemSelector.selectedItem.description);
            if(itemSelector.selectedItem.icon!=null)
                EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(100, 100), itemSelector.selectedItem.icon.texture);
        }
            
    }
}
