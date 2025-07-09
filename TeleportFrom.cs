using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportFrom : MonoBehaviour
{
    public string teleportID; // ユニークなテレポートID

    private void Start()
    {
        string enteredID = PlayerPrefs.GetString("TeleportID", "");

        if (enteredID == teleportID)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = transform.position;
            }
        }
    }
}
