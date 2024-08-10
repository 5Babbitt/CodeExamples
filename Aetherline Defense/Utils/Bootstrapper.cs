using UnityEngine;

public class Bootstrapper : Singleton<Bootstrapper>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        DontDestroyOnLoad(Instantiate(Resources.Load("Systems")));
    }
}


