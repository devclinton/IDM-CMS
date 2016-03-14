if NOT EXIST merged mkdir merged
"C:\Program Files (x86)\Microsoft\ILMerge\ILMerge.exe" /ndebug /keyfile:DEVELOPMENT.snk /lib:C:\Windows\Microsoft.NET\Framework\v4.0.30319 /out:merged\IronScheme.dll IronScheme.dll IronScheme.Closures.dll IronScheme.Scripting.dll Oyster.IntX.dll ironscheme.boot.dll 
REM copy /Y merged\IronScheme.* .
"c:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\x64\PEVerify.exe" /nologo /ignore=0x80131820,0x801318DE,0x80131854,0x8013185D,0x80131228 merged\IronScheme.dll