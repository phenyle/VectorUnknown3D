using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class menu_button_functions : MonoBehaviour
{
    public TextMeshProUGUI Title_Banner;

    public void exit()
    {
        Destroy(GameObject.Find("LevelData"));
        SceneManager.LoadScene("000Login");
    }

    public void set_title_banner( string title)
    {
        Title_Banner.text = title;
    }
}
