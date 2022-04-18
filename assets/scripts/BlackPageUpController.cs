using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackPageUpController : MonoBehaviour
{
    public Color defaultColor;
    public Image img;
    private float speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //img = transform.GetComponent <Image> ();
    }

    // Update is called once per frame
    void Update()
    {
        img.color += new Color(0.0f, 0.0f, 0.0f, speed * Time.deltaTime);
    }

    void OnEnable () {
        img.color = defaultColor;
    }
}
