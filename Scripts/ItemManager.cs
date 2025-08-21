using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : SceneSingleton<ItemManager>
{
    public List<ItemCategory> itemCategories;

    protected override void Awake()
    {
        base.Awake();
        // 아이템 목록 초기화
        itemCategories = new List<ItemCategory>();

        // '옷' 대분류
        ItemCategory clothing = new ItemCategory { categoryName = "옷" };
        clothing.itemTypes = new List<ItemType>
        {
            new ItemType { typeName = "상의", items = new List<Item>
                {
                    new Item { itemName = "티셔츠", description = "편안한 티셔츠", id = 1 },
                    new Item { itemName = "셔츠", description = "세련된 셔츠", id = 2 },
                    new Item { itemName = "후드티", description = "따뜻한 후드티", id = 3 },
                    new Item { itemName = "자켓", description = "스타일리시한 자켓", id = 4 },
                    new Item { itemName = "가디건", description = "편안한 가디건", id = 5 }
                }
            },
            new ItemType { typeName = "하의", items = new List<Item>
                {
                    new Item { itemName = "청바지", description = "편안한 청바지", id = 6 },
                    new Item { itemName = "반바지", description = "여름에 적합한 반바지", id = 7 },
                    new Item { itemName = "슬랙스", description = "세련된 슬랙스", id = 8 },
                    new Item { itemName = "치마", description = "여성스러운 치마", id = 9 },
                    new Item { itemName = "레깅스", description = "편안한 레깅스", id = 10 }
                }
            },
            new ItemType { typeName = "원피스", items = new List<Item>
                {
                    new Item { itemName = "롱 원피스", description = "우아한 롱 원피스", id = 11 },
                    new Item { itemName = "미니 원피스", description = "귀여운 미니 원피스", id = 12 },
                    new Item { itemName = "슬립 원피스", description = "편안한 슬립 원피스", id = 13 },
                    new Item { itemName = "티셔츠 원피스", description = "캐주얼한 티셔츠 원피스", id = 14 },
                    new Item { itemName = "플로럴 원피스", description = "화사한 플로럴 원피스", id = 15 }
                }
            }
        };

        itemCategories.Add(clothing);

        // '무기' 대분류 (칼, 총, 둔기)
        ItemCategory weapon = new ItemCategory { categoryName = "무기" };
        weapon.itemTypes = new List<ItemType>
        {
            new ItemType { typeName = "칼", items = new List<Item>
                {
                    new Item { itemName = "단검", description = "빠른 단검", id = 16 },
                    new Item { itemName = "장검", description = "강력한 장검", id = 17 },
                    new Item { itemName = "쿠나이", description = "닌자의 무기", id = 18 },
                    new Item { itemName = "사브르", description = "프랑스식 검", id = 19 },
                    new Item { itemName = "대검", description = "무겁고 강한 대검", id = 20 }
                }
            },
            new ItemType { typeName = "총", items = new List<Item>
                {
                    new Item { itemName = "권총", description = "소형 총기", id = 21 },
                    new Item { itemName = "소총", description = "중형 총기", id = 22 },
                    new Item { itemName = "샷건", description = "근거리 공격에 강한 샷건", id = 23 },
                    new Item { itemName = "저격총", description = "장거리 공격에 강한 저격총", id = 24 },
                    new Item { itemName = "기관총", description = "빠르게 발사되는 총기", id = 25 }
                }
            },
            new ItemType { typeName = "둔기", items = new List<Item>
                {
                    new Item { itemName = "망치", description = "강력한 망치", id = 26 },
                    new Item { itemName = "철퇴", description = "무겁고 강력한 철퇴", id = 27 },
                    new Item { itemName = "철봉", description = "길고 강한 철봉", id = 28 },
                    new Item { itemName = "곤봉", description = "속도가 빠른 곤봉", id = 29 },
                    new Item { itemName = "채찍", description = "날카로운 채찍", id = 30 }
                }
            }
        };

        itemCategories.Add(weapon);

        // '음식' 대분류 (과일, 빵, 음료)
        ItemCategory food = new ItemCategory { categoryName = "음식" };
        food.itemTypes = new List<ItemType>
        {
            new ItemType { typeName = "과일", items = new List<Item>
                {
                    new Item { itemName = "사과", description = "달콤한 사과", id = 31 },
                    new Item { itemName = "오렌지", description = "상큼한 오렌지", id = 32 },
                    new Item { itemName = "바나나", description = "영양가 높은 바나나", id = 33 },
                    new Item { itemName = "딸기", description = "달콤한 딸기", id = 34 },
                    new Item { itemName = "포도", description = "상큼한 포도", id = 35 }
                }
            },
            new ItemType { typeName = "빵", items = new List<Item>
                {
                    new Item { itemName = "식빵", description = "부드러운 식빵", id = 36 },
                    new Item { itemName = "크로와상", description = "바삭한 크로와상", id = 37 },
                    new Item { itemName = "프랑스빵", description = "전통적인 프랑스빵", id = 38 },
                    new Item { itemName = "치아바타", description = "이탈리아식 빵", id = 39 },
                    new Item { itemName = "소보로빵", description = "달콤한 소보로빵", id = 40 }
                }
            },
            new ItemType { typeName = "음료", items = new List<Item>
                {
                    new Item { itemName = "콜라", description = "탄산음료", id = 41 },
                    new Item { itemName = "사이다", description = "상큼한 사이다", id = 42 },
                    new Item { itemName = "주스", description = "상큼한 과일주스", id = 43 },
                    new Item { itemName = "커피", description = "진한 커피", id = 44 },
                    new Item { itemName = "녹차", description = "시원한 녹차", id = 45 }
                }
            }
        };

        itemCategories.Add(food);
    }
}