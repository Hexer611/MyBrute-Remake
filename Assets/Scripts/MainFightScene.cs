using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MainFightScene : MonoBehaviour
{
    public BrutePlayer playerInstance;

    private BrutePlayer player1;
    private BrutePlayer player2;

    [SerializeField] private Player player1Entity;
    [SerializeField] private Player player2Entity;

    public List<BrutePlayer> players;

    IEnumerator mainFightFunction;
    bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.5f;
        player1 = Instantiate(playerInstance);
        player1.playerEntity = player1Entity;

        player2 = Instantiate(playerInstance);
        player2.playerEntity = player2Entity;

        player1.transform.position = new Vector3(-5, -3.3f, 0);
        player2.transform.position = new Vector3( 5, -3.3f, 0);

        player1.direction = Vector3.right;
        player2.direction = Vector3.left;

        player1.SetEnemy(player2);
        player2.SetEnemy(player1);

        player1.InitializePlayer();
        player2.InitializePlayer();

        players.Add(player1);
        players.Add(player2);

        mainFightFunction = FightFunction();
        StartCoroutine(mainFightFunction);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isRunning = !isRunning;
            if (isRunning)
                StopCoroutine(mainFightFunction);
            else
                StartCoroutine(mainFightFunction);
        }
    }

    public IEnumerator FightFunction()
    {
        float initialTimer = 0;
        MainFightUI.Instance.RefreshUI();
        
        if (initialTimer < 3)
        {
            initialTimer += Time.deltaTime;
            Debug.Log(initialTimer);
            yield return null;
        }

        while (true)
        {
            if (player1.IsAlive && player2.IsAlive)
                yield return player1.Attack();
            else
                break;

            if (player1.IsAlive && player2.IsAlive)
                yield return player2.Attack();
            else
                break;
        }

        yield return null;
    }
}
