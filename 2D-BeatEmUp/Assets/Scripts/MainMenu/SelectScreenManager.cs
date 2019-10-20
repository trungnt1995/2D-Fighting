using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SelectScreenManager : MonoBehaviour
{   
    public int numberOfPlayer = 1;
    public List<PlayerInterfaces> PlInterfaces = new List<PlayerInterfaces>();
    public PotraitInfo[] potraitPrefabs;
    public int maxX;
    public int maxY;
    PotraitInfo[,] charGrid;

    public GameObject potraitCanvas;
    
    bool loadLevel;
    public bool bothPlayerSelected;

    CharacterManager characterManager;

    public static SelectScreenManager instance;
    public static SelectScreenManager GetIntance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        //we start by getting the reference to the character manager
        characterManager = CharacterManager.GetInstance();
        numberOfPlayer = characterManager.numberOfUser;
        characterManager.solo = (numberOfPlayer == 1);

        //we create grid
        charGrid = new PotraitInfo[maxX, maxY];

        int x = 0;
        int y = 0;

        potraitPrefabs = potraitCanvas.GetComponentsInChildren<PotraitInfo>();

        //we need to go into all our potraits
        for(int i = 0; i < potraitPrefabs.Length; i++)
        {
            //assign a grid position
            potraitPrefabs[i].posX += x;
            potraitPrefabs[i].posY += y;

            charGrid[x,y] = potraitPrefabs[i];
            if(x < maxX - 1)
            {
                x++;
            }else
            {
                x = 0;
                y++;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!loadLevel)
        {
            for(int i = 0; i < PlInterfaces.Count; i++)
            {
                if(i < numberOfPlayer)
                {
                    if(!characterManager.players[i].hasCharacter)
                    {
                        PlInterfaces[i].playerBase = characterManager.players[i];

                        HandleSelectorPosition(PlInterfaces[i]);
                        HandleSelectScreenInput(PlInterfaces[i], characterManager.players[i].inputId);
                        HandleCharacterPreView(PlInterfaces[i]);

                    }
                }
                else
                {
                    characterManager.players[i].hasCharacter = true;
                }
            }

            if(bothPlayerSelected)
            {
                Debug.Log("loading");
                StartCoroutine("LoadLevel");
                loadLevel = true;
            }else
            {
                if(characterManager.players[0].hasCharacter && characterManager.players[1].hasCharacter)
                {
                    bothPlayerSelected = true;
                }
            }
        }
    }

    void HandleSelectScreenInput(PlayerInterfaces pl, string playerId)
    {   
        Debug.Log("id"+playerId);
        float vertical = Input.GetAxis("Vertical" + playerId);
        if(vertical != 0)
        {
            if(!pl.hitInputOnce)
            {
                if(vertical > 0)
                {
                    pl.activeY = (pl.activeY > 0) ? pl.activeY - 1 : maxY - 1;
                }else
                {
                    pl.activeY = (pl.activeY < maxY - 1) ? pl.activeY + 1 : 0;
                }

                pl.hitInputOnce = true;
            }
        }

        float horizontal = Input.GetAxis("Horizontal" + playerId);
        if(horizontal != 0)
        {
            if(!pl.hitInputOnce)
            {
                if(horizontal > 0)
                {
                    pl.activeX = (pl.activeX > 0) ? pl.activeX - 1 : maxX - 1;
                }else
                {
                    pl.activeX = (pl.activeX < maxX - 1) ? pl.activeX + 1 : 0;
                }

                pl.timerToReset = 0;
                pl.hitInputOnce = true;
            }
        }

        if(vertical == 0 && horizontal == 0)
        {
            pl.hitInputOnce = false;
        }

        if(pl.hitInputOnce)
        {
            pl.timerToReset += Time.deltaTime;

            if(pl.timerToReset > 0.8f)
            {
                pl.hitInputOnce = false;
                pl.timerToReset = 0;

            }
        }

        if(Input.GetButtonUp("Fire1" + playerId))
        {
            pl.createdCharacter.GetComponentInChildren<Animator>().Play("Kick");

            pl.playerBase.playerPrefab = characterManager.returnCharacterWithID(pl.activePotrait.characterId).prefab;

            pl.playerBase.hasCharacter = true;
        }
    }

    void HandleSelectorPosition(PlayerInterfaces pl)
    {
        pl.selector.SetActive(true);
        pl.activePotrait = charGrid[pl.activeX, pl.activeY];

        Vector2 selectorPosition = pl.activePotrait.transform.localPosition;
        selectorPosition = selectorPosition + new Vector2(potraitCanvas.transform.localPosition.x, potraitCanvas.transform.localPosition.y);

        pl.selector.transform.localPosition = selectorPosition;
    }

    void HandleCharacterPreView(PlayerInterfaces pl)
    {
        if(pl.previewPotrait != pl.activePotrait)
        {
            if(pl.createdCharacter != null)
            {
                Destroy(pl.createdCharacter);
            }

            GameObject go = Instantiate(CharacterManager.GetInstance().returnCharacterWithID(pl.activePotrait.characterId).prefab
            , pl.charVisPos.position
            , Quaternion.identity) as GameObject;

            pl.createdCharacter = go;

            pl.previewPotrait = pl.activePotrait;

            if(!string.Equals(pl.playerBase.playerId, characterManager.players[0].playerId))
            {
                pl.createdCharacter.GetComponent<StateManager>().lookRight = false;
            }
        }
    }

    IEnumerator LoadLevel(){
        for(int i = 0; i < characterManager.players.Count; i++)
        {
            if(characterManager.players[i].playerType == PlayerBase.PlayerType.ai)
            {
                if(characterManager.players[i].playerPrefab == null)
                {
                    int ranValue = Random.Range(0, potraitPrefabs.Length);
                    characterManager.players[i].playerPrefab = characterManager.returnCharacterWithID(potraitPrefabs[ranValue].characterId).prefab;

                    Debug.Log(potraitPrefabs[ranValue].characterId);
                }
            }
        }

        yield return new WaitForSeconds(2f);
       // SceneManager.LoadSceneAsync("level", LoadSceneMode.Single);
       if(characterManager.solo)
       {
           MySceneManager.GetInstance().CreateProgression();
           MySceneManager.GetInstance().LoadNextOnProgression();
       }else
       {
           MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "level_1");
       }
    }
    
}

[System.Serializable]
public class PlayerInterfaces
{
    public PotraitInfo activePotrait; //current active potrait for player 1
    public PotraitInfo previewPotrait;
    public GameObject selector;
    public Transform charVisPos;
    public GameObject createdCharacter;

    public int activeX;
    public int activeY;

    //variables for smoothing out input
    public bool hitInputOnce;
    public float timerToReset;

    public PlayerBase playerBase;
    
}
