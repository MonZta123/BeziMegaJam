using System.Collections;
using TMPro;
using UnityEngine;

public class ChalkBoardTextManager : MonoBehaviour
{
    [SerializeField]
    private string[] ComplaintMessages;
    [SerializeField]
    private TextMeshPro displayText;
    private void Start()
    { 
        StartCoroutine(ComplaintTimer(5f));
    }

    private IEnumerator ComplaintTimer(float delay)
    {
        while (true)
        {
            CycleMessages();
            yield return new WaitForSeconds(delay);
        }
    }

    private void CycleMessages()
    {
        if (displayText != null || ComplaintMessages.Length > 0)
        {
            displayText.text = ComplaintMessages[Random.Range(0, ComplaintMessages.Length)];
            return;
        }
    }
}
