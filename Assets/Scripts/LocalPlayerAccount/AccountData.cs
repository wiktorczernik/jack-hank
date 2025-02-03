using System;

[Serializable]
public class AccountData
{
    [NonSerialized] public string AccountName;
    public int Bouncy = 0;
}
