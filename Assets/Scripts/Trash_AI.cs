// Trash_AI starts from here
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Trash_AI : MonoBehaviour
{
    NavMeshAgent NMA;
    Transform vacummPlayer;
    public int SlotsNeededToCarry = 1;
    [SerializeField]
    Animator animator;
    [SerializeField]
    GameObject DeadParticlePrefab;

	// Use this for initialization
	void Start ()
    {
        NMA = GetComponent<NavMeshAgent>();
        StartCoroutine(co_RandomDestinationSet());
    }
	
    // Update is called once per frame
    void Update()
    {
        if(vacummPlayer != null)
        {
            transform.position = Vector3.Lerp(transform.position, vacummPlayer.position, 0.03f);
        }
    }

    // Coroutines
    IEnumerator co_RandomDestinationSet()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            if(NMA.velocity.magnitude < 1)
            {
                Vector3 dest = new Vector3(Random.Range(GameController.instance.WorldBoundaryMinX.position.x, GameController.instance.WorldBoundaryMaxX.position.x),
                    0,
                    Random.Range(GameController.instance.WorldBoundaryMinZ.position.z, GameController.instance.WorldBoundaryMaxZ.position.z));
                NMA.SetDestination(dest);
                yield return new WaitForSeconds(1);
            }
        }
    }

    // he will DIE!!!! :(
    void ReceiveAttack(Transform attacker)
    {
        animator.CrossFade("Death", 0.1f);
        StopAllCoroutines();
        Destroy(NMA);
        tag = "DeadTrash";
        StartCoroutine(co_AfterDeath());
    }
    IEnumerator co_AfterDeath()
    {
        yield return new WaitForSeconds(8);
        tag = "Untagged";
        Instantiate(DeadParticlePrefab, transform.position, Quaternion.identity);
        for(int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Translate(0, -0.1f, 0, Space.World);
        }
        Destroy(gameObject);
    }
    
    // in Vacum
    void InVacum(Transform VacummPlayer)
    {
        if(vacummPlayer == null && tag == "DeadTrash")
        {
            animator.CrossFade("Collect", 0.1f);
            StopAllCoroutines();
            vacummPlayer = VacummPlayer;
        }
    }
}
// Trash_AI ends at here