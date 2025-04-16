using System.Runtime.InteropServices;

public static class LinuxConstants
{
    public const int PR_SET_NAME = 15;
    public const int PR_GET_NAME = 16;
}
public static class Helpers
{
    [DllImport("libc", SetLastError = true)]
    public static extern int prctl(int option, string arg2, ulong arg3, ulong arg4, ulong arg5);
}