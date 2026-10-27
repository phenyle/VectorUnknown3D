using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class WindRandom : MonoBehaviour
{
    public MinMaxFloat Speed = new MinMaxFloat(0.5f,3.0f);

    public void OnEnable()
    {
        float value = Random.Range(Speed.Min, Speed.Max);
        gameObject.GetComponent<Animator>().speed = value;
    }

    public void RandomSpeed()
    {
        float value = Random.Range(Speed.Min, Speed.Max);
        gameObject.GetComponent<Animator>().speed = value;
    }

}
