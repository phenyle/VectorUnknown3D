using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VV_Anchors : VisVectController
{
    public Color vecto1clr;
    public Color vecto2clr;
    public Color vectoGclr;
    public float anchorWidth = 0.1f;

    [Header("----- Vector1 Anchors --------")]
    public GameObject vector1Anchors;
    public LineRenderer xy1Anchor;
    public LineRenderer yz1Anchor;
    public LineRenderer xz1Anchor;

    [Header("----- Vector2 Anchors --------")]
    public GameObject vector2Anchors;
    public LineRenderer xy2Anchor;
    public LineRenderer yz2Anchor;
    public LineRenderer xz2Anchor;

    [Header("----- Goal Anchors --------")]
    public GameObject vectorGAnchors;
    public LineRenderer xyGAnchor;
    public LineRenderer yzGAnchor;
    public LineRenderer xzGAnchor;

    public void Start()
    {
        anchors = this;
        Init();
    }


    public void Init()
    {
        LineProperties(xy1Anchor, vecto1clr, anchorWidth);
        LineProperties(yz1Anchor, vecto1clr, anchorWidth);
        LineProperties(xz1Anchor, vecto1clr, anchorWidth);

        LineProperties(xy2Anchor, vecto2clr, anchorWidth);
        LineProperties(yz2Anchor, vecto2clr, anchorWidth);
        LineProperties(xz2Anchor, vecto2clr, anchorWidth);

        LineProperties(xyGAnchor, vectoGclr, anchorWidth);
        LineProperties(yzGAnchor, vectoGclr, anchorWidth);
        LineProperties(xzGAnchor, vectoGclr, anchorWidth);


        ToggleAnchors(false);
    }

    public void UpdateVector1(Vector3 start, Vector3 end)
    {
        xy1Anchor.SetPosition(0, new Vector3(end.x, end.y, start.z));
        xy1Anchor.SetPosition(1, end);

        yz1Anchor.SetPosition(0, new Vector3(start.x, end.y, end.z));
        yz1Anchor.SetPosition(1, end);

        xz1Anchor.SetPosition(0, new Vector3(end.x, start.y, end.z));
        xz1Anchor.SetPosition(1, end);

        if (start != end)
        {
            xy1Anchor.enabled = true;
            yz1Anchor.enabled = true;
            xz1Anchor.enabled = true;
        }
        else
        {
            xy1Anchor.enabled = false;
            yz1Anchor.enabled = false;
            xz1Anchor.enabled = false;
        }
    }
    public void UpdateVector2(Vector3 start, Vector3 end)
    {
        xy2Anchor.SetPosition(0, new Vector3(end.x, end.y, start.z));
        xy2Anchor.SetPosition(1, end);

        yz2Anchor.SetPosition(0, new Vector3(start.x, end.y, end.z));
        yz2Anchor.SetPosition(1, end);

        xz2Anchor.SetPosition(0, new Vector3(end.x, start.y, end.z));
        xz2Anchor.SetPosition(1, end);

        if (start != end)
        {
            xy2Anchor.enabled = true;
            yz2Anchor.enabled = true;
            xz2Anchor.enabled = true;
        }
        else
        {
            xy2Anchor.enabled = false;
            yz2Anchor.enabled = false;
            xz2Anchor.enabled = false;
        }
    }
    public void UpdateGoal(Vector3 start, Vector3 end)
    {
        xyGAnchor.SetPosition(0, new Vector3(end.x, end.y, start.z));
        xyGAnchor.SetPosition(1, end);

        yzGAnchor.SetPosition(0, new Vector3(start.x, end.y, end.z));
        yzGAnchor.SetPosition(1, end);

        xzGAnchor.SetPosition(0, new Vector3(end.x, start.y, end.z));
        xzGAnchor.SetPosition(1, end);
    }

    public void ToggleXYanchors(bool val)
    {
        xy1Anchor.enabled = val;
        xy2Anchor.enabled = val;
        xyGAnchor.enabled = val;
    }
    public void ToggleYZanchors(bool val)
    {
        yz1Anchor.enabled = val;
        yz2Anchor.enabled = val;
        yzGAnchor.enabled = val;
    }
    public void ToggleXZanchors(bool val)
    {
        xz1Anchor.enabled = val;
        xz2Anchor.enabled = val;
        xzGAnchor.enabled = val;
    }

    public void ToggleAnchors(bool val)
    {
        vector1Anchors.SetActive(val);
        vector2Anchors.SetActive(val);
        vectorGAnchors.SetActive(val);
    }

}
