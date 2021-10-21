using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{

    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessege { get; set; }

    public Func<Pokemon, bool> OnBeforeMove;
    public Action<Pokemon> OnAfterTurn;

    
}
