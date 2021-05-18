using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField]
    GameObject[] TrashPrefabs;
    [SerializeField]
    Transform[] SpawnLocations;
    [SerializeField]
    GameObject UpgradeBoxPrefab;
    [SerializeField]
    GameObject PausePanel;
    [SerializeField]
    RawImage imageCountdownTimer;
    [SerializeField]
    Texture2D[] UIImages;
    [SerializeField]
    GameObject P1Victory, P2Victory,Tie;
    
    public Transform WorldBoundaryMinX;
    public Transform WorldBoundaryMaxX;
    public Transform WorldBoundaryMinZ;
    public Transform WorldBoundaryMaxZ;
    public float GameTime = 10;
    public bool isStarted;
    public bool isFinished;
    public static bool firstRun = true;
    bool playHit;

    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start ()
    {
        if(!firstRun)
        {
            PausePanel.SetActive(false);
            playHit = true;
        }
        StartCoroutine(co_GameTimeLine());
    }
	// Update is called once per frame
	void Update ()
    {
        if(isStarted && !isFinished)
            GameTime -= Time.deltaTime;
	}

    // Coroutines
    // Game Time Line
    IEnumerator co_GameTimeLine()
    {
        // Wait for start sign
        while(!playHit)
            yield return new WaitForSeconds(0.2f);
        // Countdown
        imageCountdownTimer.gameObject.SetActive(true);
        Camera.main.GetComponent<AudioSource>().Play();
        for(int i=3;i>=0;i--)
        {
            if(i != 0)
                AudioSource.PlayClipAtPoint(ResourceCenter.instance.audioClips[6], Camera.main.transform.position,0.3f);
            imageCountdownTimer.texture = UIImages[i];
            yield return new WaitForSeconds(1.1f);
        }
        imageCountdownTimer.gameObject.SetActive(false);
        // Game start flag
        isStarted = true;
        // Spawn units
        while(GameTime>0)
        {
            if((GameTime >= 88 && GameTime <= 90) ||
                (GameTime >= 68 && GameTime <= 70) ||
                (GameTime >= 48 && GameTime <= 50)  ||
                (GameTime >= 28 && GameTime <= 30) ||
                (GameTime >= 8 && GameTime <= 10)
                )
                if(GameObject.FindGameObjectWithTag("UpgradeBox") == null)
                    Instantiate(UpgradeBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            if(GameObject.FindGameObjectsWithTag("Trash").Length < 5)
            {
                Transform locationToSpawn = SpawnLocations[Random.Range(0, SpawnLocations.Length)];
                Instantiate(TrashPrefabs[Random.Range(0, TrashPrefabs.Length)], locationToSpawn.position, locationToSpawn.rotation);
            }
            yield return new WaitForSeconds(0.5f);
        }
        // Set Game Finish Flag
        isFinished = true;
        // Kill all trashes
        GameObject[] trashes = GameObject.FindGameObjectsWithTag("Trash");
        foreach(GameObject go in trashes)
            go.SendMessage("ReceiveAttack", transform);
        // Victory
        if(BaseController.Base_P1.Energy > BaseController.Base_P2.Energy)
        {
            P1Victory.SetActive(true);
            for(int i=0;i<20;i++)
            {
                yield return new WaitForSeconds(0.05f);
                PlayerController.Player1.transform.position = Vector3.Lerp(PlayerController.Player1.transform.position, new Vector3(0, 1.5f, 0), 0.05f);
            }
            PlayerController.Player1.animator.CrossFade("Victory", 0.1f);
            Camera.main.SendMessage("LockAtTarget", PlayerController.Player1.transform);
        }
        else if(BaseController.Base_P1.Energy < BaseController.Base_P2.Energy)
        {
            P2Victory.SetActive(true);
            for(int i = 0; i < 20; i++)
            {
                yield return new WaitForSeconds(0.01f);
                PlayerController.Player2.transform.position = Vector3.Lerp(PlayerController.Player2.transform.position, new Vector3(0, 1.5f, 0), 0.05f);
            }
            PlayerController.Player2.animator.CrossFade("Victory", 0.1f);
            Camera.main.SendMessage("LockAtTarget", PlayerController.Player2.transform);
        }
        else
        {
            Tie.SetActive(true);
        }
        yield return new WaitForSeconds(3);
        Tie.SetActive(false);
        P1Victory.SetActive(false);
        P2Victory.SetActive(false);
        PausePanel.SetActive(true);
    }

    // UI
    public void ui_Play()
    {
        if(isFinished)
        {
            firstRun = false;
            ui_Restart();
        }
        else
        {
            PausePanel.SetActive(false);
            playHit = true;
        }
    }
    public void ui_Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void ui_Exit()
    {
        Application.Quit();
    }
}
