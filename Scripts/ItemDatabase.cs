using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 대분류 클래스
[System.Serializable]
public class ItemCategory
{
    public string categoryName; // 대분류 이름 (장비, 무기, 회복템, 퀘스트템 등)
    public List<ItemType> itemTypes; // 해당 대분류에 속하는 종류들
}
// 아이템 종류 클래스
[System.Serializable]
public class ItemType
{
    public string typeName;  // 아이템 종류 (예:장비-방어구/조명, 무기:근접/원거리/투척, 회복:음식,주사기 등)
    public List<Item> items; // 해당 종류에 속하는 아이템들
}
// 아이템 클래스
[System.Serializable]
public class Item
{
    public string itemName;   // 아이템 이름
    public string description; // 아이템 설명
    public Sprite icon;        // 아이템 아이콘 (UI에서 사용)
    public int id;             // 아이템 ID
}

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item/ItemDatabase", order = 1)]
public class ItemDatabase : ScriptableObject
{
    // 아이템 목록 초기화
    public List<ItemCategory> itemCategories;

    void Awake()
    {
        // ~~~~데이터 예시~~~~
        
        // 아이템 목록 초기화
        itemCategories = new List<ItemCategory> 
        {
            new ItemCategory {
                categoryName = "Weapon",    //카테고리 이름
                itemTypes = new List<ItemType> {
                    new ItemType {
                        typeName = "Melee", //아이템분류 이름
                        items = new List<Item> {
                            //아이템 이름,설명,고유번호
                            new Item { itemName = "Sword", description = "A sharp sword", id = 1 },
                            new Item { itemName = "Axe", description = "A heavy axe", id = 2 }
                        }
                    },
                    new ItemType {
                        typeName = "Ranged", items = new List<Item> 
                        {
                            new Item { itemName = "Bow", description = "A longbow", id = 3 },
                            new Item { itemName = "Crossbow", description = "A crossbow", id = 4 }
                        }
                    }
                }
            },
            new ItemCategory {
                categoryName = "Healing",
                itemTypes = new List<ItemType> {
                    new ItemType {
                        typeName = "Food", items = new List<Item> 
                        {
                            new Item { itemName = "Apple", description = "A juicy apple", id = 5 },
                            new Item { itemName = "Bread", description = "A loaf of bread", id = 6 }
                        }
                    }
                }
            }
        };
    }
}
