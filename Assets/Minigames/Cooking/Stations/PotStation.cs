using System.Collections.Generic;
using Cards.Environments;
using Characters;
using Minigames.Cooking.CookingStuff;
using Minigames.Cooking.PotionIngrediets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames.Cooking.Stations
{
    public class PotStation : Station
    {
        [Header("Recipe Settings")]
        [Tooltip("List of ingredients for this recipe")]
        public List<Ingredient> recipeIngredients = new List<Ingredient>();

        [Header("UI")]
        public Image currentIngredientImage;
        public Image nextIngredientImage;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI inspectPromptText;

        [Header("Ingredient Tossing")]
        public List<Transform> ingredientSpawnPoints = new List<Transform>();
        public List<GameObject> tossableIngredients = new List<GameObject>();

        [Tooltip("Seconds between ingredient tosses")]
        public float tossInterval = 3f;

        [Tooltip("Force applied to tossed ingredients")]
        public float tossForce = 6f;

        private int currentIngredientIndex = 0;
        private bool potStarted = false;
        private float ingredientTimer = 0f;
        private bool timerActive = false;
        private bool isInspecting = false;

        private float tossTimer = 0f;

        // Store original rect transform values
        private Vector2 originalAnchorMin;
        private Vector2 originalAnchorMax;
        private Vector2 originalOffsetMin;
        private Vector2 originalOffsetMax;
        private int originalSiblingIndex;

        protected override bool CheckCriteria(Cookable heldObject)
        {
            return potStarted && currentIngredientIndex < recipeIngredients.Count && heldObject != null;
        }

        /* =========================
     * POT LIFECYCLE
     * ========================= */

        public void StartPot()
        {
            potStarted = true;
            currentIngredientIndex = 0;
            tossTimer = tossInterval;
            StartIngredientTimer();
            UpdateUI();
        }

        private void StartIngredientTimer()
        {
            timerActive = false;
            ingredientTimer = 0f;

            if (currentIngredientIndex >= recipeIngredients.Count)
                return;

            float duration = recipeIngredients[currentIngredientIndex].duration;
            if (duration > 0f)
            {
                ingredientTimer = duration;
                timerActive = true;
            }

            UpdateTimerUI();
        }

        /* =========================
     * UPDATE LOOP
     * ========================= */

        private void Update()
        {
            // Inspect input
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInspect();
            }

            // Toss ingredients while pot is active
            if (potStarted)
            {
                tossTimer -= Time.deltaTime;
                if (tossTimer <= 0f)
                {
                    TossRandomIngredient();
                    tossTimer = tossInterval;
                }
            }

            if (!potStarted || !timerActive)
                return;

            ingredientTimer -= Time.deltaTime;
            UpdateTimerUI();

            if (ingredientTimer <= 0f)
            {
                timerActive = false;
                ExplodeManager.Instance.Explode(stationPoint);
            }
        }

        /* =========================
     * INTERACTION
     * ========================= */

        public override void Interact(GameObject player)
        {
            if (!potStarted)
                return;

            Cook cook = OpenWorldEnv.Current.GetComponent<Cook>();
            if (cook == null || cook.heldObject == null)
                return;

            if (currentIngredientIndex >= recipeIngredients.Count)
                return;

            Ingredient currentIngredient = recipeIngredients[currentIngredientIndex];

            if (currentIngredient.CheckValid(cook.heldObject.gameObject))
            {
                currentIngredientIndex++;
                StartIngredientTimer();
            }
            else
            {
                ExplodeManager.Instance.Explode(stationPoint);
            }

            Destroy(cook.heldObject.gameObject);
            cook.heldObject = null;

            UpdateUI();
        }

        /* =========================
     * INSPECT FUNCTIONALITY
     * ========================= */

        private void ToggleInspect()
        {
            if (!potStarted || currentIngredientIndex >= recipeIngredients.Count)
                return;

            isInspecting = !isInspecting;

            if (isInspecting)
                ShowInspect();
            else
                HideInspect();
        }

        private void ShowInspect()
        {
            if (currentIngredientImage == null)
                return;

            currentIngredientImage.transform.SetAsLastSibling();
            currentIngredientImage.rectTransform.anchorMin = Vector2.zero;
            currentIngredientImage.rectTransform.anchorMax = Vector2.one;
            currentIngredientImage.rectTransform.offsetMin = Vector2.zero;
            currentIngredientImage.rectTransform.offsetMax = Vector2.zero;
        }

        private void HideInspect()
        {
            if (currentIngredientImage == null)
                return;

            currentIngredientImage.rectTransform.anchorMin = originalAnchorMin;
            currentIngredientImage.rectTransform.anchorMax = originalAnchorMax;
            currentIngredientImage.rectTransform.offsetMin = originalOffsetMin;
            currentIngredientImage.rectTransform.offsetMax = originalOffsetMax;
            currentIngredientImage.transform.SetSiblingIndex(originalSiblingIndex);
        }

        /* =========================
     * UI
     * ========================= */

        private void UpdateUI()
        {
            if (!potStarted)
            {
                HideAllUI();
                return;
            }

            // Current ingredient
            if (currentIngredientIndex < recipeIngredients.Count)
            {
                currentIngredientImage.sprite = recipeIngredients[currentIngredientIndex].ingredientImage;
                currentIngredientImage.gameObject.SetActive(true);

                if (inspectPromptText != null)
                    inspectPromptText.gameObject.SetActive(true);
            }
            else
            {
                currentIngredientImage.gameObject.SetActive(false);

                if (inspectPromptText != null)
                    inspectPromptText.gameObject.SetActive(false);
            }

            // Next ingredient
            if (currentIngredientIndex + 1 < recipeIngredients.Count)
            {
                nextIngredientImage.sprite = recipeIngredients[currentIngredientIndex + 1].ingredientImage;
                nextIngredientImage.gameObject.SetActive(true);
            }
            else
            {
                nextIngredientImage.gameObject.SetActive(false);
            }

            UpdateTimerUI();
        }

        private void UpdateTimerUI()
        {
            if (timerText == null)
                return;

            if (!timerActive)
            {
                timerText.gameObject.SetActive(false);
                return;
            }

            timerText.gameObject.SetActive(true);
            timerText.text = Mathf.CeilToInt(ingredientTimer).ToString();
        }

        private void HideAllUI()
        {
            if (currentIngredientImage != null)
                currentIngredientImage.gameObject.SetActive(false);
            if (nextIngredientImage != null)
                nextIngredientImage.gameObject.SetActive(false);
            if (timerText != null)
                timerText.gameObject.SetActive(false);
            if (inspectPromptText != null)
                inspectPromptText.gameObject.SetActive(false);
        }

        /* =========================
     * INGREDIENT TOSSING
     * ========================= */

        private void TossRandomIngredient()
        {
            if (ingredientSpawnPoints.Count == 0 || tossableIngredients.Count == 0)
                return;

            Transform spawnPoint = ingredientSpawnPoints[Random.Range(0, ingredientSpawnPoints.Count)];
            GameObject prefab = tossableIngredients[Random.Range(0, tossableIngredients.Count)];

            GameObject ingredient = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

            Rigidbody rb = ingredient.GetComponent<Rigidbody>();
            if (rb == null)
                return;

            // Toss forward relative to the spawn point
            Vector3 direction = spawnPoint.forward;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(direction * tossForce, ForceMode.Impulse);
        }


        private void Start()
        {
            HideAllUI();

            if (currentIngredientImage != null)
            {
                originalAnchorMin = currentIngredientImage.rectTransform.anchorMin;
                originalAnchorMax = currentIngredientImage.rectTransform.anchorMax;
                originalOffsetMin = currentIngredientImage.rectTransform.offsetMin;
                originalOffsetMax = currentIngredientImage.rectTransform.offsetMax;
                originalSiblingIndex = currentIngredientImage.transform.GetSiblingIndex();
            }
        }
    }
}
