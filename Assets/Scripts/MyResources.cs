using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyResources : MonoBehaviour
{
    private static string avartarPrefix = "Avatars/avatar";
    private static string avattarPostfix = "_2x";

    private static string invokeSucessIcon = "Icons/bubble_fill";
    private static string invokeFailIcon = "Icons/hide_icon";

    private static string ignoreIcon = "Icons/xmark_thick";
    private static string acceptIcon = "Icons/check";

    private static string ellipsis = "Icons/ellipsis_vertical";
    private static string ellipsis1 = "Icons/ellipsis_vertical_1";

    private static string exitIcon = "Icons/picto_san_1@2x";
    private static string exitIconSelected = "Icons/picto_san@2x";    

    public Sprite invokeSuccessSprite;
    public Sprite invokeFailSprite;

    public Sprite ignoreSprite;
    public Sprite acceptSprite;

    public Sprite ellipsisSprite;
    public Sprite ellipsis1Sprite;

    public Sprite exitSprite;
    public Sprite exitSelectSprite;

    public Dictionary<string, Sprite> avatars;

    private void Awake()
    {
        InitIconResource();
        InitAvatarResource();
    }

    void InitAvatarResource()
    {
        avatars = new Dictionary<string, Sprite>();

        for (int i = 1; i <= 12; i++) // Check items under Assets/Resources/Avatars
        {
            string name = GetAvatarName(i.ToString());
            Sprite sprite = Resources.Load(name, typeof(Sprite)) as Sprite;
            avatars.Add(name, sprite);
        }
    }

    void InitIconResource()
    {
        invokeSuccessSprite = Resources.Load(invokeSucessIcon, typeof(Sprite)) as Sprite;
        invokeFailSprite = Resources.Load(invokeFailIcon, typeof(Sprite)) as Sprite;

        ignoreSprite = Resources.Load(ignoreIcon, typeof(Sprite)) as Sprite;
        acceptSprite = Resources.Load(acceptIcon, typeof(Sprite)) as Sprite;

        ellipsisSprite = Resources.Load(ellipsis, typeof(Sprite)) as Sprite;
        ellipsis1Sprite = Resources.Load(ellipsis1, typeof(Sprite)) as Sprite;

        exitSprite = Resources.Load(exitIcon, typeof(Sprite)) as Sprite;
        exitSelectSprite = Resources.Load(exitIconSelected, typeof(Sprite)) as Sprite;
    }

    public string GetAvatarName(string index)
    {
        return avartarPrefix + index + avattarPostfix;
    }

    public Sprite GetAvatarSprite(string index)
    {
        return avatars[GetAvatarName(index)];
    }
}
