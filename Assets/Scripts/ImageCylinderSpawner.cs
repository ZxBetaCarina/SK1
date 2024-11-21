using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ImageCylinderSpawner : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private StopOnReelInteraction _stopReelOnInteraction;
    [SerializeField] private TicTacToeInteraction _ticTacToeInteraction;
    
    [Header("RNG")] [SerializeField] private WeightedRNG<GameObject> slotRNGItems;

    [Header("Slot Image Cylinder")]
    [SerializeField] private LayerMask _imageLayer;
    public GameObject[] imagePrefabs;  // Array to hold different sets of image prefabs
    public GameObject[] imagePrefabsFreeSpin;
    public int numberOfImages = 21;
    public float cylinderRadius = 5f;
    public int numberOfCylinders = 3; // Update the number of cylinders
    public float rotationSpeed = 50f;
    public float distanceBetweenCylinders = 2f;  // Distance between cylinders
    public float[] rotations;
    public bool isRotating = false;
    public Transform[] cylinderParents;
    public Vector3[] cylinderSpawnPoints;
    public bool[] cylinderRotationStates;
    private Icons[] _spawnedIcons;
    public bool SpinAllowed = false;
    public Icons[] GetSpawnedIcons { get => _spawnedIcons; }
    private bool HitPlayAuto;
    private bool ReviewAuto;
    private bool DropAuto;

    public int _currentCylinder = 0;

    public AudioSource slotAudio;
    private GameObject allCylindersParent;  // New parent for all cylinders

    public Slider speedSlider;  // Reference to the UI slider for speed adjustment
    public float delay = 2f;
    public TMP_Text speedText;  // Reference to the UI text for displaying speed
    //public TMP_Text coinsText;  // Reference to the UI text for displaying coins
    private int coins = 100;  // Initial coins
    private bool hasMoney = true;
    public Button muteButton;  // Reference to the UI button for muting/unmuting
    public bool isMuted = false;  // Flag to track whether the game is currently muted
    public GameObject fxPrefab;
    public GameObject spinButtonDup;
    public string nextSceneName = "MainMenu";
    public int _difficultyFactor = 0;
    public bool CylinderSpawning = false;

    public GameObject popupPanel;  // Assign the pop-up panel in the Inspector
                                   // public TMP_Text popupText;  // Assign the Text element for the message
    public Button closeButton;  // Assign the Button element for closing the pop-up
    private bool checkedForPatterns = false;
    public Button refreshBtn;
    public static ImageCylinderSpawner Instance;        //Singleton Instance
    private bool _betConfirmed = false;
    public bool won = false;

    private bool bonusSpins = false;
    public bool IsBonus1 = false;

    private bool imagesRefreshed = false;
    
    public AudioSource bonusSpinAudio;



    [SerializeField] private GameObject hideImage;
    [SerializeField] private GameObject hideImage1;
    [SerializeField] private GameObject hideImage2;
    [SerializeField] private GameObject hideImage3;
    [SerializeField] private GameObject hideImage4;
    [SerializeField] private GameObject hideImage5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _spawnedIcons = new Icons[numberOfImages];
        SpinAllowed = false;

    }

    private void OnEnable()
    {
        GameController.BetConfirmed += OnBetConfirmed;
        GameController.BetChanged += OnBetChanged;

    }
    private void OnDisable()
    {
        GameController.BetConfirmed -= OnBetConfirmed;
        GameController.BetChanged -= OnBetChanged;
    }
    void Start()
    {
        allCylindersParent = new GameObject("AllCylindersParent");  // Create a new empty GameObject as the parent

        cylinderParents = new Transform[numberOfCylinders];

        cylinderRotationStates = new bool[numberOfCylinders];
        SpawnCylinders();

        // Add a listener to the slider
        speedSlider.onValueChanged.AddListener(ChangeRotationSpeed);
        // Add a listener to the mute button
        //muteButton.onClick.AddListener(ToggleMute);

        // Display initial coins
        UpdateCoinsText();

        // Ensure the pop-up panel is initially hidden
        popupPanel.SetActive(false);

        // Add a listener to the close button
        closeButton.onClick.AddListener(HidePopup);


        //AutoSpin
        if (PlayerPrefs.HasKey("HitPlayAuto"))
        {
            HitPlayAuto = PlayerPrefs.GetInt("HitPlayAuto") == 1; // Convert the saved int value to bool
        }
        else
        {
            // If the key doesn't exist, set the default value
            HitPlayAuto = false;
            // Save the default value to PlayerPrefs
            PlayerPrefs.SetInt("HitPlayAuto", HitPlayAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes
        }

        //ReviewAuto
        if (PlayerPrefs.HasKey("ReviewAuto"))
        {
            ReviewAuto = PlayerPrefs.GetInt("ReviewAuto") == 1; // Convert the saved int value to bool
        }
        else
        {
            // If the key doesn't exist, set the default value
            ReviewAuto = false;
            // Save the default value to PlayerPrefs
            PlayerPrefs.SetInt("ReviewAuto", ReviewAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes
        }


        //DropAuto
        if (PlayerPrefs.HasKey("DropAuto"))
        {
            DropAuto = PlayerPrefs.GetInt("DropAuto") == 1; // Convert the saved int value to bool
        }
        else
        {
            // If the key doesn't exist, set the default value
            DropAuto = false;
            // Save the default value to PlayerPrefs
            PlayerPrefs.SetInt("DropAuto", DropAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes
        }

        //PlayerPrefs.SetFloat("_spinning", 1);
        if (PlayerPrefs.GetFloat("_spinning") == 0)
        {
            GameController.Instance._exit_button.SetActive(false);
            PlayerPrefs.SetFloat("_spinning", 1);
        }
        else
        {
            GameController.Instance._exit_button.SetActive(true);
        }
    }

    void Update()
    {
        //Debug.Log(SpinAllowed);
        //Debug.Log(HitPlayAuto);
        if (isRotating)
        {
            RotateCylinders();
            if (checkedForPatterns)
            {
                checkedForPatterns = false;
            }
        }
        if (UIManager.INSTANCE.FollowMeCheck == false && SpinAllowed == false && HitPlayAuto == true)
        {
            //UIManager.INSTANCE.ToggleBool();

            StartRotating();
            Spin();
            HitPlayAuto = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("HitPlayAuto", HitPlayAuto ? 0 : 1); // Convert bool to int
            PlayerPrefs.Save();

        }

        if (UIManager.INSTANCE.FollowMeCheck == false && SpinAllowed == false && DropAuto == true)
        {
            //UIManager.INSTANCE.ToggleBool();

            RefreshImages();
            DropAuto = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("DropAuto", DropAuto ? 0 : 1); // Convert bool to int
            PlayerPrefs.Save();

        }
        if (UIManager.INSTANCE.FollowMeCheck == false && SpinAllowed == false && ReviewAuto == true)
        {
            //UIManager.INSTANCE.ToggleBool();

            GameController.Instance.OnClickReviewButton();
            GameController.Instance.Revealed();
            ReviewAuto = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("ReviewAuto", ReviewAuto ? 0 : 1); // Convert bool to int
            PlayerPrefs.Save();

        }
    }

    private void OnBetChanged()
    {
        //_difficultyFactor = GameController.Instance.CurrentBetIndex;
        //RefreshCylinder();
    }
    public void Spin()
    {
        if (!_betConfirmed || CylinderSpawning) return;
        if (isRotating)
        {
            StopNextCylinder();
            // UIManager.INSTANCE.ToggleBool();
        }
        else
        {
            if (hasMoney == true && coins > 0)
            {
                OnStartSpinning();
                // UIManager.INSTANCE.ToggleBool();
            }
            // Check if the player has enough coins
            if (coins >= 20)
            {
                // Deduct coins
                UpdateCoinsText();  // Update UI text
                if (fxPrefab != null)
                {
                    // Instantiate the FX Prefab at the button's position
                    GameObject fxInstance = Instantiate(fxPrefab, spinButtonDup.transform.position, Quaternion.identity);

                    slotAudio.Play();
                    // Optionally, you can destroy the fxInstance after a certain duration
                    Destroy(fxInstance, 2f); // Adjust the duration as needed
                }
            }
            else
            {
                // Display not enough money message
                Debug.Log("Not enough money!");
            }
        }
    }

    public void HitPlay()
    {
        if (UIManager.INSTANCE.FollowMeCheck == true && SpinAllowed == true)
        {
            UIManager.INSTANCE.FollowMeCheck = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("FollowMeCheck", UIManager.INSTANCE.FollowMeCheck ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes
            HitPlayAuto = true;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("HitPlayAuto", HitPlayAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes

            ReviewAuto = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("ReviewAuto", ReviewAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes

            DropAuto = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("DropAuto", DropAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes
            SceneManager.LoadScene(1);
        }
            GameController.Instance._exit_button.SetActive(false);
            PlayerPrefs.SetFloat("_spinning", 0);
    }

    public void AutoDrop()
    {
        print("Auto Drop");
        if (UIManager.INSTANCE.FollowMeCheck == true && SpinAllowed == true)
        {
            UIManager.INSTANCE.FollowMeCheck = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("FollowMeCheck", UIManager.INSTANCE.FollowMeCheck ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes
            DropAuto = true;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("DropAuto", DropAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes

            HitPlayAuto = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("HitPlayAuto", HitPlayAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes

            ReviewAuto = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("ReviewAuto", ReviewAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes

            SceneManager.LoadScene(1);


        }
    }
    public void AutoPreview()
    {
        if (UIManager.INSTANCE.FollowMeCheck == true && SpinAllowed == true)
        {
            UIManager.INSTANCE.FollowMeCheck = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("FollowMeCheck", UIManager.INSTANCE.FollowMeCheck ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes
            ReviewAuto = true;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("ReviewAuto", ReviewAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes

            DropAuto = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("DropAuto", DropAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes

            HitPlayAuto = false;
            // Save the updated boolean value to PlayerPrefs
            PlayerPrefs.SetInt("HitPlayAuto", HitPlayAuto ? 1 : 0); // Convert bool to int
            PlayerPrefs.Save(); // Save changes

            SceneManager.LoadScene(1);


        }
    }
    public void OnBetConfirmed()
    {
        _difficultyFactor = GameController.Instance.CurrentBetIndex;
        _betConfirmed = true;
    }

    void RotateCylinders()
    {
        if (CylinderSpawning)
            return;

        for (int i = 0; i < cylinderParents.Length; i++)
        {
            if (cylinderRotationStates[i])
            {
                cylinderParents[i].Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

                // Check for collisions with centerpoint
                Collider[] colliders = cylinderParents[i].GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag("centerpoint"))
                    {
                        // Check for collisions with the centerpoint
                        Collider[] symbolColliders = Physics.OverlapBox(collider.transform.position, collider.bounds.extents);
                        foreach (Collider symbolCollider in symbolColliders)
                        {
                            // Adjust the rotation to align the symbol with the center
                            if (symbolCollider.CompareTag("SymbolImage"))
                            {
                                AlignSymbolWithCenter(symbolCollider.transform, collider.transform);
                                StopNextCylinder(); // Stop the rotation for the current cylinder
                            }
                        }
                    }
                }
            }
        }
    }

    void AlignSymbolWithCenter(Transform symbol, Transform center)
    {
        // Calculate the rotation needed to align the symbol with the center
        Vector3 targetDirection = center.position - symbol.position;
        targetDirection.y = 0f; // Ignore the y-axis for rotation
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Smoothly rotate the symbol towards the center
        StartCoroutine(RotateSymbolTowardsCenter(symbol, targetRotation, 1.0f));
    }

    IEnumerator RotateSymbolTowardsCenter(Transform symbol, Quaternion targetRotation, float duration)
    {
        float elapsed = 0f;
        Quaternion startRotation = symbol.rotation;

        while (elapsed < duration)
        {
            symbol.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the symbol is perfectly aligned
        symbol.rotation = targetRotation;
    }

    public void StartRotating()
    {
        if (!isRotating)
        {
            GameController.Instance.FinaliseBetOnClickSpin();
        }
    }

    public void DisableCylinders()
    {
        //cylinderParents[0].gameObject.SetActive(false);
        //cylinderParents[4].gameObject.SetActive(false);
        hideImage.SetActive(false);
        hideImage1.SetActive(false);
        hideImage2.SetActive(false);
        hideImage3.SetActive(false);
        hideImage4.SetActive(false);
        hideImage5.SetActive(false);
    }
    public void EnableRevealImages()
    {
        hideImage.SetActive(true);
        hideImage1.SetActive(true);
        hideImage2.SetActive(true);
        hideImage3.SetActive(true);
        hideImage4.SetActive(true);
        hideImage5.SetActive(true);
    }
    private void RefreshCylinder()
    {
        if (cylinderParents.Length > 0)
        {
            foreach (var parent in cylinderParents)
            {
                if (parent.childCount > 0)
                {
                    foreach (Transform child in parent)
                    {
                        Destroy(child.gameObject);
                    }
                }
                Destroy(parent.gameObject);
            }
        }
        Vector3 spawnPosition = new Vector3(0, distanceBetweenCylinders, 0);
        for (int i = 0; i < numberOfCylinders; i++)
        {
            SpawnImagesOnCylinder(spawnPosition, i, allCylindersParent.transform);
            spawnPosition += new Vector3(0f, distanceBetweenCylinders, 0f);
        }
    }

    void SpawnCylinders()
    {
        //refreshBtn.gameObject.SetActive(true);
        Vector3 spawnPosition = new Vector3(0, distanceBetweenCylinders, 0);
        // Debug.Log("Working");

        for (int i = 0; i < numberOfCylinders; i++)
        {
            SpawnImagesOnCylinder(spawnPosition, i, allCylindersParent.transform); // Pass the parent transform
            spawnPosition += new Vector3(0f, distanceBetweenCylinders, 0f);
        }

        // Rotate the entire cylinder to make it vertical
        cylinderParents[numberOfCylinders - 1].rotation = Quaternion.Euler(0f, 0f, 0f);

        // Set the new parent for all cylinders
        allCylindersParent.transform.SetParent(transform);
        foreach (var parent in cylinderParents)
        {
            parent.SetParent(allCylindersParent.transform);
        }
        allCylindersParent.transform.localPosition = new Vector3(8.56000042f, 2.04999995f, 7.21769333f);
        allCylindersParent.transform.rotation = Quaternion.Euler(0, 0, +90);
        allCylindersParent.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        //_spinButton.gameObject.SetActive(true);
    }


void SpawnImagesOnCylinder(Vector3 spawnPosition, int index, Transform parentTransform)
{
    float angleIncrement = 360f / numberOfImages;

    // Create a parent GameObject for the cylinder
    GameObject cylinderParent = new GameObject("CylinderParent" + index);
    cylinderParent.transform.position = cylinderSpawnPoints[index];
    cylinderParents[index] = cylinderParent.transform;  // Store the reference to the parent
    cylinderRotationStates[index] = true;  // Set the initial rotation state to true

    // Set the parent transform for the cylinder
    cylinderParent.transform.SetParent(parentTransform);

    GameObject lastPrefab = null;
    for (int i = 0; i < numberOfImages; i++)
    {
        // Calculate angle and position for the current image
        float angle = i * angleIncrement;
        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * cylinderRadius;
        float z = Mathf.Cos(Mathf.Deg2Rad * angle) * cylinderRadius;

        // Determine vertical offset based on the index
        float offsetY = index == 0 ? -0.03f : index == 1 ? 0.02f : 0.09f;
        Vector3 imageSpawnPosition = spawnPosition + new Vector3(x, offsetY, z);
        Quaternion spawnRotation = Quaternion.Euler(0f, -angle, 0f);

        // Determine the prefab to use
        GameObject selectedPrefab;
        if (IsBonus1)
        {
            // Select prefab from imagePrefabsFreeSpin
            int randomSetIndex = UnityEngine.Random.Range(0, imagePrefabsFreeSpin.Length);
            if (lastPrefab == null) lastPrefab = imagePrefabsFreeSpin[randomSetIndex];
            if (imagePrefabsFreeSpin[randomSetIndex] == lastPrefab)
            {
                randomSetIndex = UnityEngine.Random.Range(0, imagePrefabsFreeSpin.Length);
            }
            selectedPrefab = imagePrefabsFreeSpin[randomSetIndex];
        }
        else
        {
            // Use weighted randomization for prefab selection
            selectedPrefab = GetWeightedRNG.GetValue(slotRNGItems.ItemsForRNG);
            if (selectedPrefab == lastPrefab)
            {
                selectedPrefab = GetWeightedRNG.GetValue(slotRNGItems.ItemsForRNG);
            }
        }
        lastPrefab = selectedPrefab;

        // Instantiate the prefab
        GameObject imageObject = Instantiate(selectedPrefab, imageSpawnPosition, spawnRotation);
        _spawnedIcons[i] = imageObject.GetComponent<Icons>();
        imageObject.transform.parent = cylinderParent.transform;  // Set the cylinder's parent as the parent

        // Rotate the image to face the center
        float angleToCenter = Mathf.Atan2(imageSpawnPosition.z - spawnPosition.z, imageSpawnPosition.x - spawnPosition.x) * Mathf.Rad2Deg;
        imageObject.transform.rotation = Quaternion.Euler(180, 90 - angleToCenter, 90);
    }

    // Reset the cylinder parent transform
    cylinderParent.transform.localPosition = Vector3.zero;
    cylinderParent.transform.localRotation = Quaternion.identity;
    cylinderParent.transform.localScale = Vector3.one;
}

/*
    void SpawnImagesOnCylinder(Vector3 spawnPosition, int index, Transform parentTransform)
    {
        float angleIncrement = 360f / numberOfImages;

        GameObject cylinderParent = new GameObject("CylinderParent" + index);
        cylinderParent.transform.position = cylinderSpawnPoints[index];
        cylinderParents[index] = cylinderParent.transform;  // Store the reference to the parent
        cylinderRotationStates[index] = true;  // Set the initial rotation state to true

        // Set the parent transform for the cylinder
        cylinderParent.transform.SetParent(parentTransform);

        int lastSpriteIndex = -1;
        for (int i = 0; i < numberOfImages; i++)
        {
            float angle = i * angleIncrement;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * cylinderRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * cylinderRadius;
            float zero = -0.03f, one = 0.02f, two = 0.09f;
            Vector3 imageSpawnPosition = spawnPosition + new Vector3(x, index == 0 ? zero : index == 1 ? one : two, z);  // Offset image position relative to cylinder
            //Vector3 imageSpawnPosition = spawnPosition + new Vector3(x, 0f, z);  // Offset image position relative to cylinder
            Quaternion spawnRotation = Quaternion.Euler(0f, -angle, 0f);

            int randomSetIndex = -1;
            GameObject imageObject;
            if (IsBonus1)
            {
                randomSetIndex = UnityEngine.Random.Range(0, imagePrefabsFreeSpin.Length);
                if (lastSpriteIndex == -1)
                    lastSpriteIndex = randomSetIndex;
                while (randomSetIndex == lastSpriteIndex)
                {
                    randomSetIndex = UnityEngine.Random.Range(0, imagePrefabsFreeSpin.Length);
                }
                lastSpriteIndex = randomSetIndex;

                imageObject = Instantiate(imagePrefabsFreeSpin[randomSetIndex], imageSpawnPosition, spawnRotation);
            }
            else
            {
                randomSetIndex = UnityEngine.Random.Range(0, imagePrefabs.Length);
                if (lastSpriteIndex == -1)
                    lastSpriteIndex = randomSetIndex;
                while (randomSetIndex == lastSpriteIndex)
                {
                    randomSetIndex = UnityEngine.Random.Range(0, imagePrefabs.Length);
                }
                lastSpriteIndex = randomSetIndex;
                imageObject = Instantiate(imagePrefabs[randomSetIndex], imageSpawnPosition, spawnRotation);
            }
            _spawnedIcons[i] = imageObject.GetComponent<Icons>();
            imageObject.transform.parent = cylinderParent.transform;  // Set the cylinder's parent as the parent

            // Calculate the angle between the center and the current image
            float angleToCenter = Mathf.Atan2(imageSpawnPosition.z - spawnPosition.z, imageSpawnPosition.x - spawnPosition.x) * Mathf.Rad2Deg;
            // Rotate the image to face the center
            imageObject.transform.rotation = Quaternion.Euler(180, 90 - angleToCenter, 90);
        }
        cylinderParent.transform.localPosition = Vector3.zero;  // Set local position to zero
        cylinderParent.transform.localRotation = Quaternion.identity;  // Set local rotation to identity
        cylinderParent.transform.localScale = new Vector3(1, 1, 1);
    }*/
    void StartRotatingCylinders()
    {
        isRotating = true;
        _currentCylinder = 0;
    }


    public void StopCylinderAtIndex(int stopCylinderAtIndex)
    {
        if (stopCylinderAtIndex < numberOfCylinders && cylinderRotationStates[stopCylinderAtIndex])
        {
            var closest = float.MaxValue;
            var minDifference = float.MaxValue;
            float TargetRot = cylinderParents[stopCylinderAtIndex].localRotation.eulerAngles.y;
            foreach (var element in rotations)
            {
                var difference = Math.Abs((double)element - TargetRot);
                if (minDifference > difference)
                {
                    minDifference = (float)difference;
                    closest = element;
                }
            }
            cylinderRotationStates[stopCylinderAtIndex] = false;  // Stop the rotation for the current cylinder
            cylinderParents[stopCylinderAtIndex].localRotation = Quaternion.Euler(0, closest, 0);

            bool isEnd = true;
            for (int i = 0; i < numberOfCylinders; i++)
            {
                if (cylinderRotationStates[i] == true)
                {
                    isEnd = false;
                }
            }

            if (GameController.Instance.IsRevealed)
            {
                cylinderParents[stopCylinderAtIndex].localRotation = Quaternion.identity;
            }

            if (isEnd)
            {
                OnEndSpinning();
                //UIManager.INSTANCE.FollowMeCheck = true;
                //// Save the updated boolean value to PlayerPrefs
                //PlayerPrefs.SetInt("FollowMeCheck", UIManager.INSTANCE.FollowMeCheck ? 1 : 0); // Convert bool to int
                //PlayerPrefs.Save(); // Save changes
                //UIManager.INSTANCE.ToggleBool();

            }
        }
    }

    void StopNextCylinder()
    {
        int selectCylinder = _currentCylinder;
        while (selectCylinder < numberOfCylinders)
        {
            if (cylinderRotationStates[selectCylinder] == false)
            {
                selectCylinder++;
            }
            else
            {
                break;
            }
        }


        if (_currentCylinder < numberOfCylinders)
        {
            var closest = float.MaxValue;
            var minDifference = float.MaxValue;
            float TargetRot = cylinderParents[_currentCylinder].localRotation.eulerAngles.y;
            foreach (var element in rotations)
            {
                var difference = Math.Abs((double)element - TargetRot);
                if (minDifference > difference)
                {
                    minDifference = (float)difference;
                    closest = element;
                }
            }
            cylinderRotationStates[_currentCylinder] = false;  // Stop the rotation for the current cylinder
            cylinderParents[_currentCylinder].localRotation = Quaternion.Euler(0, closest, 0);

            if (GameController.Instance.IsRevealed)
            {
                cylinderParents[_currentCylinder].localRotation = Quaternion.identity;
            }
            _currentCylinder++;

            if (_currentCylinder >= numberOfCylinders)
            {
                OnEndSpinning();
                //UIManager.INSTANCE.FollowMeCheck = true;
                //// Save the updated boolean value to PlayerPrefs
                //PlayerPrefs.SetInt("FollowMeCheck", UIManager.INSTANCE.FollowMeCheck ? 1 : 0); // Convert bool to int
                //PlayerPrefs.Save(); // Save changes
                //UIManager.INSTANCE.ToggleBool();
            }
        }
    }

    private void OnStartSpinning()
    {
        if (!isRotating && !IsBonus1)
        {
            _stopReelOnInteraction.Activate();
            _ticTacToeInteraction.Deactivate();
        }

        if (IsBonus1)
        {
            _stopReelOnInteraction.Deactivate();
            _ticTacToeInteraction.Deactivate();
        }

        StartRotatingCylinders();
        StartCoroutine(StopCylindersSequentially()); // Start stopping the cylinders sequentially
    }

    private void OnEndSpinning()
    {
        isRotating = false;
        if (!isRotating && !IsBonus1)
        {
            _stopReelOnInteraction.Deactivate();
            _ticTacToeInteraction.Activate();
        }

        if (!isRotating && IsBonus1)
        {
            CheckWinningCondition();
        }

        _currentCylinder = 0;
        slotAudio.Stop();
        for (int i = 0; i < numberOfCylinders; i++)
        {
            cylinderRotationStates[i] = true;
            //AlignSymbolWithClosestCenter(i);
        }
        //_spinButton.gameObject.SetActive(false);

        //TODO:Winning Checked Here
        //if (CheckForWinningPatterns.INSTANCE.isBonus)
        //    CheckWinningCondition();
        _betConfirmed = false;
    }

    IEnumerator StopCylindersSequentially()
    {
        for (int i = 0; i < numberOfCylinders; i++)
        {
            if (won)
            {
                won = false;
                yield break;
            }
            // Stop the rotation for the current cylinder
            //cylinderRotationStates[i] = false;

            // Delay for a short duration before stopping the next cylinder
            yield return new WaitForSeconds(delay); // Adjust the duration as needed

            // Check for collisions with centerpoint
            if (_currentCylinder < numberOfCylinders)
            {
                var closest = float.MaxValue;
                var minDifference = float.MaxValue;
                float TargetRot = cylinderParents[_currentCylinder].localRotation.eulerAngles.y;
                foreach (var element in rotations)
                {
                    var difference = Math.Abs((double)element - TargetRot);
                    if (minDifference > difference)
                    {
                        minDifference = (float)difference;
                        closest = element;
                    }
                }
                cylinderRotationStates[_currentCylinder] = false;  // Stop the rotation for the current cylinder
                cylinderParents[_currentCylinder].localRotation = Quaternion.Euler(0, closest, 0);

                if (GameController.Instance.IsRevealed)
                {
                    cylinderParents[_currentCylinder].localRotation = Quaternion.identity;
                }
                _currentCylinder++;

                if (_currentCylinder == numberOfCylinders)
                {
                    OnEndSpinning();
                    //UIManager.INSTANCE.ToggleBool();

                }
            }
        }
    }
    void ChangeRotationSpeed(float newSpeed)
    {
        delay = newSpeed;
        // Update the speed text
        speedText.text = $"Delay: {delay}";
    }

    void UpdateCoinsText()
    {
        // Update the coins text
        //coinsText.text = $"Coins: {coins}";
    }
    void ToggleMute()
    {
        isMuted = !isMuted;  // Toggle the mute state

        // Adjust the volume based on the mute state
        AudioListener.volume = isMuted ? 0.1f : 1.0f;

        // Update the button text
        muteButton.GetComponentInChildren<Text>().text = isMuted ? "Unmute" : "Mute";
    }
    public void ChangeScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
    public void ShowPopup(string message)
    {
        //  popupText.text = message;
        popupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }

    public void RefreshImages()
    {
        if (isRotating || CylinderSpawning)
            return;

        refreshBtn.interactable = false;
        print("Spin OFF Refresh Image");
        UIManager.INSTANCE._spinButton.interactable = false;
        //CheckForWinningPatterns.INSTANCE._reveal_button.interactable = false;
        //CylinderSpawning = true;
        StartCoroutine(RefreshImagesFX());
    }

    public void RefreshImagesAgain()
    {
        PlayerPrefs.SetFloat("_spinning", 1);
        if (isRotating || CylinderSpawning)
            return;

        CylinderSpawning = true;
        refreshBtn.interactable = true;
        StartCoroutine(RefreshImagesFX());
    }

    private IEnumerator RefreshImagesFX(bool p_is_free_spin = false)
    {
        if (isRotating)
            yield break;

        foreach (var item in GameController.Instance._highlight_objects)
        {
            Destroy(item);
        }

        CylinderSpawning = true;

        List<List<int>> t_on_screen_elements = new();

        foreach (RaycastOriginTransforms t_set in CheckForWinningPatterns.INSTANCE.raycastPatterns)
        {
            if (string.Equals(t_set.listName, "First Column") || string.Equals(t_set.listName, "Second Column") || string.Equals(t_set.listName, "Third Column"))
            {
                List<int> t_detected_sprite_column = new();
                foreach (Transform t_raycast_transform in t_set.raycastOrigins)
                {
                    RaycastHit2D t_detected_sprite = Physics2D.Raycast(t_raycast_transform.position, Vector3.forward);
                    t_detected_sprite_column.Add(t_detected_sprite.transform.GetSiblingIndex());
                }
                t_on_screen_elements.Add(t_detected_sprite_column);
            }
        }
        t_on_screen_elements.Reverse();             // Because previous developer has reversed the order slots for raycast origins
        int t_index = 0;

        //destroy previous images fx
        foreach (var parent in cylinderParents)
        {
            foreach (Transform child in parent)
            {
                if (!child.gameObject.activeInHierarchy)
                {
                    child.gameObject.SetActive(true);
                }

                if (t_on_screen_elements[t_index].Contains(child.GetSiblingIndex()))
                //if (child.GetSiblingIndex() == 9 || child.GetSiblingIndex() == 10 || child.GetSiblingIndex() == 11)       //  remove this if the line above works
                {
                    child.gameObject.AddComponent<Rigidbody2D>();
                }

                Destroy(child.gameObject, 2f);
            }
            Destroy(parent.gameObject, 2f);
            ++t_index;
        }
        yield return new WaitForSeconds(2);
        //create new images fx
        // Spawn new image symbols inside allCylindersParent
        Vector3 spawnPosition = new Vector3(0, distanceBetweenCylinders, 0);
        for (int i = 0; i < numberOfCylinders; i++)
        {
            SpawnImagesOnCylinder(spawnPosition, i, allCylindersParent.transform);
            spawnPosition += new Vector3(0f, distanceBetweenCylinders, 0f);
        }
        CylinderSpawning = false;
        UIManager.INSTANCE._spinButton.interactable = true;
        //CheckForWinningPatterns.INSTANCE._reveal_button.interactable = true;
    }

    public void CheckWinningCondition()
    {
        if (CheckForWinningPatterns.INSTANCE != null)
        {
            checkedForPatterns = true;
            CheckForWinningPatterns.INSTANCE.CheckPatterns();
        }
    }

    public void CheckWinningCondition(RaycastOriginTransforms raycastTransform)
    {
        if (CheckForWinningPatterns.INSTANCE != null)
        {
            checkedForPatterns = true;
            CheckForWinningPatterns.INSTANCE.CheckPatterns(raycastTransform);
        }
    }

    public void StartBonusSpin()
    {
        IsBonus1 = true;
        UIManager.INSTANCE._spinButton.interactable = false;
        print("Spin OFF Start Bonus Spin");
        _ticTacToeInteraction.timeBar.transform.parent.gameObject.SetActive(false);
        //UIManager.INSTANCE._spinButton.interactable = false;
        _stopReelOnInteraction.gameObject.SetActive(false);
        _ticTacToeInteraction.gameObject.SetActive(false);
        
        if (bonusSpinAudio != null)
        {
            bonusSpinAudio.loop = true;
            bonusSpinAudio.Play();
        }
        
        StartCoroutine(FifteenBonusSpins());
    }

    public IEnumerator FifteenBonusSpins()
    {
        refreshBtn.interactable = false;
        bonusSpins = true;

        yield return StartCoroutine(RefreshImagesFX(true));

        StartRotatingCylinders();

        yield return StartCoroutine(StopCylindersSequentially());

        if (bonusSpinAudio != null && bonusSpinAudio.isPlaying)
        {
            bonusSpinAudio.Stop(); // Stop the sound
        }

        yield return null;
        bonusSpins = false;
    }
}



