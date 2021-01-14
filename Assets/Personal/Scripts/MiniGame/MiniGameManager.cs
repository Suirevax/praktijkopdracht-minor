using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.IO;

public class MiniGameManager : MonoBehaviour
{
    //[SerializeField] RoundManager roundManager = null;

    [SerializeField] GameObject TargetMiniGame = null;
    [SerializeField] GameObject ConnectMiniGame = null;
    [SerializeField] GameObject InputMiniGame = null;
    [SerializeField] GameObject multipleChoiceMiniGame = null;

    public enum state { none, targetPractice, ConnectMiniGame, InputMiniGame, MultipleChoiceMiniGame };
    state currentState = state.none;

    //MultiplechoiceImport
    List<string[]> MultipleChoiceQuestions = new List<string[]>();
    List<string[]> InputQuestions = new List<string[]>();

    private void Awake()
    {
        TargetMiniGame.SetActive(false);
    }

    private void Start()
    {
        TextAsset testMiniGameData = Resources.Load<TextAsset>("MultipleChoice");

        string[] data = testMiniGameData.text.Split('\n');
        
        foreach(var line in data)
        {
            if(line.Length < 3) continue;
            var values = line.Split(';');
            MultipleChoiceQuestions.Add(values);
        }
        MultipleChoiceQuestions.RemoveAt(0);

        TextAsset InputQuestionsText = Resources.Load<TextAsset>("InputQuestion");

        string[] data2 = InputQuestionsText.text.Split('\n');

        foreach (var line in data2)
        {
            if (line.Length < 3) continue;
            var values = line.Split(';');
            InputQuestions.Add(values);
        }
        InputQuestions.RemoveAt(0);
    }

    public void SetCurrentState(state newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case state.none:
                ConnectMiniGame.SetActive(false);
                TargetMiniGame.SetActive(false);
                multipleChoiceMiniGame.SetActive(false);
                InputMiniGame.SetActive(false);
                break;
            case state.targetPractice:
                TargetMiniGame.SetActive(true);
                break;
            case state.ConnectMiniGame:
                ConnectMiniGame.SetActive(true);
                break;
            case state.InputMiniGame:
                InputMiniGame.SetActive(true);
                break;
            case state.MultipleChoiceMiniGame:
                multipleChoiceMiniGame.SetActive(true);
                break;
        }
    }

    public void Win()
    {
        PlayerController localplayer = NetworkClient.connection.identity.GetComponent<PlayerController>();
        SetCurrentState(MiniGameManager.state.none);
        //roundManager.MiniGameWon();
        localplayer.CmdMiniGameWon();
    }

    public string[] GetMultipleChoiceQuestion()
    {
        return MultipleChoiceQuestions[Random.Range(0, MultipleChoiceQuestions.Count)];
    }
    
    public string[] GetInputQuestion()
    {
        return InputQuestions[Random.Range(0, InputQuestions.Count)];
    }
}
