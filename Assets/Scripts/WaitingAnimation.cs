using System.Collections;
using TMPro;
using UnityEngine;

public class WaitingAnimation : MonoBehaviour
{

    TMP_Text waitingText;
    string text;

    void Start()
    {
        waitingText = GetComponent<TMP_Text>();
        text = waitingText.text;
        StartCoroutine(AnimateDots(1));
    }

    IEnumerator AnimateDots(int dots)
    {

        yield return new WaitForSeconds(0.6f);

        waitingText.text = text + new string('.', dots);
        StartCoroutine(AnimateDots(dots < 3 ? dots + 1 : 0));
    }

}
