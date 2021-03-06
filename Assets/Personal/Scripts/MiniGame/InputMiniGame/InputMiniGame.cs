﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputMiniGame : MonoBehaviour
{
    [SerializeField] MiniGameManager miniGameManager = null;

    [SerializeField] TMP_Text questionText = null;
    [SerializeField] TMP_InputField inputField = null;

    string correctAnswer = null;

    public void CheckAnswer()
    {
        Debug.Log("Checking Answer");
        string answer = inputField.text;

        Debug.Log(correctAnswer);
        Debug.Log(answer);

        Debug.Log(correctAnswer.Length);
        Debug.Log(answer.Length);

        if (answer.Trim() == correctAnswer)
        {
            Debug.Log("correct answer");
            miniGameManager.Win();
        }
    }

    private void OnEnable()
    {
        Setup();
    }

    private void OnDisable()
    {
            transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = string.Empty;
    }

    void Setup()
    {
        var questionData = miniGameManager.GetInputQuestion();

        questionText.text = questionData[0];
        correctAnswer = questionData[1];
        correctAnswer = correctAnswer.Trim();
    }
}
