﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JO;

public class JOPlayers : Players
{
    public GameObject playerPrefab;
    public List<Vector2> positions = new List<Vector2>();



    public override void AddPlayer(string name, PlayerColor color)
    {
        GameObject playerGo = Instantiate(playerPrefab);
        playerGo.GetComponent<RectTransform>().SetParent(transform, false);
        JoPlayer player = playerGo.GetComponent<JoPlayer>();
        player.Setup(name, color, PopPosition());
        player.Show(0);
        players.Add(player);
    }

    public override void RemovePlayer(string name)
    {
        JoPlayer player = null;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].name == name)
            {
                player = players[i] as JoPlayer;
                break;
            }
        }

        if (player != null)
        {
            player.Hide(() => {
                PushPosition(player.GetPosition());
                players.Remove(player);
            });
        }
    }



    private Vector2 PopPosition()
    {
        int randomIndex = Random.Range(0, positions.Count);
        Vector2 position = positions[randomIndex];
        positions.RemoveAt(randomIndex);
        return position;
    }

    private void PushPosition(Vector2 position)
    {
        positions.Add(position);
    }


    public void ShufflePlayers()
    {
        players.ShuffleList();
    }

    public void ResetForRound()
    {
        for (int i = 0; i < players.Count; i++)
        {
            JoPlayer player = players[i] as JoPlayer;
            player.Reset();
        }
    }


    public JoPlayer SetNextGuesser()
    {
        JoPlayer guesser = null;

        for (int i = 0; i < players.Count; i++)
        {
            JoPlayer player = players[i] as JoPlayer;
            if (!player.hasBeenGuesser)
            {
                guesser = player;
                break;
            }
        }

        if (guesser == null)
        {
            players.ShuffleList();
            for (int i = 0; i < players.Count; i++)
            {
                JoPlayer player = players[i] as JoPlayer;
                player.hasBeenGuesser = false;
            }
        }
        guesser = players[0] as JoPlayer;
        guesser.SetAsGuesser();

        for (int i = 0; i < players.Count; i++)
        {
            JoPlayer player = players[i] as JoPlayer;
            if (!player.isGuesser) { player.SetAsPlayer(); }
        }

        return guesser;
    }


    public int PlayersSubmittedCount()
    {
        int count = 0;
        for (int i = 0; i < players.Count; i++) { JoPlayer player = players[i] as JoPlayer; if (!string.IsNullOrEmpty(player.guessedWord)) { count++; } }
        return count;
    }

    public List<JoPlayer> GetUniques()
    {
        List<JoPlayer> goodPlayers = new List<JoPlayer>();
        for(int i = 0; i < players.Count; i++)
        {
            JoPlayer comparePlayerA = players[i] as JoPlayer;
            if (comparePlayerA.isGuesser) { continue; }

            bool goodPlayer = true;
            for(int j = 0; j < players.Count; j++)
            {
                JoPlayer comparePlayerB = players[j] as JoPlayer;
                if (comparePlayerA != comparePlayerB &&
                    comparePlayerA.guessedWord == comparePlayerB.guessedWord)
                {
                    goodPlayer = false;
                    break;
                }
            }
            if (goodPlayer) { goodPlayers.Add(comparePlayerA); }
        }
        return goodPlayers;
    }


    [ButtonMethod]
    public void TestAdd()
    {
        AddPlayer("Calvin", new PlayerColor(Color.magenta, Color.green));
    }

    [ButtonMethod]
    public void TestRemove()
    {
        RemovePlayer("Calvin");
    }
}
