using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneManager : MonoBehaviour
{   
    public GameObject startText;
    float timer;
    bool loadingLevel;
    bool init;

    public int activeElement;
    public GameObject menuObj;
    public ButtonRef[] menuOptions;

    // Start is called before the first frame update
    void Start()
    {
        menuObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!init)
        {
            //it flicker the "Press Start" text
            timer += Time.deltaTime;
            if(timer > 0.6f)
            {
                timer = 0;
                startText.SetActive(!startText.activeInHierarchy);
            }
            // Where start == Space :V

            if(Input.GetKeyUp(KeyCode.Space))
            {
                init = true;
                startText.SetActive(false);
                menuObj.SetActive(true); // close "Press Start" text and show Options Menu
            }
        }else
        {
            if(!loadingLevel) //if not adready loading level
            {
                //indicate selected option
                menuOptions[activeElement].selected = true;

                //change the selected option base on input;
                if(Input.GetKeyUp(KeyCode.UpArrow))
                {
                    menuOptions[activeElement].selected = false;

                    if(activeElement > 0)
                    {
                        activeElement--;
                    }
                    else
                    {
                        activeElement = menuOptions.Length -1;
                    }
                }

                if(Input.GetKeyUp(KeyCode.DownArrow))
                {
                    menuOptions[activeElement].selected = false;

                    if(activeElement < menuOptions.Length -1)
                    {
                        activeElement++;
                    }
                    else
                    {
                        activeElement = 0;
                    }
                }

                //if hit space again
                if(Input.GetKeyUp(KeyCode.Space))
                {
                    //then load the level
                    Debug.Log("Load");
                    loadingLevel = true;
                    StartCoroutine("LoadLevel");
                    menuOptions[activeElement].transform.localScale *= 1.2f;
                    // base on our options
                    // TODO: 2 - players
                }
            }
        }
    }

    void HandleSelectedOption()
    {
        switch (activeElement)
        {
            case 0:
                CharacterManager.GetInstance().numberOfUser = 1;
                break;
            case 1:
                CharacterManager.GetInstance().numberOfUser = 2;
                CharacterManager.GetInstance().players[1].playerType = PlayerBase.PlayerType.user;
                break;
        }
    }

    IEnumerator LoadLevel()
    {
        HandleSelectedOption();
        yield return new WaitForSeconds(0.6f);
        
        MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, "select");
    }
}
