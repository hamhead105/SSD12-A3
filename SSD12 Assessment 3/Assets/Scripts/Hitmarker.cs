using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Class for hitmarkers, which are world UI elements that indicate if the player has hit something.
 */
public class Hitmarker : MonoBehaviour
{
    public RawImage[] lines;
    private float transparency;
    private float size;

    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;

    [SerializeField] private float fadeDelay;
    private bool startFade;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float shrinkSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateHitmarkerAppearance();
    }

    /*
     * Called every frame, update the appearance of the hitmarker
     */
    private void UpdateHitmarkerAppearance()
    {
        if (transparency > fadeSpeed)
        {
            if (startFade) transparency -= fadeSpeed;
        }
        else
        {
            transparency = 0;
        }

        if (size - minSize > shrinkSpeed) size -= shrinkSpeed;
        else size = minSize;

        int increment = 0;
        foreach (RawImage rawImage in lines)
        {
            rawImage.color = new Vector4(rawImage.color.r, rawImage.color.g, rawImage.color.b, transparency);

            switch (increment)
            {
                case 0:
                    rawImage.transform.localPosition = new Vector3(size, size, 0);
                    break;
                case 1:
                    rawImage.transform.localPosition = new Vector3(-size, size, 0);
                    break;
                case 2:
                    rawImage.transform.localPosition = new Vector3(-size, -size, 0);
                    break;
                case 3:
                    rawImage.transform.localPosition = new Vector3(size, -size, 0);
                    break;
            }
            increment++;
        }
    }

    /*
     * Start the hitmarker animation, through adjusting scale and transparency 
     */
    public void HitAnimation()
    {
        transparency = 1;
        size = maxSize;
        startFade = false;
        Invoke("StartFade", fadeDelay);
    }

    /*
     * Will make the hitmarker fade away
     */
    private void StartFade()
    {
        startFade = true;
    }
}
