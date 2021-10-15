using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class TipsPoint : MonoBehaviour
{
    public Transform point1, point2, point3;
    public GameObject txtMesh1, txtMesh2, txtMesh3;
    public LineRenderer line1, line2, line3;

    private void Update()
    {
        line1.SetPosition(0, point1.localPosition);
        line2.SetPosition(1, point2.localPosition);
        line3.SetPosition(2, point3.localPosition);

        txtMesh1.GetComponent<TextMeshProUGUI>().text = point1.localPosition.x + "\n" +
                                  point1.localPosition.z + "\n" +
                                  point1.localPosition.y;

        txtMesh2.GetComponent<TextMeshProUGUI>().text = point2.localPosition.x.ToString("F0") + "\n" +
                                  point2.localPosition.z.ToString("F0") + "\n" +
                                  point2.localPosition.y.ToString("F0");

        txtMesh3.GetComponent<TextMeshProUGUI>().text = point3.localPosition.x.ToString("F0") + "\n" +
                                  point3.localPosition.z.ToString("F0") + "\n" +
                                  point3.localPosition.y.ToString("F0");
    }
}
