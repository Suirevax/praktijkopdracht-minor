using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] MiniGameManager miniGameManager = null;

    [SerializeField] Target target = null;
    [SerializeField] GameObject minigamePanel = null;

    [SerializeField] float targetSpawnXOffset = 20;
    [SerializeField] float targetSpawnYOffset = 20;


    int score = 0;
    int goalScore = 3;

    private void OnEnable()
    {
        InvokeRepeating("SpawnTarget", 1, 3);
    }

    private void OnDisable()
    {
        CancelInvoke();
        for(int i = 0; i < minigamePanel.transform.childCount; i++)
        {
            Destroy(minigamePanel.transform.GetChild(i).gameObject);
        }
        score = 0;
    }


    void SpawnTarget()
    {
        var tmp = Instantiate(target, minigamePanel.transform);
        tmp.transform.position += new Vector3(
            minigamePanel.GetComponent<RectTransform>().rect.xMin / 2 - targetSpawnXOffset,
            Random.Range(minigamePanel.GetComponent<RectTransform>().rect.yMin / 2 + targetSpawnYOffset, minigamePanel.GetComponent<RectTransform>().rect.yMax / 2 - targetSpawnYOffset),
            0);

        tmp.GetComponent<Target>().targetSpawner = gameObject;
    }

    public void IncreaseScore(float value)
    {
        score += (int)value;
        if(score >= goalScore)
        {
            miniGameManager.Win();
        }
    }


}
