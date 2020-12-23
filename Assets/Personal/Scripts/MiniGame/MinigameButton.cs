using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MinigameButton : MonoBehaviour
{
    [SerializeField] GrammaticaMiniGame grammaticaMiniGame;

    public enum ButtonType { question, answer }
    public enum Tijden { presentPerfect, presentSimple, presentContinuous, pastSimple, pastContinuous }

    [SerializeField] public ButtonType buttonType;
    [SerializeField] public Tijden tijd;

    public void OnPressed()
    {
        grammaticaMiniGame.ButtonPressed(this);
    }
}
