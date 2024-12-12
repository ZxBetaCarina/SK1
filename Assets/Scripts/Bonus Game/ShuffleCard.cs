using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DentedPixel;
using Random = UnityEngine.Random;

public class ShuffleCard : MonoBehaviour
{
    public GameObject[] cards;
    public List<Sprite> cardsSprites;// Assign your buttons (cards) in the Unity editor
    private List<CardAnimator> _cardAnimators=new List<CardAnimator>();
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
   private bool isShowcardsCompleted = true;
    private void Awake()
    {
        winPanel.SetActive(false);
        betterLuckPanel.SetActive(false);
    }

    private void Start()
    {
        GetAllCardScripts();
        shufflecardsprits();
        Assign_Random_Marine();
        AnimateCardInStarting(5);
        StartCoroutine(CallShowcardsWithDelay(5, 5f));
        //ShuffleCards();
        //ShuffleCardsNew();
      
    }

    private void shufflecardsprits()
    {
     
            cardsSprites = new List<Sprite>(new Sprite[_cardAnimators.Count]);
        
        for (int i = 0; i < _cardAnimators.Count; i++)
        {
            cardsSprites[i] = _cardAnimators[i].childcard.GetComponent<Image>().sprite;
        }
        for (int i = 0; i < cardsSprites.Count; i++)
        {
            int randomIndex = Random.Range(i, cardsSprites.Count);
            Sprite tempPosition = cardsSprites[i];
            cardsSprites[i]= cardsSprites[randomIndex];
            cardsSprites[randomIndex] = tempPosition;
        }

        for (int i = 0; i < _cardAnimators.Count; i++)
        {
            _cardAnimators[i].childcard.GetComponent<Image>().sprite = cardsSprites[i];
            
        }
    }
    IEnumerator CallShowcardsWithDelay(int parameter, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        yield return StartCoroutine(Showcards(parameter)); 
        
        ShuffleCardsNew(); 
    }
   void  GetAllCardScripts()
    {
        foreach (GameObject card in cards)
        {
           _cardAnimators.Add(card.GetComponent<CardAnimator>());
        }
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

            // Store initial positions
            for (int i = 0; i < cards.Length; i++)
            {
                initialPositions[i] = cards[i].transform.position;
            }

            float timer = 0f;

            while (timer < shuffleDuration)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    // Randomize the position of the cards
                    cards[i].transform.position = Vector3.Lerp(initialPositions[i], Random.insideUnitSphere * 3f, timer / shuffleDuration);
                }

                timer += Time.deltaTime;
                yield return null;
            }

            // Reset positions
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].transform.position = initialPositions[i];
            }

            // Interchange positions
            for (int i = 0; i < cards.Length; i++)
            {
                int randomIndex = Random.Range(i, cards.Length);
                Vector3 tempPosition = cards[i].transform.position;
                cards[i].transform.position = cards[randomIndex].transform.position;
                cards[randomIndex].transform.position = tempPosition;
            }

            isShuffling = false;
            _shufflingFX.Stop();
        }

    private void ShuffleCards()
    {
        if (!isShuffling)
        {
            isShuffling = true;
         //   StartCoroutine(ShuffleAnimationCoroutine());
       //  ShuffleCardsNew();
        }
    }

 

    public   void ShuffleCardsNew()
    {
        List<Vector3> originalPositions = new List<Vector3>();
        Vector3 centralPosition = new Vector3(50, 100, 0); // Define your central gathering position
    
        // Store original positions of cards
        foreach (GameObject card in cards)
        {
            originalPositions.Add(card.transform.position);
        }

        // Step 1: Move all cards to a central position (stack-like structure)
        for (int i = 0; i < cards.Length; i++)
        {
            GameObject card = cards[i];
            Vector3 stackPosition = centralPosition + new Vector3(0, 6f * i, 0); // Small offset for stacking
            LeanTween.move(card, stackPosition, 1f).setEase(LeanTweenType.easeInOutQuad);
        }

        // Step 2: Shuffle cards in place with slight movements
        LeanTween.delayedCall(1f, () =>
        {
            for (int i = 0; i < cards.Length; i++)
            {
                GameObject card = cards[i];
                Vector3 shufflePosition = centralPosition + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-3f, 0.1f), 0);
                LeanTween.move(card, shufflePosition, 0.5f).setEase(LeanTweenType.easeInOutQuad).setLoopPingPong(1);
            }
        });
        shufflecardsprits();
        // Step 3: Move cards to their final random positions
        LeanTween.delayedCall(2f, () =>
        {
            // Shuffle original positions
            for (int i = 0; i < originalPositions.Count; i++)
            {
                int randomIndex = Random.Range(0, originalPositions.Count);
                Vector3 temp = originalPositions[i];
                originalPositions[i] = originalPositions[randomIndex];
                originalPositions[randomIndex] = temp;
            }

            // Move to final positions
            for (int i = 0; i < cards.Length; i++)
            {
                GameObject card = cards[i];
                LeanTween.move(card, originalPositions[i], 1f).setEase(LeanTweenType.easeInOutQuad);
            }
        });
    }




    public void Assign_Random_Marine()
    {
        print(_marine_images_list.Count);
        _random_marine_index = Random.Range(0, _marine_images_list.Count - 1);
        print(_random_marine_index);
        _random_marine.sprite = _marine_images_list[_random_marine_index].sprite;
    }

    void AnimateCardInStarting(float time)
    {
        foreach (CardAnimator card in _cardAnimators)
        {
            card.StartWiggling(time);
        }
    }
    

    

    IEnumerator Showcards(float time)
    {
        foreach (CardAnimator card in _cardAnimators)
        {
            card.RevealAndHide(time);
           // Assuming time is the duration of the reveal/hide animation
        }
        yield return new WaitForSeconds(time+1);  
    }
        // isWiggling = true;
        // Wiggle left-right
    

}
