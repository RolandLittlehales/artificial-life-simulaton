using UnityEngine;
using System.Collections;

public class Foliage : MonoBehaviour {

    private float growthRate = 0.01f;
    private int growthType = 0;
    private float trunk = 0;
    Color32 sage =  new Color32(55,107,47,0);
    Color32 darkGreen =  new Color32(0,51,0,0);
    Color32 paleGreen =  new Color32(95,200,47,0);
    Color32 olive =  new Color32(58,95,11,0);
    Color32 verdun =  new Color32(44,103,0,0);
	// Update is called once per frame
	void Update () {
       switch(growthType)
        {
            case 0: transform.localScale += new Vector3 (growthRate, growthRate/2, growthRate); 

                break;
            
            case 1: transform.localScale += new Vector3 (growthRate*1.5f, growthRate/2, growthRate*1.5f);
                transform.gameObject.renderer.material.color = sage;
                break;
            case 2: transform.localScale += new Vector3 (growthRate*2, growthRate/2, growthRate*2);
                transform.gameObject.renderer.material.color = darkGreen;
                break;
            case 3: transform.localScale += new Vector3 (growthRate*1.5f, growthRate/4, growthRate*1.5f);
                transform.gameObject.renderer.material.color = olive;
                break;
            case 4: transform.localScale += new Vector3 (growthRate, growthRate * 1.5f, growthRate);
                transform.gameObject.renderer.material.color = verdun;
                break;
            case 5: transform.localScale += new Vector3 (growthRate*1.0125f, growthRate*1.125f, growthRate*1.0125f);
                transform.gameObject.renderer.material.color = paleGreen;
                break;
        }
        transform.Translate(new Vector3 (0,trunk+growthRate/2,0));
	}

    public void SetGrowthRate(float rate, int type, float trunkGrowth)
    {
        growthRate = rate;
        growthType = type;
        trunk = trunkGrowth;
    }


}
