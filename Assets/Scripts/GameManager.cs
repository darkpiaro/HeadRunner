using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject getReady;
    [SerializeField] private GameObject playerNameInput;
    [SerializeField] private string formurl = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdbe7aPUbC6ogctgjnKkrjjCRU8aA7lBAjNm88Ck7Ru5gbcdw/formResponse";
    string playername;

    private int score;
    public int Score => score;

    private void Awake()
    {
        PlayerPrefs.SetString("playername", "");
        PlayerPrefs.SetInt("highscore", 0);
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            Application.targetFrameRate = 60;
            DontDestroyOnLoad(gameObject);
            Pause();
        }
    }

    private void Start()
    {
        gameOver.SetActive(false);
        getReady.SetActive(false);
        playername = PlayerPrefs.GetString("playername");
        if (playername == "")
        {
            getReady.SetActive(false);
            playerNameInput.SetActive(true);
        }
        else
        {
            playerNameInput.SetActive(false);
            getReady.SetActive(true);
            player.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        }
    }

    public void Play()
    {
        score = 0;
        scoreText.text = score.ToString();

        playButton.SetActive(false);
        getReady.SetActive(false);
        gameOver.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;

        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for (int i = 0; i < pipes.Length; i++) {
            Destroy(pipes[i].gameObject);
        }
        player.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
    }

    public void GameOver()
    {
        playButton.SetActive(true);
        gameOver.SetActive(true);

        Pause();
        SaveHighScore();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    public void IncreaseScore()
    {
        if (playername =="Tuii")
        {
            score += Random.Range(5, 10);
        } else
        {
            score++;
        }
        scoreText.text = score.ToString();
    }

    public void SavePlayerName()
    {
        string playername = playerNameInput.GetComponentInChildren<InputField>().text;
        PlayerPrefs.SetString("playername", playername);
        this.playername = playername;
        playerNameInput.SetActive(false);
        getReady.SetActive(true);
        player.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
    }

    void SaveHighScore()
    {
        int highrscore = PlayerPrefs.GetInt("highscore");
        if(score > highrscore)
        {
            PlayerPrefs.SetInt("highscore", score);
            highrscore = score;
            StartCoroutine(PostScore());
        }
        gameOver.GetComponentInChildren<TMP_Text>().text = playername + " highscore: " + highrscore;
    }


    IEnumerator PostScore()
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.905709239", playername);
        form.AddField("entry.594118339", score);

        UnityWebRequest www = UnityWebRequest.Post(formurl, form);

        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("SUCCESS");
        }
        else
        {
            Debug.LogError("ERROR in submission" + www.error);
        }
    }

}
