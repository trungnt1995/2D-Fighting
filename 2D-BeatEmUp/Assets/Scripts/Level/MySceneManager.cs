using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MySceneManager : MonoBehaviour
{
    public int progressionStages = 5;
    public List<string> levels = new List<string>();
    public List<MainScenes> mainScenes = new List<MainScenes>();

    bool waitToLoad;
    int progIndex;
    public List<SoloProgression> progressions = new List<SoloProgression>();

    CharacterManager chaM;

    // Start is called before the first frame update
    void Start()
    {
        chaM = CharacterManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateProgression()
    {   
        progressions.Clear();

        List<int> usedCharacter = new List<int>();

        int playerInt = chaM.ReturnCharacterInt(chaM.players[0].playerPrefab);
        usedCharacter.Add(playerInt);

        if(progressionStages > chaM.characterList.Count - 1)
        {
            progressionStages = chaM.characterList.Count - 2;
        }

        for(int i = 0; i < progressionStages; i++)
        {
            SoloProgression s = new SoloProgression();
            int levelInt = Random.Range(0, levels.Count);
            s.levelID = levels[levelInt];

            int charInt = UniqueRandomInt(usedCharacter, 0, chaM.characterList.Count);
            s.charId = chaM.characterList[charInt].charId;
            usedCharacter.Add(charInt);
            progressions.Add(s);
        }
    }

    public void LoadNextOnProgression()
    {
        string targetId = "";
        SceneType sceneType = SceneType.prog;

        if(progIndex > progressions.Count - 1)
        {
            targetId = "intro";
            sceneType = SceneType.main;
        }else
        {
            targetId = progressions[progIndex].levelID;

            chaM.players[1].playerPrefab = chaM.returnCharacterWithID(progressions[progIndex].charId).prefab;

            progIndex++;
        }

        RequestLevelLoad(sceneType, targetId);
    }

    public void RequestLevelLoad(SceneType st, string level)
    {
        if(!waitToLoad)
        {
            string targetId = "";
            switch (st)
            {
                case SceneType.main:
                    targetId = ReturnMainScene(level).levelId;
                    break;
                case SceneType.prog:
                targetId = level;
                break;
               
            }

            StartCoroutine(LoadScene(level));
            waitToLoad = true;
        }
    }

    int UniqueRandomInt(List<int> l, int min, int max)
    {
        int retVal = Random.Range(min, max);

        while(l.Contains(retVal))
        {
            retVal = Random.Range(min, max);
        }

        return retVal;
    }

    IEnumerator LoadScene(string levelid)
    {
        yield return SceneManager.LoadSceneAsync(levelid, LoadSceneMode.Single);
        waitToLoad = false;
    }

    MainScenes ReturnMainScene(string level)
    {
        MainScenes r = null;    
        for( int i = 0; i < mainScenes.Count; i++)
        {
            if(mainScenes[i].levelId == level)
            {
                r = mainScenes[i];  
                break;
            }
        }
        return r;
    }

   

    [System.Serializable]
    public class MainScenes
    {
        public string levelId;
    }

    [System.Serializable]
    public class SoloProgression
    {
        public string charId;
        public string levelID;
    }

    public static MySceneManager instance;
    public static MySceneManager GetInstance()
    {
        return instance;
    }


    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
 public enum SceneType
{
    main, prog
}
