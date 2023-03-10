using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Turn_Base : NetworkBehaviour
{
    public abstract void EnterTurn(S_TurnManager turnManager);
}
