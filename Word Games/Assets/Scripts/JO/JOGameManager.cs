using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using JO;
using NiceJson;

public class JOGameManager : GameManager
{
    [Header("UI")]
    public Logo logo;



    // game variables
    private List<string> allWords;

    // round variables
    private JoPlayer guessingPlayer;
    private string guessingWord = string.Empty;
    

    public void Start()
    {
        base.StartServer();
        server.OnPlayerJoined.AddListener(PlayerJoined);
        server.OnPlayerLeft.AddListener(PlayerLeft);
        server.OnMessage.AddListener(MessageFromServer);

        logo.Show(gameUrl);

        allWords = new List<string>(System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/JustOneWords.txt"));
        allWords.ShuffleList();
    }


    public void PlayerJoined(string playerName, PlayerColor playerColor)
    {
        players.AddPlayer(playerName, playerColor);
        if (players.PlayerCount() == 1) { logo.Hide(); }
    }

    public void PlayerLeft(string playerName)
    {
        players.RemovePlayer(playerName);
        if (players.PlayerCount() == 0) { logo.Show(gameUrl); }
    }


    public void MessageFromServer(JsonNode message)
    {
        string messageType = message["messageType"];
        switch(messageType)
        {
            case "START_ROUND":
                StartRound();
                break;
            case "WORD_SUBMITTED":
                WordSubmitted(message);
                break;
        }
    }


    public void StartRound()
    {
        JOPlayers allPlayers = players as JOPlayers;
        allPlayers.ResetForRound();
        allPlayers.ShufflePlayers();
        guessingPlayer = allPlayers.SetNextGuesser();

        guessingWord = allWords[0];
        allWords.RemoveAt(0);

        JsonObject message = new JsonObject();
        message.Add("gameType", "JO");
        message.Add("messageType", "START_ROUND");
        message.Add("player", guessingPlayer.name);
        message.Add("word", guessingWord);
        server.SendMessage(message);
    }


    public void WordSubmitted(JsonNode message)
    {
        JOPlayers allPlayers = players as JOPlayers;
        JoPlayer player = allPlayers.GetPlayer(message["name"]) as JoPlayer;
        player.guessedWord = message["word"];
        player.Show();

        int submittedCount = allPlayers.PlayersSubmittedCount();
        int totalPlayers = allPlayers.PlayerCount();
        if (allPlayers.PlayersSubmittedCount() == allPlayers.PlayerCount())
        {
            allPlayers.SendAllOffscreen();

            // compare all words
            List<JoPlayer> uniques = allPlayers.GetUniques();

            // show off the answers
            float delay = 0;
            for (int i = 0; i < uniques.Count; i++)
            {
                JoPlayer uniquePlayer = uniques[i];
                if (uniquePlayer.name != guessingPlayer.name)
                {
                    uniquePlayer.nameText.text = uniquePlayer.guessedWord;
                    uniquePlayer.OnScreen(delay);
                    delay += 0.3f;
                }
            }
        }
    }


}