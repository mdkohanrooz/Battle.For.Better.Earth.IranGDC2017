using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITimeController : MonoBehaviour
{
    [SerializeField]
    Texture[] Numbers;
    [SerializeField]
    RawImage[] UITime;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(co_UIUpdate());
	}
	
	// Update is called once per frame
	IEnumerator co_UIUpdate ()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            if(GameController.instance.GameTime < 0)
                continue;
            string strTime = System.TimeSpan.FromSeconds(GameController.instance.GameTime).ToString();
            // 3 4 6 7
            UITime[0].texture = Numbers[System.Convert.ToInt32("" + strTime[3])];
            UITime[1].texture = Numbers[System.Convert.ToInt32("" + strTime[4])];
            UITime[2].texture = Numbers[System.Convert.ToInt32("" + strTime[6])];
            UITime[3].texture = Numbers[System.Convert.ToInt32("" + strTime[7])];
        }
	}
}
