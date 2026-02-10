using _Project.Scripts.Player;
using UnityEngine;

public class TeddyCutscene : MonoBehaviour
{
    private Animator _anim;
    
    private void StartCut()
    {
        _anim ??= GetComponent<Animator>();
        var player = FindAnyObjectByType<PlayerMovement>();
        transform.SetParent(player.transform);
    }
}
