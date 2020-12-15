using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{   
    public bool solo;
    public int numberOfUser;
    public List<PlayerBase> players = new List<PlayerBase>(); //list of all player and player type;

    //the list were we hold anything we need to know for each separate character;
    public List<CharacterBase> characterList = new List<CharacterBase>();

    public CharacterBase returnCharacterWithID(string id)
    {
        CharacterBase retVal = null;
        for(int i = 0; i < characterList.Count; i++)
        {
            if(string.Equals(characterList[i].charId, id))
            {
                retVal = characterList[i];
                break;
            }
        }
        return retVal;
    }

    public PlayerBase returnPlayerFromStates(StateManager states)
    {
        PlayerBase retVal = null;
        
        for( int i = 0; i < players.Count ; i++)
        {
            if(players[i].playerStates == states)
            {
                retVal = players[i];
                break;
            }
        }
        return retVal;
    } 

    public PlayerBase returnOppositePlater(PlayerBase pl)
    {
        PlayerBase retVal = null;
        for( int i = 0; i < players.Count ; i++)
        {
            if(players[i] != pl)
            {
                retVal = players[i];
                break;
            }
        }
        return retVal;
    }

    public int ReturnCharacterInt(GameObject prefab )
    {
        int retVal = 0;
        for(int i = 0; i < characterList.Count; i++)
        {
            if(characterList[i].prefab == prefab)
            {
                retVal = i;
                break;
            }
        }

        return retVal;
    }

    public static CharacterManager instance;
    public static CharacterManager GetInstance()
    {
        return instance;
    }

    
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

}

[System.Serializable]
public class CharacterBase
{
    public string charId;
    public GameObject prefab;
}

[System.Serializable]
public class PlayerBase
{
    public string playerId;
    public string inputId;
    public PlayerType playerType;
    public bool hasCharacter;
    public GameObject playerPrefab;
    public StateManager playerStates;
    public int score;

    public enum PlayerType
    {
        user,
        ai
    }
}

