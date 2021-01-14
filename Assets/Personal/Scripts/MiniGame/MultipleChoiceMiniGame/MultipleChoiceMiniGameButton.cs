using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleChoiceMiniGameButton : MonoBehaviour
{
    [SerializeField] MultipleChoiceMiniGame multipleChoiceMiniGame = null;


    public void OnPressed()
    {
        multipleChoiceMiniGame.ButtonPressed(this);
    }
}
