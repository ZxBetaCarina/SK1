using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimator : MonoBehaviour
{
    public GameObject childcard;
    public bool isWiggling=false;
    
   

    public void StartWiggling(float time)
    {
        if (!isWiggling)
        {
            isWiggling = true;
            Debug.Log("called");

            // Start the wiggling animation with a unique tween ID
          int tweenId = LeanTween.rotateZ(gameObject, 10f, 0.5f)
                .setLoopPingPong()
                .setEase(LeanTweenType.easeInOutSine)
                .id;

            // Stop the animation after the given time
            LeanTween.delayedCall(gameObject, time, () => StopWiggling(tweenId));
        }
    }

    public void StopWiggling(int tweenId)
    {
        if (isWiggling)
        {
            isWiggling = false;
        
            // Cancel using the unique ID
            LeanTween.cancel(gameObject, tweenId);
        
            // Reset rotation smoothly using LeanTween
            LeanTween.rotateZ(gameObject, 0f, 0.3f).setEase(LeanTweenType.easeInOutQuad);
        }
    }
    public void RevealAndHide( float delay)
    {
        childcard.SetActive(true);

        Invoke(nameof(HideCard), delay); // Hide after delay
    }

    void HideCard()
    {
        childcard.SetActive(false);
    }
 
    

}
