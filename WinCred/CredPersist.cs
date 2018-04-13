namespace Advapi32.WinCred
{
    public enum CredPersist : uint
    {
        None = 0,
        Session = 1,
        LocalMachine = 2,
        Enterprise = 3,
        Max = 4,
    }
}
