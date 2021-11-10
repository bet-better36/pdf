using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusScreen : MonoBehaviour
{
    PartyMemberStatusUI screenData;

    public void SetPartyStatusData(Pokemon pokemon)
    {
        screenData.SetData(pokemon);
    }

    
}
