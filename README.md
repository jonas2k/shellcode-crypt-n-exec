# shellcode-crypt-n-exec

This application is a proof of concept tool in order to demonstrate a way to hide shellcode execution from getting detected by dynamic analysis techniques in Windows binaries written in C#. It XOR-encrypts the input shellcode with an user defined password and compiles the resulting binary using certain templates containing evasion activities at runtime. It got developed as part of the "IT-Analyst" bachelor thesis at HS-KL University of applied sciences Zweibrücken/Germany.
