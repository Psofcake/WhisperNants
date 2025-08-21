using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelector : MonoBehaviour
{
    public ItemDatabase itemDatabase;  // ItemDatabase를 참조 (Inspector에서 연결)
    public ItemCategory selectedCategory;
    public ItemType selectedType;
    public Item selectedItem;
}