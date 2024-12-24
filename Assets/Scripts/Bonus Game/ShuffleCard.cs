using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShuffleCard : MonoBehaviour
{
    public GameObject[] cards; // Assign your buttons (cards) in the Unity editor
    public List<Image> _marine_images_list;
    public Image _random_marine;
    public int _random_marine_index;

    public float shuffleDuration = 1f; // Duration of the shuffle animation

    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private GameObject betterLuckPanel;
    [SerializeField] private GameObject _goHomePanel;

    [SerializeField] private AudioSource _winningSoundFX;
    [SerializeField] private AudioSource _looseSoundFX;
    [SerializeField] private AudioSource _shufflingFX;

    [SerializeField] private RectTransform canvasRectTransform; // You can assign this manually in the Inspector

    private bool isShuffling = false;

    private void Awake()
    {
        winPanel.SetActive(false);
        betterLuckPanel.SetActive(false);
    }

    private void Start()
    {
        Assign_Random_Marine();
        ShuffleCards();
    }

    public void OnCardClick(int cardIndex)
    {
        if (!isShuffling) // Ensure the game is not shuffling before checking the result
        {
            if (cardIndex == _random_marine_index) // Assuming the name of the joker button is "Joker"
            {
                Debug.Log("You win!");
                float balance = PlayerPrefs.GetFloat("Balance");

                balance += PlayerStats.Instance.BetAmount * 2;

                PlayerPrefs.SetFloat("Balance", balance);

                StartCoroutine(ShowWin());

                pointsText.text = (PlayerStats.Instance.BetAmount * 2) + "";
            }
            else
            {
                Debug.Log("You lose!");

                StartCoroutine(ShowBetterLuck());
            }

            // Activate child objects of all cards
            foreach (GameObject card in cards)
            {
                ActivateChildObjects(card);
            }
        }
    }

    IEnumerator ShowBetterLuck()
    {
        yield return new WaitForSeconds(3);
        betterLuckPanel.SetActive(true);
        _goHomePanel.SetActive(true);
        _looseSoundFX.Play();
    }

    IEnumerator ShowWin()
    {
        yield return new WaitForSeconds(3f);

        winPanel.SetActive(true);
        _winningSoundFX.Play();

        yield return new WaitForSeconds(1);
        _goHomePanel.SetActive(true);
    }

    private void ActivateChildObjects(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void DeactivateChildObjects(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShuffleAnimationCoroutine()
    {
        _shufflingFX.Play();

        // Store initial positions
        Vector3[] initialPositions = new Vector3[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            initialPositions[i] = cards[i].transform.position;
        }

        // Shuffle the card positions randomly
        for (int i = 0; i < cards.Length; i++)
        {
            int randomIndex = Random.Range(0, cards.Length);
            Vector3 tempPosition = cards[i].transform.position;
            cards[i].transform.position = cards[randomIndex].transform.position;
            cards[randomIndex].transform.position = tempPosition;
        }

        // Wait for the rotation to complete before opening the cards
        yield return new WaitForSeconds(0.5f);  // Wait for the rotation animation

        // Open the cards by activating child objects
        foreach (GameObject card in cards)
        {
            ActivateChildObjects(card);
        }

        // Wait for another second before closing the cards
        yield return new WaitForSeconds(2f);

        // Close the cards by deactivating child objects
        foreach (GameObject card in cards)
        {
            DeactivateChildObjects(card);
        }

        // Wait for 1 second before moving cards to the center position
        yield return new WaitForSeconds(1f);

        // Ensure that the canvasRectTransform is assigned
        if (canvasRectTransform == null)
        {
            Debug.LogError("Canvas RectTransform is not assigned! Please assign it in the inspector.");
            yield break;  // Exit the coroutine to prevent further errors
        }

        // Calculate the center of the canvas (use the RectTransform of the Canvas)
        Vector3 canvasCenter = canvasRectTransform.position;  // The center of the canvas is its position

        // Move cards to the center of the canvas using LeanTween for smooth animation
        foreach (GameObject card in cards)
        {
            LeanTween.move(card, canvasCenter, 1f); // Move each card to the canvas center in 1 second
        }

        // Wait for the animation to complete
        yield return new WaitForSeconds(2f);

        // Now move the cards back to their original positions
        for (int i = 0; i < cards.Length; i++)
        {
            LeanTween.move(cards[i], initialPositions[i], 1f);  // Move each card back to its original position in 1 second
        }

        // Optionally, after moving them to their original positions, reset the isShuffling flag
        yield return new WaitForSeconds(1f); // Wait for the animation to complete
        isShuffling = false;
        _shufflingFX.Stop();
    }

    private void ShuffleCards()
    {
        if (!isShuffling)
        {
            isShuffling = true;
            StartCoroutine(ShuffleAnimationCoroutine());
        }
    }

    public void Assign_Random_Marine()
    {
        print(_marine_images_list.Count);
        _random_marine_index = Random.Range(0, _marine_images_list.Count - 1);
        print(_random_marine_index);
        _random_marine.sprite = _marine_images_list[_random_marine_index].sprite;
    }
}
