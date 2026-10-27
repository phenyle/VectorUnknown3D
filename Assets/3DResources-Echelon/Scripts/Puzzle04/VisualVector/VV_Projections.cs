using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VV_Projections : VisVectController
{
    public Color vector1clr;
    public Color vector2clr;
    public Color vectoAclr;
    public float projectionWidth = 0.2f;


    [Header("----- Vector1 Projections --------")]
    public GameObject vector1projections;
    public LineRenderer xy1proj;
    public LineRenderer yz1proj;
    public LineRenderer xz1proj;

    [Header("----- Vector2 Projections --------")]
    public GameObject vector2projections;
    public LineRenderer xy2proj;
    public LineRenderer yz2proj;
    public LineRenderer xz2proj;

    [Header("----- Goal Projections --------")]
    public GameObject vectorAprojections;
    public LineRenderer xyAproj;
    public LineRenderer yzAproj;
    public LineRenderer xzAproj;

    public void Start()
    {
        projections = this;
        Init();
    }

    public void Init()
    {
        LineProperties(xy1proj, vector1clr, projectionWidth);
        LineProperties(yz1proj, vector1clr, projectionWidth);
        LineProperties(xz1proj, vector1clr, projectionWidth);

        LineProperties(xy2proj, vector2clr, projectionWidth);
        LineProperties(yz2proj, vector2clr, projectionWidth);
        LineProperties(xz2proj, vector2clr, projectionWidth);

        LineProperties(xyAproj, vectoAclr, projectionWidth);
        LineProperties(yzAproj, vectoAclr, projectionWidth);
        LineProperties(xzAproj, vectoAclr, projectionWidth);


        UpdateVector1(Vector3.zero, Vector3.zero);
        UpdateVector2(Vector3.zero, Vector3.zero, Vector3.zero);
        UpdateAnswer(Vector3.zero, Vector3.zero);
        ToggleProjections(false);
    }

    public void UpdateVector1(Vector3 start, Vector3 end)
    {
        xy1proj.SetPosition(0, start);
        xy1proj.SetPosition(1, new Vector3(end.x, end.y, start.z));

        yz1proj.SetPosition(0, start);
        yz1proj.SetPosition(1, new Vector3(start.x, end.y, end.z));

        xz1proj.SetPosition(0, start);
        xz1proj.SetPosition(1, new Vector3(end.x, start.y, end.z));
        if(start != end)
        {
            xy1proj.enabled = true;
            yz1proj.enabled = true;
            xz1proj.enabled = true;
        }
        else
        {
            xy1proj.enabled = false;
            yz1proj.enabled = false;
            xz1proj.enabled = false;
        }
    }
    public void UpdateVector2(Vector3 origin, Vector3 start, Vector3 end)
    {
        xy2proj.SetPosition(0, new Vector3(start.x, start.y, origin.z));
        xy2proj.SetPosition(1, new Vector3(end.x, end.y, origin.z));

        yz2proj.SetPosition(0, new Vector3(origin.x, start.y, start.z));
        yz2proj.SetPosition(1, new Vector3(origin.x, end.y, end.z));

        xz2proj.SetPosition(0, new Vector3(start.x, origin.y, start.z));
        xz2proj.SetPosition(1, new Vector3(end.x, origin.y, end.z));
        if (start != end)
        {
            xy2proj.enabled = true;
            yz2proj.enabled = true;
            xz2proj.enabled = true;
        }
        else
        {
            xy2proj.enabled = false;
            yz2proj.enabled = false;
            xz2proj.enabled = false;
        }
    }
    public void UpdateAnswer(Vector3 start, Vector3 end)
    {
        xyAproj.SetPosition(0, start);
        xyAproj.SetPosition(1, new Vector3(end.x, end.y, start.z));

        yzAproj.SetPosition(0, start);
        yzAproj.SetPosition(1, new Vector3(start.x, end.y, end.z));

        xzAproj.SetPosition(0, start);
        xzAproj.SetPosition(1, new Vector3(end.x, start.y, end.z));
        if (start != end)
        {
            xyAproj.enabled = true;
            yzAproj.enabled = true;
            xzAproj.enabled = true;
        }
        else
        {
            xyAproj.enabled = false;
            yzAproj.enabled = false;
            xzAproj.enabled = false;
        }

    }

    public void ToggleXYprojections(bool val)
    {
        xy1proj.enabled = val;
        xy2proj.enabled = val;
        xyAproj.enabled = val;
    }
    public void ToggleYZprojections(bool val)
    {
        yz1proj.enabled = val;
        yz2proj.enabled = val;
        yzAproj.enabled = val;
    }
    public void ToggleXZprojections(bool val)
    {
        xz1proj.enabled = val;
        xz2proj.enabled = val;
        xzAproj.enabled = val;
    }
    public void ToggleProjections(bool val)
    {        
        vector1projections.SetActive(val);
        vector2projections.SetActive(val);
        vectorAprojections.SetActive(val);
    }

}
