using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;
using static CraftMachine;
using static PlasticGui.WorkspaceWindow.CodeReview.ReviewChanges.Summary.CommentSummaryData;

public class CraftMachine : MonoBehaviour, IInteractable
{
    [SerializeField] Transceiver transceiver;


    [System.Serializable]
    public struct Recipe
    {
        public GameObject prefab; // 만들 아이템 프리팹
        public SerializedDictionary<Item, int> ingredients; // 필요한 재료와 수량
    }

    [SerializeField] List<Recipe> recipes; // 레시피 목록
    [SerializeField] Transform itemSpawnPosition; // 빈오브젝트로 원하는 위치 지정 후 넣어주기

    // 인스펙터에서 설정
    [SerializeField] Recipe weaponRecipe; // 무기 레시피 
    [SerializeField] Recipe trapRecipe; // 덫 레시피

    // 전송기에 오브젝트가 들어있다면 제작하기 UI가 뜨게하고 아니면 아무것도 뜨지 않도록
    public string TooltipText => transceiver.objectInTransceiver.Count > 0 ? "제작하기 [E]" : "";

    private bool canUseCraftMachine;

    private void Start()
    {
        // 리스트에 넣어주고
        recipes.Add(weaponRecipe);
        recipes.Add(trapRecipe);

        // 레시피를 수량 기준으로 내림차순 정렬 (수량이 같을 경우는 리스트에 넣은 순서대로)
        recipes.Sort((recipeA, recipeB) =>
        {
            int totalA = 0, totalB = 0;

            foreach (var ingredient in recipeA.ingredients)
            {
                totalA += ingredient.Value;
            }

            foreach (var ingredient in recipeB.ingredients)
            {
                totalB += ingredient.Value;
            }

            return totalB.CompareTo(totalA); // 수량이 많은 레시피가 먼저 오도록 정렬
        });
    }

    public void Interact()
    {
        foreach (var recipe in recipes) // 정렬된 레시피 돌면서
        {
            if (CanCraft(recipe)) // 크래프팅 가능한지 여부 파악
            {
                // 필요한 재료 차감
                foreach(var ingredient in recipe.ingredients)
                {
                    string ingredientName=ingredient.Key.itemData.itemName;
                    int quantityToRemove = ingredient.Value;

                    // 리스트에서 해당 아이템 찾아서 제거
                    for (int i = 0; i < quantityToRemove; i++)
                    {
                        var itemToRemove = transceiver.objectInTransceiver.Find(obj => obj.GetComponent<Item>().itemData.itemName == ingredientName);
                        if (itemToRemove != null)
                        {
                            transceiver.objectInTransceiver.Remove(itemToRemove); //리스트에서 remove는
                            Destroy(itemToRemove); // 오브젝트 삭제
                        }
                    }
                }
                StartCoroutine(CraftingEffect());
                Instantiate(recipe.prefab, itemSpawnPosition.position, Quaternion.identity);
                
                return; // 첫 번째로 제작 가능한 물건만 제작
            }
        }
    }

    private IEnumerator CraftingEffect()
    {
        transceiver.TurnOnEmission(); // Emission 켜기

        yield return new WaitForSeconds(2f); // 2초 대기

        transceiver.TurnOffEmission(); // Emission 끄기
    }

    private bool CanCraft(Recipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            string ingredientName = ingredient.Key.itemData.itemName; // 아이템의 이름을 가져옴

            // 해당 아이템이 리스트에 있는지 확인하고 수량 계산
            int ingredientCount = transceiver.objectInTransceiver.Count(obj => obj.GetComponent<Item>().itemData.itemName == ingredientName); // 이름으로 수량 확인

            // 수량 확인
            if (ingredientCount < ingredient.Value)
            {
                Debug.Log($"필요한 아이템 부족: {ingredientName}, 필요 수량: {ingredient.Value}, 보유 수량: {ingredientCount}");
                return false; // 필요한 아이템이 부족한 경우
            }
        }
        return true; // 모든 재료가 충분한 경우
    }
}
