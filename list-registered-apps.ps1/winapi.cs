using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

// https://docs.microsoft.com/en-us/dotnet/standard/native-interop/best-practices
// https://docs.microsoft.com/en-us/dotnet/framework/interop/default-marshaling-for-strings

// void SHChangeNotify(
//                  LONG    wEventId,
//                  UINT    uFlags,
//   [in, optional] LPCVOID dwItem1,
//   [in, optional] LPCVOID dwItem2
// );

// LWSTDAPI SHLoadIndirectString(
//   [in]  PCWSTR pszSource,
//   [out] PWSTR  pszOutBuf,
//   [in]  UINT   cchOutBuf,
//         void   **ppvReserved
// );

namespace My
{

using HRESULT = UInt32;

public class WinApi
{
  [DllImport("shlwapi.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, ExactSpelling = true)]
  internal static extern HRESULT SHLoadIndirectString(
    String pszSource,
    IntPtr pszOutBuf, // no "[Out]" because it's written inplace, no "out" because it's pass by value (pointer value)
    UInt32 cchOutBuf,
    IntPtr ppvReserved);
}

public class Spec
{
  public static string LoadIndirectString(string s) {
    var buf = new char[32768];
    var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buf, 0);
    var hr = WinApi.SHLoadIndirectString(s, ptr, (UInt32)buf.Length, IntPtr.Zero);
    if (0 == hr) {
      var sz = Array.FindIndex(buf, c => c == '\0');
      return new string(buf, 0, sz);
    }
    else {
      Console.Error.WriteLine(String.Format("HRESULT={0:x} LastWin32Error={1:x}",
        hr,
        Marshal.GetLastWin32Error()));
      return "";
    }
  }
}

}
