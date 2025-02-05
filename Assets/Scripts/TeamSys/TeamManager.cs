using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeamManager : NetworkBehaviour
{
    public static TeamManager Instance;

    // Dictionnaire pour stocker les équipes et les joueurs
    public NetworkVariable<Dictionary<int, List<ulong>>> Teams = new NetworkVariable<Dictionary<int, List<ulong>>>(
        new Dictionary<int, List<ulong>>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Compteur pour attribuer des ID d'équipe séquentiels
    private NetworkVariable<int> NextTeamID = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Méthode pour attribuer une équipe à un joueur
    public int AssignTeamToPlayer(ulong playerId)
    {
        // Attribuer un nouvel ID d'équipe séquentiel
        int teamID = NextTeamID.Value;
        NextTeamID.Value++; // Incrémenter pour le prochain joueur

        // Ajouter le joueur à l'équipe
        AddPlayerToTeam(teamID, playerId);

        Debug.Log($"[TeamManager] Joueur {playerId} a été assigné à l'équipe {teamID}");
        return teamID;
    }

    // Méthode pour ajouter un joueur à une équipe
    public void AddPlayerToTeam(int teamID, ulong playerId)
    {
        if (!Teams.Value.ContainsKey(teamID))
        {
            Teams.Value[teamID] = new List<ulong>();
        }

        if (!Teams.Value[teamID].Contains(playerId))
        {
            Teams.Value[teamID].Add(playerId);
            Teams.SetDirty(true);

            Debug.Log($"[TeamManager] Joueur {playerId} ajouté à l'équipe {teamID}");
        }
    }

    // Méthode pour retirer un joueur d'une équipe
    public void RemovePlayerFromTeam(int teamID, ulong playerId)
    {
        if (Teams.Value.ContainsKey(teamID))
        {
            if (Teams.Value[teamID].Contains(playerId))
            {
                Teams.Value[teamID].Remove(playerId);
                Teams.SetDirty(true);

                Debug.Log($"[TeamManager] Joueur {playerId} retiré de l'équipe {teamID}");

                // Supprimer l'équipe si elle est vide
                if (Teams.Value[teamID].Count == 0)
                {
                    Teams.Value.Remove(teamID);
                    Teams.SetDirty(true);

                    Debug.Log($"[TeamManager] Équipe {teamID} supprimée car elle est vide");
                }
            }
        }
    }

    // Méthode pour réinitialiser les équipes (à appeler lorsque la partie est terminée)
    public void ResetTeams()
    {
        if (IsServer)
        {
            Teams.Value.Clear();
            NextTeamID.Value = 1; // Réinitialiser le compteur d'équipe
            Teams.SetDirty(true);

            Debug.Log("[TeamManager] Équipes réinitialisées");
        }
    }

    // Méthode pour obtenir la liste des joueurs dans une équipe
    public List<ulong> GetPlayersInTeam(int teamID)
    {
        if (Teams.Value.ContainsKey(teamID))
        {
            return Teams.Value[teamID];
        }
        return new List<ulong>();
    }
}