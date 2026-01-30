using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FootstepSounds", menuName = "Scriptable Objects/FootstepSounds")]
public class FootstepSounds : ScriptableObject
{
    public List<AudioClip> Run;
    public List<AudioClip> Walk;
    public List<AudioClip> Jump;
    public List<AudioClip> Land;
}
