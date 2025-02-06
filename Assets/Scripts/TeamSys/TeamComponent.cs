using Unity.Netcode;
using UnityEngine;

public class TeamComponent : NetworkBehaviour
{
    public NetworkVariable<int> TeamID = new NetworkVariable<int>(-1); // -1 signifie "pas d'équipe"

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Attribuer une équipe au joueur lorsqu'il se connecte
            int assignedTeam = TeamManager.Instance.AssignTeamToPlayer(OwnerClientId);
            TeamID.Value = assignedTeam;

            Debug.Log($"[TeamComponent] Joueur {OwnerClientId} a rejoint l'équipe {TeamID.Value}");
        }

        // Écouter les changements de TeamID pour le débogage
        TeamID.OnValueChanged += OnTeamChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            // Retirer le joueur de son équipe lorsqu'il se déconnecte
            TeamManager.Instance.RemovePlayerFromTeam(TeamID.Value, OwnerClientId);

            Debug.Log($"[TeamComponent] Joueur {OwnerClientId} a quitté l'équipe {TeamID.Value}");
        }

        // Désabonner l'événement pour éviter les fuites de mémoire
        TeamID.OnValueChanged -= OnTeamChanged;
    }

    private void OnTeamChanged(int oldTeam, int newTeam)
    {
        Debug.Log($"[TeamComponent] Joueur {OwnerClientId} a changé d'équipe : {oldTeam} -> {newTeam}");
    }
}
