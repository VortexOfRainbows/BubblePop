using UnityEngine;
using Steamworks; // Make sure to include this namespace

public class SteamTest : MonoBehaviour
{
    void Start()
    {
        // Always verify that SteamManager successfully connected first
        if (SteamManager.Initialized)
        {
            string personaName = SteamFriends.GetPersonaName();
            Debug.Log("Successfully connected to Steam! Hello, " + personaName);
        }
        else
        {
            Debug.LogError("Steamworks failed to initialize.");
        }
    }
}