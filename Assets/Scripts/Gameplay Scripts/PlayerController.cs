﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // ref to gridmaker 
    private GridMaker gm;

    // ref to the progress controller 
    private ProgressController pc;

    // player matrix x and y position
    public int matrixX;
    public int matrixY;


    // bool to determine whether the player can move or not
    public bool canMove = true;


    // particle effects
    public GameObject redGlow;
    public GameObject orangeGlow;
    public GameObject yellowGlow;
    public GameObject greenGlow;


    bool isGreen = false;
    bool isYellow = false;
    bool isOrange = false;
    bool isRed = false;
    GameObject currentParticles = null;

    public AudioClip moveSound;
    private AudioSource audioSource;

    // get borders
    public GameObject greenBorder;
    public GameObject pinkBorder;
    public GameObject yellowBorder;
    public GameObject redBorder;
    public GameObject orangeBorder;




    // Start is called before the first frame update
    void Start()
    {
        // ref to grid maker 
        gm = GameObject.Find("Grid Maker").GetComponent<GridMaker>();
        pc = GameObject.Find("Progress Arrow").GetComponent<ProgressController>();
        audioSource = GetComponent<AudioSource>();

        // initialize player position
        this.transform.position = gm.tiles[2,2].transform.position;

        matrixX = 2;
        matrixY = 2;
        gm.tiles[2, 2] = this.gameObject;

        

  
    }

    // Update is called once per frame
    void Update()
    {

        checkPosition();
        
        // get the mouse position
        Vector3 mousePos = Input.mousePosition;
        // set the z axis of the mouse
        mousePos.z = 0;

        // grab the mouse pos with regards to the screen
        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

        // use raycasting to see if the player clicks on a game object
        RaycastHit2D hit = Physics2D.Raycast(screenPos, Vector2.zero);

        GameObject[] gms = GameObject.FindGameObjectsWithTag("border");
        if (hit && Input.GetMouseButtonDown(0))
        {
            // if the user clicks on the start 
            // button, start the gameplay scene
            if ((matrixY-1>=0) && hit.collider.tag == "tile" && hit.collider.transform.position == gm.gridHolder[matrixX, matrixY - 1].transform.position)
            {
                // make the tile look selected 
                
                if (gms.Length == 0)
                {
                    isSelected(matrixX, matrixY + 1);
                }
                
                GameManager.Instance.RemoveHintParticles();
                audioSource.PlayOneShot(moveSound);
                switchUp();
            }
            else if ((matrixY+1 <= (gm.getHeight()-1)) && hit.collider.tag == "tile" && hit.collider.transform.position == gm.gridHolder[matrixX, matrixY + 1].transform.position)
            {
                if (gms.Length == 0)
                {
                    isSelected(matrixX, matrixY + 1);
                }
                
                GameManager.Instance.RemoveHintParticles();
                audioSource.PlayOneShot(moveSound);
                switchDown();

            }
            else if ((matrixX + 1 <= (gm.getWidth() - 1)) && hit.collider.tag == "tile" && hit.collider.transform.position == gm.gridHolder[matrixX + 1, matrixY].transform.position)
            {
                if (gms.Length == 0)
                {
                    isSelected(matrixX + 1, matrixY);
                }
                
                GameManager.Instance.RemoveHintParticles();
                audioSource.PlayOneShot(moveSound);
                switchRight();

            }
            else if ((matrixX - 1 >= 0)  && hit.collider.tag == "tile" && hit.collider.transform.position == gm.gridHolder[matrixX - 1, matrixY].transform.position)
            {
                if (gms.Length == 0)
                {
                    isSelected(matrixX - 1, matrixY);
                }
                
                GameManager.Instance.RemoveHintParticles();
                audioSource.PlayOneShot(moveSound);
                switchLeft();

            }
        }
        else if (hit)
        {
            if ((matrixY - 1 >= 0) && hit.collider.tag == "tile" && hit.collider.transform.position == gm.gridHolder[matrixX, matrixY - 1].transform.position)
            {
                // make the tile look selected 
                if (gms.Length == 0)
                {
                    isSelected(matrixX, matrixY - 1);
                }
                
            }
            else if ((matrixY + 1 <= (gm.getHeight() - 1)) && hit.collider.tag == "tile" && hit.collider.transform.position == gm.gridHolder[matrixX, matrixY + 1].transform.position)
            {
                if (gms.Length == 0)
                {
                    isSelected(matrixX, matrixY + 1);
                }
                

            }
            else if ((matrixX + 1 <= (gm.getWidth() - 1)) && hit.collider.tag == "tile" && hit.collider.transform.position == gm.gridHolder[matrixX + 1, matrixY].transform.position)
            {
                if (gms.Length == 0)
                {
                    isSelected(matrixX + 1, matrixY);
                }
                

            }
            else if ((matrixX - 1 >= 0) && hit.collider.tag == "tile" && hit.collider.transform.position == gm.gridHolder[matrixX - 1, matrixY].transform.position)
            {
                if (gms.Length == 0)
                {
                    isSelected(matrixX - 1, matrixY);
                }
                

            }

        }
        else
        {
            GameObject[] borderParticlesList = GameObject.FindGameObjectsWithTag("border");
            foreach (GameObject borderParticle in borderParticlesList)
            {
                GameObject.Destroy(borderParticle);
            }
        }
    }

    void isSelected(int x, int y)
    {
        Vector2 pos = gm.gridHolder[x, y].transform.position;
        GameObject currentTile = gm.tiles[x, y];
        Tile currentTileScript = currentTile.GetComponent<Tile>();
        int currentGroup = currentTileScript.group;
        if (currentGroup==1)
        {
            GameObject border = Instantiate(greenBorder);
            Vector3 scale = new Vector3(gm.scaleAmount, gm.scaleAmount, 1f);
            border.transform.localScale = scale;
            border.transform.position = pos;
        }
        else if (currentGroup==2)
        {
            GameObject border = Instantiate(pinkBorder);
            Vector3 scale = new Vector3(gm.scaleAmount, gm.scaleAmount, 1f);
            border.transform.localScale = scale;
            border.transform.position = pos;

        }
        else if (currentGroup == 3)
        {
            GameObject border = Instantiate(redBorder);
            Vector3 scale = new Vector3(gm.scaleAmount, gm.scaleAmount, 1f);
            border.transform.localScale = scale;
            border.transform.position = pos;

        }
        else if (currentGroup == 4)
        {
            GameObject border = Instantiate(yellowBorder);
            Vector3 scale = new Vector3(gm.scaleAmount, gm.scaleAmount, 1f);
            border.transform.localScale = scale;
            border.transform.position = pos;

        }
        else if (currentGroup == 5)
        {
            GameObject border = Instantiate(orangeBorder);
            Vector3 scale = new Vector3(gm.scaleAmount, gm.scaleAmount, 1f);
            border.transform.localScale = scale;
            border.transform.position = pos;

        }

    }

    // update player position
    public void updatePlayerPos(int x, int y)
    {

        matrixX = x;
        matrixY = y;

    }


    private float _currentScale = InitScale;
    private const float TargetScale = 0.81f;
    private const float InitScale = .70f;
    private const int FramesCount = 100;
    private const float AnimationTimeSeconds = 2;
    private float _deltaTime = AnimationTimeSeconds / FramesCount;
    private float _dx = (TargetScale - InitScale) / FramesCount;
    private bool _upScale = true;
    private IEnumerator Breath(GameObject Lungs)
    {
        while (true)
        {
            while (_upScale)
            {
                _currentScale += _dx;
                if (_currentScale > TargetScale)
                {
                    _upScale = false;
                    _currentScale = TargetScale;
                }
                Lungs.transform.localScale = Vector3.one * _currentScale;
                yield return new WaitForSeconds(_deltaTime);
            }

            while (!_upScale)
            {
                _currentScale -= _dx;
                if (_currentScale < InitScale)
                {
                    _upScale = true;
                    _currentScale = InitScale;
                }
                Lungs.transform.localScale = Vector3.one * _currentScale;
                yield return new WaitForSeconds(_deltaTime);
            }
        }
    }

    // switch player with a tile underneath
    void switchDown()
    {
        GameObject tileToSwitch = gm.tiles[matrixX,matrixY+1];
        Vector2 tempPos = tileToSwitch.transform.position;
        int xToCheck = matrixX;
        int yToCheck = matrixY;

        tileToSwitch.transform.localPosition = this.transform.position;
        gm.tiles[matrixX, matrixY] = tileToSwitch;

        matrixY += 1;
        this.transform.localPosition = tempPos;
        gm.tiles[matrixX, matrixY] = this.gameObject;

        GameManager.Instance.switchMatchCheck(xToCheck, yToCheck);
    }

    // switch player with the tile above
    void switchUp()
    {
        GameObject tileToSwitch = gm.tiles[matrixX, matrixY - 1];
        Vector2 tempPos = tileToSwitch.transform.position;
        int xToCheck = matrixX;
        int yToCheck = matrixY;

        tileToSwitch.transform.localPosition = this.transform.position;
        gm.tiles[matrixX, matrixY] = tileToSwitch;

        matrixY -= 1;
        this.transform.localPosition = tempPos;
        gm.tiles[matrixX, matrixY] = this.gameObject;

        // check for matches
        GameManager.Instance.switchMatchCheck(xToCheck,yToCheck);
        Debug.Log("Player is located at " + matrixX + " " + matrixY);

    }

    // switch player with the tile to the right of it
    void switchRight()
    {
        GameObject tileToSwitch = gm.tiles[matrixX + 1, matrixY];
        Vector2 tempPos = tileToSwitch.transform.position;
        int xToCheck = matrixX;
        int yToCheck = matrixY;

        tileToSwitch.transform.localPosition = this.transform.position;
        gm.tiles[matrixX, matrixY] = tileToSwitch;

        matrixX += 1;
        this.transform.localPosition = tempPos;
        gm.tiles[matrixX, matrixY] = this.gameObject;

        GameManager.Instance.switchMatchCheck(xToCheck, yToCheck);
        Debug.Log("Player is located at " + matrixX + " " + matrixY);

    }

    // switch player with the tile to the left of it
    void switchLeft()
    {
        GameObject tileToSwitch = gm.tiles[matrixX - 1, matrixY];
        Vector2 tempPos = tileToSwitch.transform.position;
        int xToCheck = matrixX;
        int yToCheck = matrixY;

        tileToSwitch.transform.localPosition = this.transform.position;
        gm.tiles[matrixX, matrixY] = tileToSwitch;

        matrixX -= 1;
        this.transform.localPosition = tempPos;
        gm.tiles[matrixX, matrixY] = this.gameObject;

        GameManager.Instance.switchMatchCheck(xToCheck, yToCheck);

    }



    // turns movement on and off
    public void switchCanMove(bool res)
    {
        canMove = res;
    }

    // change player color according to current grade
    void checkPosition()
    {
        string greenString = "#56FF00";
        string yellowString = "#FFFF01";
        string orangeString = "#FFAA01";
        string redString = "#FE0003";

        // B range
        // make red
        double pos = pc.transform.position.y;
        if (pos >= -2.2 && pos < -1.19)
        {
            if (isRed == false)
            {
                Color newColor;
                ColorUtility.TryParseHtmlString(redString, out newColor);
                this.GetComponent<SpriteRenderer>().color = newColor;
                isRed = true;
                // add particles 
                currentParticles = Instantiate(redGlow);
            }
        }
        // B+ range
        // make orange
        else if (pos >= -1.19 && pos < 0.4)
        {
            if (isOrange == false)
            {
                Color newColor;
                ColorUtility.TryParseHtmlString(orangeString, out newColor);
                this.GetComponent<SpriteRenderer>().color = newColor;
                isOrange = true;

                // set particles 
                Destroy(currentParticles);
                currentParticles = Instantiate(orangeGlow);
            }


        }
        // A range
        // make yellow
        else if (pos >= 0.4 && pos < 1.97)
        {
            if (isYellow == false)
            {
                Color newColor;
                ColorUtility.TryParseHtmlString(yellowString, out newColor);
                this.GetComponent<SpriteRenderer>().color = newColor;
                isYellow = true;
                // add particles
                Destroy(currentParticles);
                currentParticles = Instantiate(yellowGlow);
            }
        }
        // A+ range
        else if (pos >= 1.97 && pos < 3.11)
        {
            if (isGreen == false)
            {
                Color newColor;
                ColorUtility.TryParseHtmlString(greenString, out newColor);
                this.GetComponent<SpriteRenderer>().color = newColor;
                isRed = true;
                // add particles
                Destroy(currentParticles);
                currentParticles = Instantiate(redGlow);

            }

        }
    }
}
