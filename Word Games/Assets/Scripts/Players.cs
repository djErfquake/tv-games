using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Players : MonoBehaviour
{
    protected List<Player> players = new List<Player>();
    public virtual int PlayerCount() { return players.Count; }

    public virtual void AddPlayer(string name, PlayerColor color) {}
    public virtual void RemovePlayer(string name) {}

    public virtual Player GetPlayer(string playerName)
    {
        for (int i = 0; i < players.Count; i++) { if (players[i].name == playerName) { return players[i]; } }
        return null;
    }

}
