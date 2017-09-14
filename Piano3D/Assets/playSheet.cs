using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playSheet : MonoBehaviour {

    public MovieTexture sheet;
    // Use this for initialization
    void Start ()
    {
        GetComponent<RawImage>().texture = sheet as MovieTexture;
        //sheet.Play();
        InvokeRepeating("myPlay", 9.2f, 0);
    }

    public void myPlay()
    {
        sheet.Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
