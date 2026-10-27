using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

[ExecuteInEditMode]
public class StarBar : MonoBehaviour
{
    [ReadOnly]
    public float score = 0.0f;
    [SerializeField] private Slider m_Slider;

    [SerializeField] private GameObject m_RecordFill;

    [SerializeField] private HorizontalLayoutGroup horiStarFill;
    [SerializeField] private HorizontalLayoutGroup horiRecordFill;

    [SerializeField] private LayoutElement[] layoutStarFill;
    [SerializeField] private LayoutElement[] layoutRecordFill;


    // Start is called before the first frame update
    void Start()
    {
        ResetLayout();
     //   m_Slider.value = 0;
        m_RecordFill.SetActive(false);
    }

    public void SetScore(float val)
    {
        score = val;
        gameObject.SetActive(true);
        //if(val < 0)
        //    val = 0;
        //if(val > 1)
        //    val = 1;
        m_Slider.value = val;
    }

    public void SetScore(float value, float maxValue)
    {
        score = value / maxValue;
        gameObject.SetActive(true);
        m_Slider.value = score;
    }

    public void SetScore(float val, bool record)  {
        score = val;
        gameObject.SetActive(true);
        //if (val < 0)
        //    val = 0;
        //if (val > 1)
        //    val = 1;
        m_Slider.value = val;
        m_RecordFill.SetActive(record);
    }

    public Vector2 GetSize()
    {
        return transform.GetComponentInParent<RectTransform>().sizeDelta;
    }

    public void SetSize(float width, float height)
    {
        transform.GetComponentInParent<RectTransform>().sizeDelta = new Vector2(width, height);
        ResetLayout();
    }

    public void SetSize(Vector2 size)
    {
        transform.GetComponentInParent<RectTransform>().sizeDelta = size;
        ResetLayout();
    }

    private void ResetLayout()
    {
        LayoutElement temp;
        foreach (Transform child in horiStarFill.transform)
        {
            if (!child.TryGetComponent(out temp))
                child.gameObject.AddComponent<LayoutElement>();

            child.GetComponent<LayoutElement>().minWidth = transform.GetComponent<RectTransform>().rect.width / horiStarFill.transform.childCount;
            child.GetComponent<LayoutElement>().minHeight = transform.GetComponent<RectTransform>().rect.height;
        }
        foreach (Transform child in horiRecordFill.transform)
        {
            if (!child.TryGetComponent(out temp))
                child.gameObject.AddComponent<LayoutElement>();

            child.GetComponent<LayoutElement>().minWidth = transform.GetComponent<RectTransform>().rect.width / horiRecordFill.transform.childCount;
            child.GetComponent<LayoutElement>().minHeight = transform.GetComponent<RectTransform>().rect.height;
        }
    }
}
