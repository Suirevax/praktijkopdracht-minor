using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GrammaticaMiniGame : MonoBehaviour
{
    [SerializeField] MiniGameManager miniGameManager = null;

    int score = 0;
    int maxScore = 5;

    MinigameButton selectedButton = null;

    public void ButtonPressed(MinigameButton pressedButton)
    {
        if(selectedButton == null)
        {
            selectedButton = pressedButton;
            pressedButton.GetComponent<Button>().interactable = false;
            return;
        }

        switch (selectedButton.buttonType)
        {
            case MinigameButton.ButtonType.answer:
                if (pressedButton.buttonType.Equals(MinigameButton.ButtonType.answer))
                {
                    ChangeSelectedButton(pressedButton);
                }
                else if (pressedButton.buttonType.Equals(MinigameButton.ButtonType.question))
                {
                    if (pressedButton.tijd == selectedButton.tijd)
                    {
                        //correct combination
                        CorrectCombination(pressedButton);
                    }
                    else
                    {
                        //incorrect combination
                        InCorrectCombination(pressedButton);
                    }
                }
                break;

            case MinigameButton.ButtonType.question:
                if (pressedButton.buttonType.Equals(MinigameButton.ButtonType.answer))
                {
                    if (pressedButton.tijd == selectedButton.tijd)
                    {
                        //correct combination
                        CorrectCombination(pressedButton);
                    }
                    else
                    {
                        //incorrect combination
                        InCorrectCombination(pressedButton);
                    }
                }
                else if (pressedButton.buttonType.Equals(MinigameButton.ButtonType.question))
                {
                    ChangeSelectedButton(pressedButton);
                }
                break;
        }
    }

    void CorrectCombination(MinigameButton pressedButton)
    {
        pressedButton.GetComponent<Button>().interactable = false;
        pressedButton.GetComponent<Image>().color = Color.green;
        selectedButton.GetComponent<Button>().interactable = false;
        selectedButton.GetComponent<Image>().color = Color.green;
        selectedButton = null;

        IncreaseScore(1);
    }

    void InCorrectCombination(MinigameButton pressedButton)
    {
        selectedButton.GetComponent<Button>().interactable = true;
        selectedButton = null;
    }

    void ChangeSelectedButton(MinigameButton pressedButton)
    {
        selectedButton.GetComponent<Button>().interactable = true;
        selectedButton = pressedButton;
        pressedButton.GetComponent<Button>().interactable = false;
    }

    void IncreaseScore(int value)
    {
        score += value;
        if(score >= maxScore)
        {
            miniGameManager.Win();
            Reset();
        }
    }

    private void Reset()
    {
        score = 0;
        foreach(var button in gameObject.GetComponentsInChildren<Button>(true))
        {
            button.interactable = true;
            button.GetComponent<Image>().color = Color.white;
        }
    }
}
