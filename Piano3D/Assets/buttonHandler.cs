using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class buttonHandler : MonoBehaviour {
    public Button Button;
    public Image[] Images;
    public int i;
    public List<List<int>> current_ids;
    public bool isStarted;
    public string gameDataFileName = "melody.json";
    public List<Color> colorList;
    public JsonData jsonString;
    public int id_song;

    // Use this for initialization
    void Start () {
        reset();
        isStarted = true;
        // repeat each 1 second
        InvokeRepeating("visibleKeys", 0.1f, 0.1f);
    }

    public void reset()
    {
        // current key
        current_ids = new List<List<int>>();
        // list of colors
        colorList = new List<Color>() { Color.red, Color.yellow, Color.green, Color.blue, Color.magenta};
        // FIXME: get id_song from Menu, default is 0
        id_song = 0;
        // read musical script
        readMusicalScript();
        // Ticks
        i = 0;
        foreach (var image in this.Images)
        {
            var imageComponent = image.GetComponent<Image>();
            imageComponent.gameObject.SetActive(false);
        }
    }

    public void readMusicalScript()
    {
        // Read Json files
        // Path.Combine combines strings into a file path
        string filePath = Path.Combine(Application.dataPath, gameDataFileName);

        try {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            jsonString = JsonMapper.ToObject(dataAsJson);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error: " + ex);
        }
    }

    public List<List<int>> activateKey(int time)
    {
        // musical script -> which key is activated based on time (ticks)
        int n_time = jsonString["songs"][id_song]["duration"].Count;
        List<List<int>> id_keys = new List<List<int>>();
        for (int i = 0; i < n_time; i++)
        {
            int the_time;
            Int32.TryParse(jsonString["songs"][id_song]["duration"][i][0].ToString(), out the_time);
            int id_key;
            Int32.TryParse(jsonString["songs"][id_song]["duration"][i][1].ToString(), out id_key);
            int id_color;
            Int32.TryParse(jsonString["songs"][id_song]["duration"][i][2].ToString(), out id_color);
            if (time == the_time)
            {
                id_keys.Add(new List<int>() { id_key, id_color});
            }
        }
        if (id_keys.Count > 0)
            return id_keys;
        return current_ids;
    }

    public void visibleKeys()
    {
        try
        {
            if (isStarted)
            {
                i++;
                current_ids = activateKey(i);
                foreach (var image in this.Images)
                {
                    var imageComponent = image.GetComponent<Image>();
                    foreach (List<int> current_id in current_ids)
                    {
                        if (imageComponent.name.Equals("Image" + current_id[0]))
                        {
                            imageComponent.gameObject.SetActive(true);
                            imageComponent.color = colorList[current_id[1] - 1];
                            break;
                        }
                        else
                        {
                            imageComponent.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Timer: " + i);
            Debug.Log(ex);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
