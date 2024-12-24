using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LeanTween;

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

                print(balance);
                print(PlayerStats.Instance);
                print(PlayerStats.Instance.BetAmount);

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

    private IEnumerator ShuffleAnimationCoroutine()
    {
        _shufflingFX.Play();

        Vector3[] initialPositions = new Vector3[cards.Length];
        Vector3[] spreadPositions = new Vector3[cards.Length];  // For opening the cards

        // Store initial positions
        for (int i = 0; i < cards.Length; i++)
        {
            initialPositions[i] = cards[i].transform.position;
        }

        // Spread out cards for the "open" effect
        for (int i = 0; i < cards.Length; i++)
        {
            spreadPositions[i] = initialPositions[i] + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
            LeanTween.move(cards[i], spreadPositions[i], shuffleDuration / 2f).setEase(LeanTweenType.easeInOutQuad);
        }

        yield return new WaitForSeconds(shuffleDuration / 2f);

        // Close the cards by bringing them back together
        for (int i = 0; i < cards.Length; i++)
        {
            LeanTween.move(cards[i], initialPositions[i], shuffleDuration / 2f).setEase(LeanTweenType.easeInOutQuad);
        }

        yield return new WaitForSeconds(shuffleDuration / 2f);

        // Move the cards together to form the deck
        Vector3 deckPosition = new Vector3(0f, 0f, 0f); // Center position to form the deck
        for (int i = 0; i < cards.Length; i++)
        {
            LeanTween.move(cards[i], deckPosition, shuffleDuration / 2f).setEase(LeanTweenType.easeInOutQuad);
        }

        yield return new WaitForSeconds(shuffleDuration / 2f);

        // Shuffle the cards like a deck
        for (int i = 0; i < cards.Length; i++)
        {
            int randomIndex = Random.Range(i, cards.Length);
            Vector3 tempPosition = cards[i].transform.position;
            LeanTween.move(cards[i], cards[randomIndex].transform.position, shuffleDuration / 2f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.move(cards[randomIndex], tempPosition, shuffleDuration / 2f).setEase(LeanTweenType.easeInOutQuad);
        }

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
