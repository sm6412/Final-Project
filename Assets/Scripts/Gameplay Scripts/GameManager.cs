﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // create singleton
    public static GameManager Instance;

    // get ref to player 
    public GameObject Player;

    // get ref to gridmaker script 
    private GridMaker gm;

    // get ref to the progress controller script
    private ProgressController progressController;

    // get ref to the species tracker script
    private SpeciesTracker st;

    // have to lists to hold matches along the
    // x and y axis 
    List<List<int>> matchPosX;
    List<List<int>> matchPosY;
    

    // current tile color you compare to 
    int currentGroup;
    int currentSpecies;

    // lerp variables 
    public float lerpSpeed;

    // particle effects 
    public GameObject particles;

    // score information
    public int scoreNum = 0;

    // grid recheck variables
    public bool recheckingGrid = false;
    bool foundMatches = false;

    // see if learping 
    bool isLerping = false;

    // holds the number of each species in a match
    int[] numOfEachSpecies;

    // ref to audio source
    public AudioSource audioSource;

    // sound effects for when the players makes a match
    public AudioClip successSound;
    

    // create a reactions for when you get matches
    public GameObject awesome;
    public GameObject bonus;

    // handle mode
    public bool easy;
    public bool medium;
    public bool hard;

    // used to determine when hint detection starts
    bool canStartTimer = true;
    HintDetection hd;

    // back button variables 
    float originalScale;
    
    // ref to audio source
    bool overOption = false;
    
    // sound effects for when the players makes a match
    public AudioClip mouseOver;
    public GameObject backButton;

    // Start is called before the first frame update
    void Awake()
    {
        // set singleton 
        Instance = this;
        Instantiate(Player);

        // ref other scripts
        gm = GameObject.Find("Grid Maker").GetComponent<GridMaker>();
        progressController = GameObject.Find("Progress Arrow").GetComponent<ProgressController>();
        st = GameObject.Find("Species Tracker").GetComponent<SpeciesTracker>();
        hd = GetComponent<HintDetection>();
        audioSource = GetComponent<AudioSource>();
        originalScale = backButton.transform.localScale.x;

    }

    // function checks to find and eliminate matches in the starting grid
    public void checkGrid()
    {
        for(int x = 0; x < gm.getWidth(); x++)
        {
            for (int y = 0; y < gm.getHeight(); y++)
            {
                bool res = checkTile(x,y);
                if (res == false)
                {
                    GameObject currentTile = gm.tiles[x, y];
                    Tile currentTileScript = currentTile.GetComponent<Tile>();
                    currentGroup = currentTileScript.group;
                    int randomVal = Random.Range(0, 6);
                    
                    // make sure tile gets a new animal group
                    while(randomVal == currentGroup)
                    {
                        randomVal = Random.Range(0, 6);
                    }
                    // change tile group
                    currentTileScript.SetSprite(randomVal);

                    // restart search
                    checkGrid();
                }
            }
        }
    }

    // check for matches when the grid repopulates 
    public void checkRepopulatedGrid()
    {
        for(int i = gm.getHeight() - 1; i >= 0; i--)
        {
            for (int j = gm.getWidth() - 1; j >= 0; j--)
            {
                if(gm.tiles[j,i].tag != "player")
                {
                    switchMatchCheck(j, i);
                    // if a match is found in the grid, restart the 
                    // match checking 
                    if (foundMatches == true)
                    {
                        i = gm.getHeight() - 1;
                        j = gm.getWidth() - 1 ;
                        foundMatches = false;
                    }
                }
            }
        }
        // end the rechecking 
        recheckingGrid = false;
    }

    // removes hint particles when the player moves 
    public void RemoveHintParticles()
    {
        GameObject[] hintParticlesList = GameObject.FindGameObjectsWithTag("hint");
        foreach (GameObject hintParticle in hintParticlesList)
        {
            GameObject.Destroy(hintParticle);
        }
            
    }

    // returns a boolean based on whether the tile has matches
    bool checkTile(int x, int y)
    {
        matchPosX = new List<List<int>>();
        matchPosY = new List<List<int>>();

        // currrent tile info
        GameObject currentTile = gm.tiles[x, y];
        Tile currentTileScript = currentTile.GetComponent<Tile>();
        currentGroup = currentTileScript.group;

        // check left tiles
        if ((x != (gm.getWidth() - 1)))
        {
            for (int i = x + 1; i < gm.getWidth(); i++)
            {
                string result = checkMatch(i, y, "x");
                if (result == "break")
                {
                    break;
                }
            }
        }


        // check right tiles 
        if ((x != 0))
        {
            for (int i = x - 1; i >= 0; i--)
            {
                string result = checkMatch(i, y, "x");
                if (result == "break")
                {
                    break;
                }
            }
        }

        // check bottom tiles 
        if ((y != (gm.getHeight() - 1)))
        {
            for (int i = y + 1; i < gm.getHeight(); i++)
            {
                string result = checkMatch(x, i, "y");
                if (result == "break")
                {
                    break;
                }
            }
        }

        // check top tiles 
        if ((y != 0))
        {
            for (int i = y - 1; i >= 0; i--)
            {
                string result = checkMatch(x, i, "y");
                if (result == "break")
                {
                    break;
                }
            }
        }

        if (matchPosX.Count>=2 || matchPosY.Count>=2)
        {
            return false;
        }
        return true; 
    }

    // checks for matches when a tile gains a new position
    public void switchMatchCheck(int x, int y)
    {
        matchPosX = new List<List<int>>();
        matchPosY = new List<List<int>>();

        // currrent tile info
        GameObject currentTile = gm.tiles[x, y];
        Tile currentTileScript = currentTile.GetComponent<Tile>();
        currentGroup = currentTileScript.group;
        currentSpecies = currentTileScript.type;

        // check left tiles
        if ((x != (gm.getWidth()-1)))
        {
            for (int i = x + 1; i < gm.getWidth(); i++)
            {
                string result = checkMatch(i, y, "x");
                if (result == "break")
                {
                    break;
                }
            }

        }


        // check right tiles 
        if ((x != 0))
        {
            for (int i = x - 1; i >= 0; i--)
            {
                string result = checkMatch(i, y, "x");
                if (result == "break")
                {
                    break;
                }

            }

        }

        // check bottom tiles 
        if ((y != (gm.getHeight()-1)))
        {
            for (int i = y + 1; i < gm.getHeight(); i++)
            {
                string result = checkMatch(x, i, "y");
                if (result == "break")
                {
                    break;
                }
            }

        }

        // check top tiles 
        if ((y != 0))
        {
            for (int i = y - 1; i >= 0; i--)
            {
                string result = checkMatch(x, i, "y");
                if (result == "break")
                {
                    break;
                }
            }
        }

        if (matchPosY.Count>=2 || matchPosX.Count>=2)
        {

            
          // if the grid recheck is still occuring and a match
          // is found, change found matches to true
            if (recheckingGrid == true)
            {
                foundMatches = true;
            }

            // disable player movement 
            PlayerController player = GameObject.Find("Player(Clone)").GetComponent<PlayerController>();
            player.switchCanMove(false);

            // destroy tiles 
            RemoveMatches(x,y);

            // repopulate grid
            Repopulate(x,y);

            // recheck the grid for matches
            if(recheckingGrid == false)
            {
                recheckingGrid = true;
                checkRepopulatedGrid();
            }
        }
        else
        {
            // give the tile an effect
            StartCoroutine(TileEffect(currentTile,gm.scaleAmount));
        }
        

    }

    
    // repopulates grid 
    public void Repopulate(int x, int y)
    {
        
        // start from the bottom left hand corner of the grid 
        for (int i = gm.getHeight() - 1; i >= 0; i--)
        {
            for (int j = 0; j < gm.getWidth(); j++)
            {
                // if the tile is empty, we have to replace it
                if (gm.tiles[j, i] == null)
                {
                    int newY = i - 1;

                    // look at all the tiles above in order to find one that
                    // can drop down into the empty space 
                    while (newY >= 0 && gm.tiles[j, newY] == null)
                    {
                        newY--;
                    }

                    // if a tile is found above, make it fall down
                    if (newY >= 0)
                    {
                        Vector2 startPos = gm.gridHolder[j,newY].transform.position;
                        Vector2 endPos = gm.gridHolder[j, i].transform.position; 
                        StartCoroutine(lerp(startPos, endPos,gm.tiles[j,newY]));
                        gm.tiles[j, i] = gm.tiles[j, newY];
                        gm.tiles[j, newY] = null;
                        // update player position
                        if (gm.tiles[j, i].name == "Player(Clone)")
                        {
                            GameObject.Find("Player(Clone)").GetComponent<PlayerController>().updatePlayerPos(j, i);
                        }
                    }
                    // if a tile is not found above the new space, one must be generated
                    else
                    {
                        // if at the top row, tile does not fall
                        if (i == 0)
                        {
                            gm.setNewTile(gm.gridHolder[j, 0].transform.position.x, gm.gridHolder[j, 0].transform.position.y,j,0);
                        }
                        // generate a new tile and make it fall down
                        else
                        {
                            gm.setNewTile(gm.gridHolder[j, 0].transform.position.x, gm.gridHolder[j, 0].transform.position.y, j, 0);
                            newY = 0;
                            Vector2 startPos = gm.gridHolder[j, newY].transform.position;
                            Vector2 endPos = gm.gridHolder[j, i].transform.position;
                            StartCoroutine(lerp(startPos, endPos, gm.tiles[j, newY]));
                            gm.tiles[j, i] = gm.tiles[j, newY];
                            gm.tiles[j, newY] = null;

                        }
                    }
                }
            }
        }
    }

    // tile effect for when the player switches positions
    // with it
    [Header ("Tile Animation Variables")]
    public int FramesCount;
    public float AnimationTimeSeconds;
    private IEnumerator TileEffect(GameObject currentTile, float _currentScale)
    {
        const float TargetScale = 0.5f;
        float InitScale = _currentScale;
        float _deltaTime = AnimationTimeSeconds / FramesCount;
        float _dx = (TargetScale - InitScale) / FramesCount;
        bool _upScale = true;
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
                if (currentTile == null)
                {
                    yield break;
                }
                currentTile.transform.localScale = Vector3.one * _currentScale;
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
                if (currentTile == null)
                {
                    yield break;
                }
                currentTile.transform.localScale = Vector3.one * _currentScale;
                yield return new WaitForSeconds(_deltaTime);
            }
            yield break;
        }
        
    }

    // functions creates the reaction animation
    GameObject currentReaction = null;
    [Header("Reaction Variables")]
    public float reactionTimer;
    public float reactionSpeed;
    private IEnumerator DisplayReaction(GameObject reaction)
    {
        Destroy(currentReaction);
        currentReaction = Instantiate(reaction);
        // grow
        while (currentReaction.transform.localScale.x < 1.3741)
        {
            float scaleAmount = currentReaction.transform.localScale.x + (Time.deltaTime*reactionSpeed);
            Vector3 scale = new Vector3(scaleAmount, scaleAmount, 1f);
            currentReaction.transform.localScale = scale;
            yield return null;

        }
        float temp = reactionTimer;
        // pause
        while (temp > 0)
        {
            temp -= Time.deltaTime;
            yield return null;

        }
        // shrink
        while (currentReaction.transform.localScale.x > 0)
        {
            float scaleAmount = currentReaction.transform.localScale.x - (Time.deltaTime*reactionSpeed);
            Vector3 scale = new Vector3(scaleAmount, scaleAmount, 1f);
            currentReaction.transform.localScale = scale;
            yield return null;

        }

        // destroy
        Destroy(currentReaction);
        yield break;

    }

    // this function is responsible for the lerping that occurs as the tiles fall down
    public IEnumerator lerp(Vector2 start, Vector2 end, GameObject g)
    {
        float slideLerp = 0;
        while(slideLerp < 1)
        {
            isLerping = true;
            if (g == null)
            {
                yield break;
            }
            slideLerp += Time.deltaTime / lerpSpeed;
            g.transform.localPosition = Vector3.Lerp(start, end, slideLerp);
            yield return null;
        }
        g.transform.localPosition = end;
        isLerping = false;
        yield break;
    }


    private void Update()
    {
        // get the mouse position
        Vector3 mousePos = Input.mousePosition;
        // set the z axis of the mouse
        mousePos.z = 0;

        // grab the mouse pos with regards to the screen
        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

        // use raycasting to see if the player clicks on a game object
        RaycastHit2D hit = Physics2D.Raycast(screenPos, Vector2.zero);
        
        if (hit && Input.GetMouseButtonDown(0))
        {
            if (hit.collider.tag == "back button")
            {
                // switch scene to start
                SceneManager.LoadScene("Start");
            }
        }
        else if (hit)
        {
            if (hit.collider.tag == "back button")
            {
                if (overOption == false)
                {
                    audioSource.PlayOneShot(mouseOver);
                    overOption = true;
                }
                float scaleAmount = 36f;
                Vector3 scale = new Vector3(scaleAmount, scaleAmount, 1f);
                backButton.transform.localScale = scale;
            }
        }
        else
        {
            float scaleAmount = originalScale;
            Vector3 scale = new Vector3(scaleAmount, scaleAmount, 1f);
            backButton.transform.localScale = scale;
            overOption = false;

        }

        // check for lerping to enable player movement
        if (isLerping == false)
        {
            GameObject.Find("Player(Clone)").GetComponent<PlayerController>().switchCanMove(true);
            // when the grid is not lerping or rechecking 
            if (recheckingGrid==false && canStartTimer==true && easy==true)
            {
                CheckForHint();
            }
            else
            {
                seconds = 0;
            }
        }
        else
        {
            seconds = 0;
            GameObject.Find("Player(Clone)").GetComponent<PlayerController>().switchCanMove(false);
        }
    }

    float seconds = 0;
    [Header("Hint Variables")]
    public float waitTimeForHint;
    void CheckForHint()
    {
        // see whether its time for a hint 
        if (seconds >= waitTimeForHint && easy==true)
        {
            PlayerController player = GameObject.Find("Player(Clone)").GetComponent<PlayerController>();
            // lets go to the hint detection script!
            hd.IdleHint(player.matrixX, player.matrixY, 1);
            seconds = 0;
        }
        else
        {
            seconds += Time.deltaTime;
        }

    }

    // checks for when a match occurs
    string checkMatch(int x, int y, string axis)
    {
        // check new tile with the switched tile for a match
        List<int> matchColor = new List<int>();
        GameObject newTile = gm.tiles[x, y];

        if (newTile.tag == "player")
        {
            return "break";
        }

        Tile tileScript = newTile.GetComponent<Tile>();
        int group = tileScript.group;

        if (currentGroup == group)
        {
            matchColor.Add(x);
            matchColor.Add(y);
        }
        else
        {
            return "break";
        }

        // put in correct match pos with correct axis 
        if (axis == "x")
        {
            matchPosX.Add(matchColor);
        }
        else
        {
            matchPosY.Add(matchColor);
        }
        
        return "continue";
    }


    // removes matches from grid
    public void RemoveMatches(int x, int y)
    {
        // remove hint particles from screen
        if (easy == true)
        {
            RemoveHintParticles();
        }
        
        // reset list that holds all the species 
        resetNumOfEachSpecies();

        Vector2 pos; 
        if (matchPosY.Count >= 2)
        {
            int length = matchPosY.Count;
            for (int i = 0; i < length; i++)
            {
                // destroy
                List<int> currentTile = matchPosY[i];
                int row = currentTile[0];
                int col = currentTile[1];
                
                // increment species in match chain
                GameObject newTile = gm.tiles[row, col];
                Tile tileScript = newTile.GetComponent<Tile>();
                int species = tileScript.type;
                numOfEachSpecies[species]++;
                st.updateSpecies(species);


                pos = gm.tiles[row, col].transform.position;

                Destroy(gm.tiles[row, col]);
                gm.tiles[row, col] = null;

                // emit particles
                (Instantiate(particles)).transform.position = pos;

            }
        }

        if (matchPosX.Count >= 2)
        {
            int length = matchPosX.Count;
            for (int i = 0; i < length; i++)
            {
                // destroy
                List<int> currentTile = matchPosX[i];
                int row = currentTile[0];
                int col = currentTile[1];

                 // increment species in match chain
                GameObject newTile = gm.tiles[row, col];
                Tile tileScript = newTile.GetComponent<Tile>();
                int species = tileScript.type;
                numOfEachSpecies[species]++;
                st.updateSpecies(species);

                pos = gm.tiles[row, col].transform.position;

                Destroy(gm.tiles[row, col]);
                gm.tiles[row, col] = null;

                // emit particles
                (Instantiate(particles)).transform.position = pos;

            }

        }

        // destroy switched tile 
        pos = gm.tiles[x, y].transform.position;
        Destroy(gm.tiles[x, y]);
        gm.tiles[x, y] = null;

        // emit particles
        (Instantiate(particles)).transform.position = pos;

        // increment species in match chain
        numOfEachSpecies[currentSpecies]++;
        st.updateSpecies(currentSpecies);

        // increase score
        IncrementScore();
    }

    // reset the number of each species found in a match
    void resetNumOfEachSpecies()
    {
        numOfEachSpecies = new int[25];
        for (int i = 0; i < 25; i++)
        {
            numOfEachSpecies[i] = 0;
        }
    }

    // increment the player's score
    void IncrementScore()
    {
        scoreNum = 0;
        bool haveBonus = false;
        for (int i = 0; i < 25; i++)
        {
            int currentNumOfSpecies = numOfEachSpecies[i];
            // if two or more of the same species are found in the match, double the score 
            // for those tiles 
            if (currentNumOfSpecies > 1)
            {

                scoreNum += currentNumOfSpecies * 2;
                haveBonus = true;
            }
            else if(currentNumOfSpecies == 1)
            {
                scoreNum++;
            }
        }

        // make progress controller move up
        StartCoroutine(progressController.AdjustProgress(scoreNum));

        // dont display reaction when rechecking the grid
        if (recheckingGrid == false)
        {
            if (haveBonus == true)
            {
                StartCoroutine(DisplayReaction(bonus));
            }
            else
            {
                StartCoroutine(DisplayReaction(awesome));
            }

        }


    }
}

