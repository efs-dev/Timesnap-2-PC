using System;
using UnityEngine;

public class WaitUntilWithTimeout : CustomYieldInstruction
{
    private readonly Func<bool> _predicate;

    public WaitUntilWithTimeout(Func<bool> predicate, int timeoutSeconds)
    {
        var timeAtStart = Time.time;
        _predicate = () => predicate() || (Time.time - timeAtStart > timeoutSeconds);
    }

    public override bool keepWaiting { get { return !_predicate(); } }
}