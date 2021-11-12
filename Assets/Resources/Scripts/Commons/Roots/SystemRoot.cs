using UnityEngine;

public class SystemRoot : MonoBehaviour
{
    protected ResourceService resourceService;
    protected AudioService audioService;
    //public Transform uiWindows;

    public virtual void InitSystem()
    {
        resourceService = GameRoot.instance.resourceService;
        audioService = GameRoot.instance.audioService;
    }

    protected void CloseAllWindow()
    {
        Transform canvas = transform.Find("Canvas");
        Transform uiWindows = canvas.Find("UIWindows");

        for (int i = 0; i < uiWindows.childCount; i++)
        {
            uiWindows.GetChild(i).gameObject.SetActive(false);
        }

        uiWindows.Find("DynamicWindow").GetComponent<DynamicWindow>().SetWindowState(true);
    }
}
