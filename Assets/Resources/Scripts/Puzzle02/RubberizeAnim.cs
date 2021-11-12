using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RubberizeAnim : MonoBehaviour
{
    [Range(0, 2.0f)]
    public float maxIncrease = 0.5f;
    [Range(0, 0.2f)]
    public float speed = 0.02f;

    public bool active = false;
    public bool flag = true;

    private Vector3 maxSize;
    private Vector3 minSize;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(active)
        {
            if(flag)
            {

                this.transform.localScale *= 1 + speed;
                if (this.transform.localScale.magnitude > maxSize.magnitude)
                    flag = false;
            }
            else
            {
                this.transform.localScale *= 1 - speed;
                if (this.transform.localScale.magnitude < minSize.magnitude)
                    flag = true;
            }
        }
    }

    public void ToggleActivate(bool val)
    {
        active = val;
        if(val)
        {
            minSize = this.transform.localScale;
            maxSize = this.transform.localScale * ( 1 + maxIncrease);
        }
        else
        {
            this.transform.localScale = Vector3.one;
        }
    }
}
