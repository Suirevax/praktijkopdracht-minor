using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5;
    [SerializeField] float scoreValue = 1;

    public GameObject targetSpawner;

    void Update()
    {
        gameObject.transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

        if(GetComponent<RectTransform>().transform.position.x > 1000)
        {
            Destroy(gameObject);
        }
    }

    public void Hit()
    {
        GetComponent<Image>().color = Color.red;
        targetSpawner.GetComponent<TargetSpawner>().IncreaseScore(scoreValue);
    }

    public float ScoreValue => scoreValue;
}
