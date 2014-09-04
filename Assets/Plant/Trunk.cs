using UnityEngine;
using System.Collections;

public class Trunk : MonoBehaviour {

    private float growthRate = 0.01f;
    int growthType = 2;
	void Update () {
        switch(growthType)
        {
            case 0:    
                    transform.localScale += new Vector3 (growthRate/2,growthRate,growthRate/2);
                     transform.Translate( new Vector3(0, growthRate/2,0));
                break;
            case 1: growthRate = growthRate/4;
                    transform.localScale += new Vector3 (growthRate/2,growthRate,growthRate/2);
                    transform.Translate( new Vector3(0, growthRate/2,0));
                break;
            case 2: growthRate = growthRate *1.0005f; 
                    transform.localScale += new Vector3 (growthRate/2,growthRate,growthRate/2);
                    transform.Translate( new Vector3(0, growthRate/2,0));
                break;
            case 3: transform.localScale += new Vector3 (growthRate/4,growthRate,growthRate/4);
                transform.Translate( new Vector3(0, growthRate/2,0));
                break;
            case 4: growthRate = growthRate/2;
                transform.localScale += new Vector3 (growthRate/3,growthRate,growthRate/3);
                transform.Translate( new Vector3(0, growthRate/2,0));
                break;
            case 5: growthRate = growthRate *1.0005f; 
                transform.localScale += new Vector3 (growthRate,growthRate/4,growthRate);
                transform.Translate( new Vector3(0, growthRate/4,0));
                break;
                
             
        }
       




	}

    public void SetGrowthRate(float rate, int type)
    {
        growthRate = rate;
        growthType = type; 
    }
    public float GetVerticalGrowthRate()
    {
        if(growthType == 5)
        {
            return growthRate/4;
               
        }
        return growthRate;
    }
}
