
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

// BOOL CloseHandle(
//   [in] HANDLE hObject
// );

// BOOL CreateProcessA(
//   [in, optional]      LPCSTR                lpApplicationName,
//   [in, out, optional] LPSTR                 lpCommandLine,
//   [in, optional]      LPSECURITY_ATTRIBUTES lpProcessAttributes,
//   [in, optional]      LPSECURITY_ATTRIBUTES lpThreadAttributes,
//   [in]                BOOL                  bInheritHandles,
//   [in]                DWORD                 dwCreationFlags,
//   [in, optional]      LPVOID                lpEnvironment,
//   [in, optional]      LPCSTR                lpCurrentDirectory,
//   [in]                LPSTARTUPINFOA        lpStartupInfo,
//   [out]               LPPROCESS_INFORMATION lpProcessInformation
// );

// HANDLE CreateEventA(
//   [in, optional] LPSECURITY_ATTRIBUTES lpEventAttributes,
//   [in]           BOOL                  bManualReset,
//   [in]           BOOL                  bInitialState,
//   [in, optional] LPCSTR                lpName
// );

// BOOL GetExitCodeProcess(
//   [in]  HANDLE  hProcess,
//   [out] LPDWORD lpExitCode
// );

// ULONGLONG GetTickCount64();

// DWORD WaitForSingleObject(
//   [in] HANDLE hHandle,
//   [in] DWORD  dwMilliseconds
// );

namespace My
{

using BOOL = UInt32;
using DWORD = UInt32;
using HANDLE = IntPtr;
using HRESULT = UInt32;

public class WinApi
{
    public const HRESULT ERROR_SIGNAL_REFUSED = 0x8000009C;
    public const HRESULT E_FAIL = 0x80004005;
    public const HRESULT WAIT_TIMEOUT = 0x00000102;
    public const HRESULT WAIT_OBJECT_0 = 0x0;
    public const DWORD STILL_ACTIVE = 259;

    internal enum CreationFlags
    {
        CREATE_SUSPENDED = 0x00000004,
        CREATE_NEW_CONSOLE = 0x00000010,
        CREATE_NEW_PROCESS_GROUP = 0x00000200,
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,
        CREATE_SEPARATE_WOW_VDM = 0x00000800,
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        CREATE_NO_WINDOW = 0x08000000
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct STARTUPINFOA
    {
        public String lpReserved;
        public String lpDesktop;
        public String lpTitle;
        public Int32 dwX;
        public Int32 dwY;
        public Int32 dwXSize;
        public Int32 dwYSize;
        public Int32 dwXCountChars;
        public Int32 dwYCountChars;
        public Int32 dwFillAttribute;
        public Int32 dwFlags;
        public Int16 wShowWindow;
        public Int16 cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public HANDLE hProcess;
        public HANDLE hThread;
        public DWORD dwProcessId;
        public DWORD dwThreadId;

        public override string ToString()
        {
            return string.Format(
                "hProcess={0} hThread={1} dwProcessId={2} dwThreadId={3}",
                hProcess, hThread, dwProcessId, dwThreadId);
        }
    }

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern BOOL CloseHandle(
        HANDLE hObject);

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern BOOL CreateProcessA(
        String lpApplicationName,
        [In] [Out] String lpCommandLine,
        IntPtr lpProcessAttributes,
        IntPtr lpThreadAttributes,
        BOOL bInheritHandles,
        DWORD dwCreationFlags,
        IntPtr lpEnvironment,
        String lpCurrentDirectory,
        [In] ref STARTUPINFOA lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern HANDLE CreateEventA(
        IntPtr lpEventAttributes,
        BOOL bManualReset,
        BOOL bInitialState,
        String lpName);

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern BOOL GetExitCodeProcess(
        HANDLE hProcess,
        out DWORD lpExitCode);

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern UInt64 GetTickCount64();

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern HRESULT WaitForSingleObject(
        HANDLE hHandle,
        DWORD dwMilliseconds);
}

public class Spec
{
    public static bool CreateProcess(string args, out WinApi.PROCESS_INFORMATION processInfo)
    {
        var startupInfo = new WinApi.STARTUPINFOA();
        var ret = WinApi.CreateProcessA(
            null,
            args,
            IntPtr.Zero,
            IntPtr.Zero,
            0,
            ((DWORD)WinApi.CreationFlags.CREATE_NEW_CONSOLE) | ((DWORD)WinApi.CreationFlags.CREATE_NEW_PROCESS_GROUP),
            IntPtr.Zero,
            null,
            ref startupInfo,
            out processInfo);
        WinApi.CloseHandle(processInfo.hThread);
        return 1 == ret;
    }

    public static HANDLE CreateEvent(string guid)
    {
        return WinApi.CreateEventA(IntPtr.Zero, 0, 0, guid);
    }

    public static HRESULT WaitFor(HANDLE hProcess, HANDLE hEvent, DWORD dwTimeoutInMs)
    {
        UInt64 timeoutAt = WinApi.GetTickCount64() + dwTimeoutInMs;
        while (WinApi.GetTickCount64() < timeoutAt)
        {
            DWORD exitcode;
            var br = WinApi.GetExitCodeProcess(hProcess, out exitcode);
            if (1 != br) {
                return WinApi.E_FAIL;
            }
            if (WinApi.STILL_ACTIVE != exitcode) {
                return WinApi.ERROR_SIGNAL_REFUSED;
            }

            var hr = WinApi.WaitForSingleObject(hEvent, 500);
            if (WinApi.WAIT_TIMEOUT != hr) {
                return hr;
            }
        }
        return WinApi.WAIT_TIMEOUT;
    }
}

}
