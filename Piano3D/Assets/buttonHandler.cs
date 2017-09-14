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
    public List<int> current_ids;
    public bool isStarted;
    public string gameDataFileName = "melody.json";
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
        current_ids = new List<int>();
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

    public List<int> activateKey(int time)
    {
        // musical script -> which key is activated based on time (ticks)
        int n_time = jsonString["songs"][id_song]["duration"].Count;
        List<int> id_keys = new List<int>();
        for (int i = 0; i < n_time; i++)
        {
            int the_time;
            Int32.TryParse(jsonString["songs"][id_song]["duration"][i][0].ToString(), out the_time);
            int id_key;
            Int32.TryParse(jsonString["songs"][id_song]["duration"][i][1].ToString(), out id_key);
            if (time == the_time)
                id_keys.Add(id_key);
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
                    foreach (int current_id in current_ids)
                    {
                        if (imageComponent.name.Equals("Image" + current_id))
                        {
                            imageComponent.gameObject.SetActive(true);
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
