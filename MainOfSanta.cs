using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Class in which the player's actions are programmed, used in the game "Saving Christmas"
public class MainOfSanta : MonoBehaviour
{


    private CharacterController controller;
    public Animator anime;
    public GameObject bomb;

    public int power;
    public int range;

    private float playerSpeed = 2f;

    public bool playerLive;

    public bool winFlag = false;
    public bool leftB = false;
    public bool rightB = false;
    public bool upB = false;
    public bool downB = false;
    public bool bombB = false;

    private bool left = false;
    private bool right = false;
    private bool up = true;
    private bool down = false;

    public bool granade = true;
    public bool nextGranade = true;
    public bool isHeart = false;

    public bool stopped = false;
   

    public GameObject pres1;
    public GameObject pres2;
    public GameObject pres3;
    public GameObject pres4;
    public GameObject pres5;



    public List<GameObject> myBonuses = new List<GameObject>();
    public List<GameObject> enemyDestroyList = new List<GameObject>();

    public ParticleSystem particleExplosion;
    public ParticleSystem particleBomb;

    public AudioSource audioBomb;
    public AudioSource audioSteps;
    public AudioSource audioUp;
    public AudioSource audioDown;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        anime.speed = 1f;
        playerLive = true;
    }

    // Update is called once per frame
    void Update()
    {
  
        if (playerLive)
        {
            if (!stopped)
            {
                if (leftB)
                {
                    if (left)
                    {

                    }
                    if (right)
                    {
                        transform.Rotate(0, -180, 0);
                        right = false;
                    }
                    if (up)
                    {
                        transform.Rotate(0, -90, 0);
                        up = false;
                    }
                    if (down)
                    {
                        transform.Rotate(0, 90, 0);
                        down = false;
                    }
                    left = true;
                    anime.SetBool("Running", true);
                    controller.Move(new Vector3(0, 0, -1) * Time.deltaTime * playerSpeed);

                }
                else if (rightB)
                {
                    if (left)
                    {
                        transform.Rotate(0, 180, 0);
                        left = false;
                    }
                    if (right)
                    {

                    }
                    if (up)
                    {
                        transform.Rotate(0, 90, 0);
                        up = false;
                    }
                    if (down)
                    {
                        transform.Rotate(0, -90, 0);
                        down = false;
                    }
                    right = true;
                    anime.SetBool("Running", true);
                    controller.Move(new Vector3(0, 0, 1) * Time.deltaTime * playerSpeed);
                }
                else if (upB)
                {
                    if (left)
                    {
                        transform.Rotate(0, 90, 0);
                        left = false;
                    }
                    if (right)
                    {
                        transform.Rotate(0, -90, 0);
                        right = false;
                    }
                    if (up)
                    {

                    }
                    if (down)
                    {
                        transform.Rotate(0, 180, 0);
                        down = false;
                    }
                    up = true;
                    anime.SetBool("Running", true);
                    controller.Move(new Vector3(-1, 0, 0) * Time.deltaTime * playerSpeed);
                }
                else if (downB)
                {
                    if (left)
                    {
                        transform.Rotate(0, -90, 0);
                        left = false;
                    }
                    if (right)
                    {
                        transform.Rotate(0, 90, 0);
                        right = false;
                    }
                    if (up)
                    {
                        transform.Rotate(0, -180, 0);
                        up = false;
                    }
                    if (down)
                    {

                    }
                    down = true;
                    anime.SetBool("Running", true);
                    controller.Move(new Vector3(1, 0, 0) * Time.deltaTime * playerSpeed);
                }
                else if (bombB)
                {
                    if ((granade) && (nextGranade))
                    {
                     
                        anime.SetBool("Running", false);
                        granade = false;
                        nextGranade = false;

                    }
                }
                else if(Input.anyKeyDown) 
                {
                    leftB = false;
                    rightB = false;
                    upB = false;
                    downB = false;
                    bombB = false;
                   
                    anime.SetBool("Running", false);
                }

            }
            if (stopped)
            {
                if (!isHeart)
                {
                   
                    anime.SetBool("Running", false);
                    Vector3 newPos = transform.position;
                    GameObject newHeart = Instantiate(myBonuses[3], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y + 2f) * 100f) / 100f, Mathf.Round((newPos.z - 0.1f) * 100f) / 100f), Quaternion.identity);
                    newHeart.transform.Rotate(90, 0, 0);
                    isHeart = true;
                    StartCoroutine(heart(newHeart));
                }
            }
        }
    }

    // Function is responsible for placing the bomb on the map
    public void bombActivate(List<GameObject> boxList, List<GameObject> presentsList, List<GameObject> listOfEnemy)
    {
        float minDist = 10000;
        int min = 10000;

        for (int j = 0; j < boxList.Count; j++)
        {
            float dist = Vector3.Distance(transform.position, boxList[j].transform.position);
            if (dist < minDist)
            {

                minDist = dist;
                min = j;

            }

        }

        GameObject newbomb = Instantiate(bomb, new Vector3(boxList[min].transform.position.x, 4.8f, boxList[min].transform.position.z), Quaternion.identity);

        particleBomb.transform.position = new Vector3(newbomb.transform.position.x, newbomb.transform.position.y+0.2f, newbomb.transform.position.z);
  
        particleBomb.Play();

        StartCoroutine(bombBlast(newbomb, presentsList, listOfEnemy));
        flashesBox(newbomb, boxList);


    }

    // Function is responsible for the explosion of the bomb and the effects of this action
    //
    // Code can be optimized by applying "Instantiate" before the game and working on prepared lists
    IEnumerator bombBlast(GameObject newbomb, List<GameObject> presentsList, List<GameObject> listOfEnemy)
    {

        yield return new WaitForSeconds(3);
        for (int m = range; m >= 1; m--)
        {

            for (int i = 0; i < presentsList.Count; i++)
            {


                if ((presentsList[i].transform.position.x == newbomb.transform.position.x && Mathf.Abs(presentsList[i].transform.position.z - newbomb.transform.position.z) == m) || (presentsList[i].transform.position.z == newbomb.transform.position.z && Mathf.Abs(presentsList[i].transform.position.x - newbomb.transform.position.x) == m))
                {
                  

                    if (presentsList[i].tag == "present1")
                    {
                        GameObject toDelete = presentsList[i];

                        Vector3 newPos = presentsList[i].transform.position;
                        presentsList.RemoveAt(i);
                        toDelete.SetActive(false);
                        i--;
                        int z = Random.Range(0, 100);
                        if (z > -1 && z < 5)
                        {
                            Instantiate(myBonuses[0], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                        }
                        if (z > 21 && z < 25)
                        {
                            
                            Instantiate(myBonuses[1], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                        }
                        if (z > 10 && z < 15)
                        {
                            Instantiate(myBonuses[2], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                        }
                    }
                    else if (presentsList[i].tag == "present2")
                    {
                        GameObject toDelete = presentsList[i];
                        Vector3 newPos = presentsList[i].transform.position;
                        if (power >= 2)
                        {
                            presentsList.RemoveAt(i);
                            toDelete.SetActive(false);
                            i--;
                            int z = Random.Range(0, 100);
                            if (z > -1 && z < 5)
                            {
                                Instantiate(myBonuses[0], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                            if (z > 21 && z < 25)
                            {

                                Instantiate(myBonuses[1], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                            if (z > 11 && z < 15)
                            {
                                Instantiate(myBonuses[2], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                        }
                        if (power == 1)
                        {
                            toDelete.SetActive(false);
                            GameObject newPresent = Instantiate(pres1, new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            presentsList[i] = newPresent;
                        }
                    }
                    else if (presentsList[i].tag == "present3")
                    {
                        GameObject toDelete = presentsList[i];
                        Vector3 newPos = presentsList[i].transform.position;

                        if (power >= 3)
                        {
                            presentsList.RemoveAt(i);
                            toDelete.SetActive(false);
                            i--;
                            int z = Random.Range(0, 100);
                            if (z > -1 && z < 5)
                            {
                                Instantiate(myBonuses[0], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                            if (z > 21 && z < 25)
                            {

                                Instantiate(myBonuses[1], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                            if (z > 11 && z < 15)
                            {
                                Instantiate(myBonuses[2], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                        }
                        if (power == 2)
                        {
                            toDelete.SetActive(false);
                            GameObject newPresent = Instantiate(pres1, new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            presentsList[i] = newPresent;
                        }
                        else if (power == 1)
                        {
                            toDelete.SetActive(false);
                            GameObject newPresent = Instantiate(pres2, new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            presentsList[i] = newPresent;
                        }
                    }
                    else if (presentsList[i].tag == "present4")
                    {
                        GameObject toDelete = presentsList[i];
                        Vector3 newPos = presentsList[i].transform.position;
                        if (power >= 4)
                        {
                            presentsList.RemoveAt(i);
                            toDelete.SetActive(false);
                            i--;
                            int z = Random.Range(0, 100);
                            if (z > -1 && z < 5)
                            {
                                Instantiate(myBonuses[0], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                            if (z > 21 && z < 25)
                            {

                                Instantiate(myBonuses[1], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                            if (z > 11 && z < 15)
                            {
                                Instantiate(myBonuses[2], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                        }
                        else if (power == 3)
                        {
                            toDelete.SetActive(false);
                            GameObject newPresent = Instantiate(pres1, new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            presentsList[i] = newPresent;
                        }
                        else if (power == 2)
                        {
                            toDelete.SetActive(false);
                            GameObject newPresent = Instantiate(pres2, new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            presentsList[i] = newPresent;
                        }
                        else if (power == 1)
                        {
                            toDelete.SetActive(false);
                            GameObject newPresent = Instantiate(pres3, new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            presentsList[i] = newPresent;
                        }
                    }
                    else if (presentsList[i].tag == "present5")
                    {
                        GameObject toDelete = presentsList[i];
                        Vector3 newPos = presentsList[i].transform.position;
                        if (power >= 5)
                        {
                            presentsList.RemoveAt(i);
                            toDelete.SetActive(false);
                            i--;
                            int z = Random.Range(0, 100);
                            if (z > -1 && z < 5)
                            {
                                Instantiate(myBonuses[0], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                            if (z > 21 && z < 25)
                            {

                                Instantiate(myBonuses[1], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                            if (z > 11 && z < 15)
                            {
                                Instantiate(myBonuses[2], new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            }
                        }
                        else if (power == 4)
                        {
                            toDelete.SetActive(false);
                            GameObject newPresent = Instantiate(pres1, new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            presentsList[i] = newPresent;
                        }
                        else if (power == 3)
                        {
                            toDelete.SetActive(false);
                            GameObject newPresent = Instantiate(pres2, new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            presentsList[i] = newPresent;
                        }
                        else if (power == 2)
                        {
                            toDelete.SetActive(false);
                            GameObject newPresent = Instantiate(pres3, new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            presentsList[i] = newPresent;
                        }
                        else if (power == 1)
                        {
                            toDelete.SetActive(false);
                            GameObject newPresent = Instantiate(pres4, new Vector3(Mathf.Round((newPos.x) * 100f) / 100f, Mathf.Round((newPos.y) * 100f) / 100f, Mathf.Round((newPos.z) * 100f) / 100f), Quaternion.identity);
                            presentsList[i] = newPresent;
                        }
                    }
                }
               
            }

            if ((Mathf.Abs(transform.position.x - newbomb.transform.position.x) <=(range+0.4f)) && (Mathf.Abs(transform.position.z - newbomb.transform.position.z) <= 0.4f)|| (Mathf.Abs(transform.position.x - newbomb.transform.position.x) <=0.4f) && (Mathf.Abs(transform.position.z - newbomb.transform.position.z) <= (range+0.4f)))
            {
                playerLive = false;
            }
            for (int p = 0; p < listOfEnemy.Count; p++)
            {
                if ((Mathf.Abs(listOfEnemy[p].transform.position.x - newbomb.transform.position.x) <= (range + 0.4f)) && (Mathf.Abs(listOfEnemy[p].transform.position.z - newbomb.transform.position.z) <= 0.4f) || (Mathf.Abs(listOfEnemy[p].transform.position.x - newbomb.transform.position.x) <= 0.4f) && (Mathf.Abs(listOfEnemy[p].transform.position.z - newbomb.transform.position.z) <= (range + 0.4f)))
                {
                    listOfEnemy[p].GetComponent<MainOfEnemy>().enemyLive = false;
                }
            }
            particleExplosion.transform.position = newbomb.transform.position;
            particleExplosion.Play();
            particleBomb.Stop();
            newbomb.SetActive(false);
            audioBomb.Play();
            nextGranade = true;
        }
    }


    // Function is responsible for showing the range of the bomb
    void flashesBox(GameObject newbomb, List<GameObject> boxList)
    {
        List<GameObject> flashesList = new List<GameObject>();
        for (int j = range; j > 0; j--)
        {
            for (int i = 0; i < boxList.Count; i++)
            {
                if (((Mathf.Abs(boxList[i].transform.position.x - newbomb.transform.position.x)) == j && boxList[i].transform.position.z == newbomb.transform.position.z) || ((Mathf.Abs(boxList[i].transform.position.z - newbomb.transform.position.z)) == j && boxList[i].transform.position.x == newbomb.transform.position.x))
                {
                    flashesList.Add(boxList[i]);
                    boxList[i].GetComponent<Renderer>().material.color = new Color32(255, 112, 109, 255);
                }
                if ((boxList[i].transform.position.x == newbomb.transform.position.x) && (boxList[i].transform.position.z == newbomb.transform.position.z))
                {
                    flashesList.Add(boxList[i]);
                    boxList[i].GetComponent<Renderer>().material.color = new Color32(255, 112, 109, 255);
                }
            }
        }
        StartCoroutine(whiteFounction1(flashesList));
        StartCoroutine(redFounction1(flashesList));
        StartCoroutine(whiteFounction2(flashesList));
        StartCoroutine(redFounction2(flashesList));
        StartCoroutine(whiteFounction3(flashesList));

    }

    IEnumerator whiteFounction1(List<GameObject> flashesList)
    {
        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < flashesList.Count; i++)
        {
            flashesList[i].GetComponent<Renderer>().material.color =  new Color32(225, 229, 255, 255);
        }

    }
    IEnumerator whiteFounction2(List<GameObject> flashesList)
    {
        yield return new WaitForSeconds(1.8f);
        for (int i = 0; i < flashesList.Count; i++)
        {
            flashesList[i].GetComponent<Renderer>().material.color = new Color32(225, 229, 255, 255);
        }

    }
    IEnumerator whiteFounction3(List<GameObject> flashesList)
    {
        yield return new WaitForSeconds(3.0f);
        for (int i = 0; i < flashesList.Count; i++)
        {
            flashesList[i].GetComponent<Renderer>().material.color = new Color32(225, 229, 255, 255);
        }

    }
    
    IEnumerator redFounction1(List<GameObject> flashesList)
    {
        yield return new WaitForSeconds(1.2f);
            for (int i = 0; i < flashesList.Count; i++)
            {
                flashesList[i].GetComponent<Renderer>().material.color = new Color32(255, 112, 109, 255);
        }
        }
    IEnumerator redFounction2(List<GameObject> flashesList)
    {
        yield return new WaitForSeconds(2.4f);
        for (int i = 0; i < flashesList.Count; i++)
        {
            flashesList[i].GetComponent<Renderer>().material.color = new Color32(255, 112, 109, 255);
        }
    }
    IEnumerator heart(GameObject myHeart)
    {
       
        yield return new WaitForSeconds(3);
        //Destroy(myHeart);
        myHeart.SetActive(false);
        stopped = false;
        isHeart = false;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
            

            
          if (hit.gameObject.tag == "pump1")
           {
            //Destroy(hit.gameObject);
            hit.gameObject.SetActive(false);
            audioUp.Play();
            power++;
           }
          if (hit.gameObject.tag == "pump2")
           {
            audioUp.Play();
            //Destroy(hit.gameObject);
            hit.gameObject.SetActive(false);
            range++;
           }
          if (hit.gameObject.tag == "pump3")
           {
            audioDown.Play();
            //Destroy(hit.gameObject);
            hit.gameObject.SetActive(false);
            stopped = true;
           }
        if (hit.gameObject.tag == "Finish")
        {
            winFlag = true;
            anime.SetBool("Running", false);

        }
        if (hit.gameObject.tag == "enemy")
        {
            playerLive = false;
        }


    }

    public void leftFunctionEnter()
    {
        leftB = true;
    }
    public void leftFunctionExit()
    {
        leftB = false;
        anime.SetBool("Running", false);
        
    }

    public void rightFunctionEnter()
    {
        rightB = true;
    }
    public void rightFunctionExit()
    {
        rightB = false;
        anime.SetBool("Running", false);
    }

    public void upFunctionEnter()
    {
        upB = true;
    }
    public void upFunctionExit()
    {
        upB = false;
        anime.SetBool("Running", false);
    }

    public void downFunctionEnter()
    {
        downB = true;
    }
    public void downFunctionExit()
    {
        downB = false;
        anime.SetBool("Running", false);
    }

    public void bombFunctionEnter()
    {
        bombB = true;
    }
    public void bombFunctionExit()
    {
        bombB = false;
    }
}

