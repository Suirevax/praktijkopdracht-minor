using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MultipleChoiceMiniGame : MonoBehaviour
{
    [SerializeField] MiniGameManager miniGameManager = null;

    [SerializeField] TMP_Text QuestionText = null;
    [SerializeField] List<Button> buttons = null;

    string correctAnswer = null;

    public void ButtonPressed(MultipleChoiceMiniGameButton button)
    {
        button.GetComponent<Button>().interactable = false;
        string answer = button.transform.GetChild(0).GetComponent<TMP_Text>().text;
        if(answer == correctAnswer)
        {
            miniGameManager.Win();
        }
    }

    private void OnEnable()
    {
        Setup();
    }

    void Setup()
    {
        var questionData = miniGameManager.GetMultipleChoiceQuestion();

        QuestionText.text = questionData[0];
        correctAnswer = questionData[1];

        List<string> tmpAnswers = new List<string> { questionData[1], questionData[2], questionData[3] };

        foreach (var button in buttons )
        {
            var tmpAnswerIndex = Random.Range(0, tmpAnswers.Count);
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = tmpAnswers[tmpAnswerIndex];
            tmpAnswers.RemoveAt(tmpAnswerIndex);

            button.interactable = true;
        }
    }
}
