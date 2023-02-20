using UnityEngine;


public enum Channels
{
    World = 1,
    Guild,
    Party,
    Friends,
    Friend,
    Other
};

public class ChannelController : MonoBehaviour
{
    public Channels currentChannel { get; set; }

    private void Awake()
    {
        currentChannel = Channels.World; // default channel
    }
}
