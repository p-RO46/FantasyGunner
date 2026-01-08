using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static readonly WaitForEndOfFrame WaitForEndOfFrame =  new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate(); 
  
    private static readonly Dictionary<float, WaitForSeconds> _WaitForSeconds = new Dictionary<float, WaitForSeconds>();
  
    public static WaitForSeconds WaitForSecond(float seconds)
    {
        if(!_WaitForSeconds.TryGetValue(seconds, out var wfs))
        {
            wfs = new WaitForSeconds(seconds);
            _WaitForSeconds.Add(seconds, wfs);
        }
        return wfs;
    }
}