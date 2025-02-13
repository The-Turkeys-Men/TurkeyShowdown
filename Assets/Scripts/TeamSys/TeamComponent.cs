using System;
using Unity.Netcode;
using UnityEngine;

public class TeamComponent : NetworkBehaviour
{
    public NetworkVariable<int> TeamID = new NetworkVariable<int>(-1); // -1 signifie "pas d'équipe"
    public Action OnTeamChangedEvent;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Attribuer une équipe au joueur lorsqu'il se connecte
            int assignedTeam = TeamManager.Instance.AssignTeamToPlayer(OwnerClientId);
            TeamID.Value = assignedTeam;
        }

        // Écouter les changements de TeamID pour le débogage
        TeamID.OnValueChanged += OnTeamChanged;
    }

    private void OnTeamChanged(int oldTeam, int newTeam)
    {
        OnTeamChangedEvent?.Invoke();
    }
}
