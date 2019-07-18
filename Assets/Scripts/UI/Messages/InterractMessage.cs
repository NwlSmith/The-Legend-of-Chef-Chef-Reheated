using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterractMessage : Message {

    // constant message for interractable notifications, if nothing super important is happening, will be on until mm tells it to not be enabled.

    protected override void Awake()
    {
        priority = 2;
        base.Awake();
    }

    protected override IEnumerator Display()
    {
        // do not need ANYTHING here. The start tells it to turn on, the stop tells it to turn off.
        yield return null;
    }
}
