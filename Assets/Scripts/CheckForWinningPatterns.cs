using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class RaycastOriginTransforms
{
    public string listName;
    public List<Transform> raycastOrigins;
}

public class CheckForWinningPatterns : MonoBehaviour
{
    public List<RaycastOriginTransforms> raycastPatterns;
    public static CheckForWinningPatterns INSTANCE;
    [SerializeField] private GameObject leftSpins;
    [SerializeField] private TMP_Text leftSpinsText;
    [SerializeField] private GameObject reviewImgParent;

    [SerializeField] public List<Sprite> _Reveal_Buttons;
    [SerializeField] public List<Sprite> _Spin_Buttons;

    public Image _reveal_btn_img;
    public Image _spin_btn_img;

    //[SerializeField] public Button _reveal_button;
    [SerializeField] private TMP_Text reviewText;

    [Header("Reward Calculations")]
    [SerializeField] private PayTable_SO _paytables;

    [SerializeField] private TicTacToeInteraction ticTacToeInteraction;

    public static Action CoolDownRubicButton;

    public static Action<string> PatternFound;
    public static Action PatternNotFound;
    private int noOfPatterns = 0;
    private bool _isJackPotMode = false;
    private bool _checking = false;
    private bool isLastSpin = false;

    private bool isReviewActive = false;

    private int spinCounts = 15;

    public bool isBonus = false;

    private string _detectedCharacter;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void FinaliseBet()
    {
        reviewImgParent.SetActive(false);
        _reveal_btn_img.sprite = _Reveal_Buttons[1];
        //UIManager.INSTANCE._spinButton.interactable = false;
        _spin_btn_img.sprite = _Spin_Buttons[1];
        UIManager.INSTANCE._revealbutton.SetActive(false);
    }

    public void ReviewImages(bool ShowReview)
    {
        if (isReviewActive)
        {
            isReviewActive = false;
            return;
        }
        else if (!ShowReview)
        {
            return;
        }
        StartCoroutine(ShowImages());
    }


    IEnumerator ShowImages()
    {
        //reviewBtn.interactable = false;

        isReviewActive = true;

        reviewText.text = "Hide Preview";

        reviewImgParent.SetActive(false);

        _reveal_btn_img.sprite = _Reveal_Buttons[1];

        UIManager.INSTANCE._spinButton.interactable = false;

        float t_timer = GameController.Instance._reveal_time;

        while (t_timer > 0)
        {
            t_timer -= Time.deltaTime;

            if (ImageCylinderSpawner.Instance.isRotating)
            {
                isReviewActive = false;
                yield break;
            }

            if (!isReviewActive)
            {
                reviewImgParent.SetActive(true);
                _reveal_btn_img.sprite = _Reveal_Buttons[0];
                UIManager.INSTANCE._spinButton.interactable = true;
                reviewText.text = "Preview";

                yield break;
            }
            yield return null;
        }

        //UIManager.INSTANCE._revealbutton.interactable = true;
        reviewImgParent.SetActive(true);
        _reveal_btn_img.sprite = _Reveal_Buttons[0];
        UIManager.INSTANCE._spinButton.interactable = true;
        reviewText.text = "Review";

        isReviewActive = false;
    }

    private void OnEnable()
    {
        leftSpins.SetActive(false);
    }

    private void OnDisable()
    {
    }

    public IEnumerator CheckForPatterns(RaycastOriginTransforms patternProvided = null)
    {
        if (_checking || UIManager.INSTANCE.RubicMode) yield break;


        _checking = true;
        //yield return new WaitForSeconds(.1f);

        if (patternProvided != null)
        {
            CheckPatternsInList(patternProvided.raycastOrigins);
        }

        if (ImageCylinderSpawner.Instance.IsBonus1)
        {
            yield return new WaitWhile(() => ImageCylinderSpawner.Instance.isRotating);

            foreach (RaycastOriginTransforms raycastOrigins in raycastPatterns)
            {
                CheckPatternsInList(raycastOrigins.raycastOrigins);
            }

            if (noOfPatterns >= 1)
            {
                PatternFound?.Invoke(_detectedCharacter);
                ImageCylinderSpawner.Instance.won = false;
            }
            yield return new WaitForSeconds(1);

            ImageCylinderSpawner.Instance.StartBonusSpin();

            spinCounts--;

            leftSpinsText.text = spinCounts + "/" + 15;

            _checking = false;



            if (spinCounts < 0)
            {
                isBonus = false;
                spinCounts = 0;
                isLastSpin = true;

                noOfPatterns = 0;
                leftSpins.SetActive(false);
                SceneManager.LoadScene(1);

            }

            noOfPatterns = 0;
            yield break;
        }

        //if (isBonus)
        //{
        //    isBonus = false;
        //}

        if (noOfPatterns >= 1 && !_isJackPotMode)
        {
            PatternFound?.Invoke(_detectedCharacter);

        }
        else
        {
            if (isLastSpin)
            {
                leftSpins.SetActive(false);
                yield return new WaitForSeconds(2f);
                SceneManager.LoadScene(1);
            }
            else
            {
                print("Pattern Not found 1");
                PatternNotFound?.Invoke();
            }
        }

        _isJackPotMode = false;

        noOfPatterns = 0;
        _checking = false;
    }

    public void CheckPatterns()
    {
        if (!_checking)
        {
            _isJackPotMode = GameController.Instance.JackPotMode;
            StartCoroutine(CheckForPatterns());
        }
    }
    public void CheckPatterns(RaycastOriginTransforms pattern)
    {
        if (!_checking)
        {
            _isJackPotMode = GameController.Instance.JackPotMode;
            StartCoroutine(CheckForPatterns(pattern));
        }
    }

    public List<(Icons, RaycastOriginTransforms)> CheckPatternsTicTacToe(Transform raycastPoint)
    {
        List<(Icons, RaycastOriginTransforms)> possibleIcons = new List<(Icons, RaycastOriginTransforms)>();
        foreach (RaycastOriginTransforms pattern in raycastPatterns)
        {
            if (!pattern.raycastOrigins.Contains(raycastPoint))
                continue;

            List<Icons> prev = new List<Icons>();
            Icons selected = null;
            int matching = 0;
            foreach (Transform raycastPoints in pattern.raycastOrigins)
            {
                RaycastHit2D hit = Physics2D.Raycast(raycastPoints.position, Vector3.forward);

                if (hit)
                {
                    Icons icon = hit.collider.GetComponent<Icons>();
                    if (icon == null)
                    {
                        continue;
                    }

                    //    if (matching == 0)
                    //    {
                    //        prev.Add(icon);
                    //        matching++;
                    //    }
                    //    else
                    //    {
                    //        for (int i = 0; i < prev.Count; i++)
                    //        {
                    //            if (prev[i].GetSprite() == icon.GetSprite())
                    //            {
                    //                selected = prev[i];
                    //                matching++;
                    //            }
                    //        }
                    //        if (selected == null)
                    //        {
                    //            prev.Add(icon);
                    //        }
                    //        else
                    //        {
                    //            possibleIcons.Add((selected, pattern));
                    //            break;
                    //        }
                    //    }

                    prev.Add(icon);
                }
            }

            if (prev.Count == 3)
            {
                if (prev[0].GetSprite() == prev[1].GetSprite() || prev[0].GetSprite() == prev[2].GetSprite())
                {
                    possibleIcons.Add((prev[0], pattern));
                }
                else if (prev[1].GetSprite() == prev[2].GetSprite())
                {
                    possibleIcons.Add((prev[1], pattern));
                }
            }
            prev.Clear();
        }
        return possibleIcons;
    }

    public List<(Icons, RaycastOriginTransforms)> CheckPatternsTicTacToe()
    {
        List<(Icons, RaycastOriginTransforms)> possibleIcons = new List<(Icons, RaycastOriginTransforms)>();
        foreach (RaycastOriginTransforms pattern in raycastPatterns)
        {
            List<Icons> prev = new List<Icons>();
            foreach (Transform raycastPoints in pattern.raycastOrigins)
            {
                RaycastHit2D hit = Physics2D.Raycast(raycastPoints.position, Vector3.forward);

                if (hit)
                {
                    Debug.DrawRay(raycastPoints.position, Vector3.forward * 100, Color.red, float.MaxValue);
                    Icons icon = hit.collider.GetComponent<Icons>();
                    if (icon != null)
                    {
                        prev.Add(icon);
                    }
                }
            }
            if (prev.Count == 3)
            {
                /*if (prev[0].GetComponent<SpriteRenderer>().sprite == prev[1].GetComponent<SpriteRenderer>().sprite)
                {
                    possibleIcons.Add((prev[0], pattern));
                    Debug.Log(possibleIcons);
                }
                else if (prev[1].GetComponent<SpriteRenderer>().sprite == prev[0].GetComponent<SpriteRenderer>().sprite)
                {
                    possibleIcons.Add((prev[1], pattern));
                    Debug.Log(possibleIcons);
                }
                else if (prev[1].GetComponent<SpriteRenderer>().sprite == prev[2].GetComponent<SpriteRenderer>().sprite)
                {
                    possibleIcons.Add((prev[1], pattern));
                    Debug.Log(possibleIcons);
                }
                else if (prev[2].GetComponent<SpriteRenderer>().sprite == prev[1].GetComponent<SpriteRenderer>().sprite)
                {
                    possibleIcons.Add((prev[2], pattern));
                    Debug.Log(possibleIcons);
                }
                else if (prev[0].GetComponent<SpriteRenderer>().sprite == prev[2].GetComponent<SpriteRenderer>().sprite)
                {
                    possibleIcons.Add((prev[0], pattern));
                    Debug.Log(possibleIcons);
                }
                else if (prev[2].GetComponent<SpriteRenderer>().sprite == prev[0].GetComponent<SpriteRenderer>().sprite)
                {
                    possibleIcons.Add((prev[2], pattern));
                    Debug.Log(possibleIcons);
                }*/
                if (prev[0].IconSprite == prev[1].IconSprite && prev[0].IconSprite == prev[2].IconSprite)
                {
                    possibleIcons.Add((prev[0], pattern));
                }
                else if (prev[0].IconSprite == prev[1].IconSprite || prev[0].IconSprite == prev[2].IconSprite)
                {
                    possibleIcons.Add((prev[0], pattern));
                }
                else if (prev[1].IconSprite == prev[2].IconSprite)
                {
                    possibleIcons.Add((prev[1], pattern));
                }
            }
            else if (prev.Count < 3)
                Debug.Log("Detected less than 3" + prev.Count);
            else if (prev.Count > 3)
                Debug.Log("Detected more than 3" + prev.Count);

            prev.Clear();
        }
        return possibleIcons;
    }

    private void CheckPatternsInList(List<Transform> transformList)
    {
        int matchCount = 0;
        Sprite spritePrev = null;
        List<Vector3> detected = new List<Vector3>();
        Debug.Log("Yashhhh" + detected);

        //Find patterns in ascending list order
        for (int i = 0; i < transformList.Count; i++)
        {
            if (!transformList[i].gameObject.activeInHierarchy)
            {
                //Debug.Log("Continued");
                continue;
            }
            RaycastHit2D detectedObject = Physics2D.Raycast(transformList[i].position, Vector3.forward);
            Debug.Log("Yashhhh" + detectedObject);
            //detect first image for the pattern
            if (matchCount == 0)
            {
                if (detectedObject.transform != null)
                {
                    //matchCount = 1 for first detected image in the pattern
                    matchCount++;
                    spritePrev = detectedObject.collider.GetComponent<SpriteRenderer>().sprite;
                    detected.Add(detectedObject.transform.position);
                }
            }

            //already detected first image 
            else
            {

                if (detectedObject.transform != null) // 3*3
                {
                    //check name of previous image and newly detected image
                    if (spritePrev == detectedObject.collider.GetComponent<Icons>().GetSprite())
                    {
                        matchCount++;
                        detected.Add(detectedObject.transform.position);
                    }
                    else
                    {
                        //name of images are not matching
                        //strart count from one and check for other pattern
                        matchCount = 1;
                        spritePrev = detectedObject.collider.GetComponent<SpriteRenderer>().sprite;
                        detected = new List<Vector3>();
                    }
                    if (matchCount >= 3)
                    {
                        noOfPatterns++;

                        Icons detectedObjIcon = detectedObject.collider.GetComponent<Icons>();
                        _detectedCharacter = detectedObjIcon.Name;

                        if (detectedObjIcon.Name == "Bonus1")
                        {
                            isBonus = true;
                            ImageCylinderSpawner.Instance.IsBonus1 = true;
                            StartCoroutine(UIManager.INSTANCE.FreeSpinShow());

                            StartCoroutine(ShowDelaysSpinsLeft());

                            //SceneManager.LoadScene
                        }
                        else if (detectedObjIcon.Name == "Bonus2")
                        {
                            isBonus = true;
                            Debug.Log(isBonus);
                            StartCoroutine(BonusSecond());
                        }
                        else if (detectedObjIcon.Name == "Bonus3")
                        {
                            isBonus = true;
                            Debug.Log(isBonus);
                            StartCoroutine(BonusThird());
                        }
                        foreach (Vector3 pattern in detected)
                        {
                            Debug.Log(detected.Count);
                            if (GameController.Instance != null)
                            {
                                GameController.Instance._patterns.Add(pattern);
                                if (!GameController.Instance._patternFormed.Contains(_detectedCharacter))
                                    GameController.Instance._patternFormed.Add(_detectedCharacter);
                                Debug.Log("Pattern Found");
                            }
                        }

                    }
                }
            }
        }
    }


    IEnumerator ShowDelaysSpinsLeft()
    {
        yield return new WaitWhile(() => ImageCylinderSpawner.Instance.isRotating);
        leftSpins.SetActive(true);
        leftSpinsText.text = 15 + "/" + 15;
    }

    private IEnumerator BonusSecond()
    {
        PlayerPrefs.SetFloat("_spinning", 1);
        //Show Bonus UI
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(2);
    }

    private IEnumerator BonusThird()
    {
        PlayerPrefs.SetFloat("_spinning", 1);
        //Show Bonus UI
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(3);
    }

    public bool CheckForFacesRubicsCube(List<Icons> face)
    {
        Sprite recognisedSprite = null;
        int faceMatches = 0;
        foreach (Icons icon in face)
        {
            if (recognisedSprite == null)
            {
                recognisedSprite = icon.GetSprite();
                faceMatches++;
                continue;
            }

            if (icon.GetSprite() != recognisedSprite)
            {
                break;
            }
            else
            {
                faceMatches++;
            }
        }
        return faceMatches == 9;
    }


    public void OnRevealEnable()
    {
        StartCoroutine(OnRevealEnabled());
    }

    private IEnumerator OnRevealEnabled()
    {
        yield return new WaitWhile(() => _checking);
        StartCoroutine(CheckForPatterns());
    }
}
