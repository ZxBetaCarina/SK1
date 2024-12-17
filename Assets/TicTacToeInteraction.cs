using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeInteraction : MonoBehaviour
{
    [SerializeField] private Transform[] _rayCastOrigins;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private PaytableCalculator _paytableCalculator;

    [Header("UI")]
    public Image timeBar;

    public int timebartime;

    private (Icons sprite, RaycastOriginTransforms patterns) bestAnswer;
    public static (Icons sprite, RaycastOriginTransforms patterns) bestAnswerstatic;

    [SerializeField] public GameObject fx;

    public string AnswerSelectedMessage = null;
    public string BestSpotName;
    private bool answerSelected = false;
    private bool bestAnswerExist = false;
    private bool bestAnswerSelected = false;

    bool _selected_best_answer;
    
    public GameObject indicatorSpritePrefab; // Prefab for the indicator sprite
    public float indicatorZOffset = -1f; 
    

    public async void SelectPosition(Transform pos)
    {
        while (ImageCylinderSpawner.Instance.isRotating)
        {
            await Task.Delay(16);
        }

        List<(Icons, RaycastOriginTransforms patterns)> possibleIcon = CheckForWinningPatterns.INSTANCE.CheckPatternsTicTacToe(pos);
        List<(Icons, RaycastOriginTransforms patterns)> filteredPossiblilities = new List<(Icons, RaycastOriginTransforms patterns)>(possibleIcon);
        (Icons sprite, RaycastOriginTransforms patterns) selectedAns = (null, null);
        Collider2D iconSelectedFromPossiblilities = null;

        if (possibleIcon.Count > 0)
        {
            selectedAns = _paytableCalculator.ReturnHighestValuePattern(filteredPossiblilities);

            for (int i = 0; i < possibleIcon.Count; i++)
            {
                if (selectedAns.patterns != null && selectedAns.patterns.raycastOrigins.Contains(pos))
                {
                    if (AllMatchingPattern(selectedAns.sprite.GetSprite(), selectedAns.patterns))
                    {
                        break;
                    }

                    RaycastHit2D iconSelected = Physics2D.Raycast(pos.position, Vector3.forward);
                    if (iconSelected && iconSelected.collider.GetComponent<Icons>().GetSprite() != selectedAns.Item1.GetSprite())
                    {
                        iconSelectedFromPossiblilities = iconSelected.collider;
                        //iconSelected.collider.GetComponent<SpriteRenderer>().sprite = selectedAns.sprite.GetSprite();
                        //iconSelected.collider.GetComponent<Icons>().IconSprite = selectedAns.sprite.GetSprite();
                        //iconSelected.collider.GetComponent<Icons>().Name = selectedAns.sprite.Name;
                        break;
                    }
                    else
                    {
                        filteredPossiblilities.Remove(selectedAns);
                    }
                }
                selectedAns = _paytableCalculator.ReturnHighestValuePattern(filteredPossiblilities);
                
                
            }
        }
        Debug.Log($"Selected Answer: {selectedAns.Item1.Name}, Best Answer: {bestAnswer.sprite.Name}");

        answerSelected = true;
        Deactivate();
        if (selectedAns.sprite != null)
        {
            if (bestAnswer.sprite != null && selectedAns.Item1.Name == bestAnswer.sprite.Name)
            {
                //AnswerSelectedMessage =  $"And the {bestansname} was the best spot.";
                bestAnswerSelected = true;
            }
            else
            {
                //AnswerSelectedMessage = $"And the {bestansname} was the best spot.";
                //OnNotSelectingBestAnswer();
                await Task.Delay(1000);
            }
            //print(AnswerSelectedMessage);

            if (iconSelectedFromPossiblilities != null)
            {
                iconSelectedFromPossiblilities.GetComponent<SpriteRenderer>().sprite = selectedAns.sprite.GetSprite();
                iconSelectedFromPossiblilities.GetComponent<Icons>().Name = selectedAns.sprite.Name;
            }
            ImageCylinderSpawner.Instance.CheckWinningCondition(selectedAns.patterns);
            _selected_best_answer = true;
        }
        else
        {
            OnNotSelectingBestAnswer();
            ImageCylinderSpawner.Instance.CheckWinningCondition();
        }
    }

    public void OnNotSelectingBestAnswer()
    {
        if (bestAnswerSelected == false)
        {
        
        if (bestAnswer.sprite != null)
        {
            bool isDone = false;
            foreach (Transform origin in bestAnswer.patterns.raycastOrigins)
            {
                RaycastHit2D hit = Physics2D.Raycast(origin.position, Vector3.forward);
                if (hit && hit.collider.GetComponent<SpriteRenderer>().sprite != bestAnswer.sprite.IconSprite)
                {
                    hit.collider.GetComponent<SpriteRenderer>().color = Color.red;
                    SpawnIndicator(hit.collider.transform);
                    isDone = true;
                    break;
                }
            }

            if (!isDone && (AnswerSelectedMessage != "Wild Got It"))
            {
                foreach (Transform origin in bestAnswer.patterns.raycastOrigins)
                {
                    RaycastHit2D hit = Physics2D.Raycast(origin.position, Vector3.forward);
                    if (hit)
                    {
                        //hit.collider.GetComponent<SpriteRenderer>().color = Color.red;
                        //SpawnIndicator(hit.collider.transform);
                    }
                }
            }
        }
        }
        // CheckForWinningPatterns.PatternNotFound.Invoke();            ///************************* TEMP REMOVED *************************///
    }

    private void Update()
    {
        //Debug.Log(bestAnswerSelected);
    }

    private void SpawnIndicator(Transform targetTransform)
    {
        if (indicatorSpritePrefab != null)
        {
            // Instantiate the indicator sprite
            GameObject indicator = Instantiate(indicatorSpritePrefab, targetTransform.position, Quaternion.identity);
        
            // Optionally, apply a slight vertical offset to position the indicator above the best answer sprite
            indicator.transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y , targetTransform.position.z + indicatorZOffset);
        }
    }

    IEnumerator StartTimer()
    {
        if(!UIManager.INSTANCE._followPanel.gameObject.activeInHierarchy)
        {
            UIManager.INSTANCE._spinButton.interactable = false;
        }
        
        while (ImageCylinderSpawner.Instance.isRotating)
        {
            yield return new WaitForSeconds(3);
        }
        yield return new WaitForSeconds(1.5f);
        FindBestAnswers();

        if (bestAnswer.sprite != null)
        {
            //float timer = _paytableCalculator.GetTimeForIcon(bestAnswer.sprite.Name);
            float timer = timebartime;
            StartCoroutine(Timer(timer));
        }
        else
        {
            ImageCylinderSpawner.Instance.CheckWinningCondition();
        }
    }

    private void FindBestAnswers()
    {
        if (bestAnswerExist)
        {
            return;
        }

        (Icons sprite, RaycastOriginTransforms patterns) bestAns = (null, null);
        List<(Icons, RaycastOriginTransforms patterns)> possibleIcon = CheckForWinningPatterns.INSTANCE.CheckPatternsTicTacToe();

        string str = "";
        foreach ((Icons, RaycastOriginTransforms patterns) possibility in possibleIcon)
        {
            str += possibility.Item1.ToString() + ":" + possibility.patterns.listName + "\n";
        }
        // Debug.Log("possibilities " + "\n" + str);

        (Icons sprite, RaycastOriginTransforms patterns) selectedAns = _paytableCalculator.ReturnHighestValuePattern(possibleIcon);

        bestAns = selectedAns;

        if (bestAns.sprite != null)
        {
            bestAnswer = bestAns;
            bestAnswerExist = true;
            //bestansname = bestAnswer.sprite.Name2Show;

            BestSpotName = $"And the {bestAnswer.sprite.Name2Show} was the best spot.";
            
            //Debug.Log("Best Answer Icon name2: " + bestAnswer.sprite.Name2Show);
        }
        else
        {
            BestSpotName = $"And you have made the best spot.";
        }
    }

    private IEnumerator Timer(float netTime)
    {
        if (netTime == -1)
            yield break;

        timeBar.transform.parent.gameObject.SetActive(true);

        float timeCounter = netTime;
        while (timeCounter > 0 && !answerSelected)
        {
            timeBar.fillAmount = timeCounter / netTime;
            timeCounter -= Time.deltaTime;
            //spinButton.interactable = false;
            //OnNotSelectingBestAnswer();
            yield return null;
        }
        if (!answerSelected)
        {
            timeBar.fillAmount = 0;
            print("Pattern Not found : Timer");
            CheckForWinningPatterns.PatternNotFound();
            OnNotSelectingBestAnswer();
            //  Debug.Log("Yash2");
        }
        else
        {
            if (!_selected_best_answer)
            {
                OnNotSelectingBestAnswer();
            }
        }

        Deactivate();
        //UIManager.INSTANCE._spinButton.interactable = true;
        //yield return new WaitForSeconds(2);
        timeBar.transform.parent.gameObject.SetActive(false);
        Deactivate();
    }

    public bool AllMatchingPattern(Sprite selectedSprite, RaycastOriginTransforms pattern)
    {
        foreach (Transform obj in pattern.raycastOrigins)
        {
            RaycastHit2D hit = Physics2D.Raycast(obj.position, Vector3.forward);
            if (!hit || selectedSprite != hit.collider.GetComponent<SpriteRenderer>().sprite)
            {
                return false;
            }
        }
        return true;
    }

    public void Activate()
    {
        if (ImageCylinderSpawner.Instance.CylinderSpawning || (_buttons[0].gameObject.activeInHierarchy && _buttons[0].interactable == true))
        {
            return;
        }

        foreach (Button button in _buttons)
        {
            button.interactable = true;
            button.gameObject.SetActive(true);
        }
        StartCoroutine(StartTimer());
    }

    public void Deactivate()
    {
        foreach (Button button in _buttons)
        {
            if (button.gameObject.activeInHierarchy && button.interactable)

                button.interactable = false;
            button.gameObject.SetActive(false);
        }
    }
}
