using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ShellcodeCryptNExec.Helpers {
    public static class Utils {

        private static readonly char[] repo = "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();
        private static Random random = new Random();

        public static String GetRandomString(int length) {
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < length; i++) {
                strBuilder.Append(repo[random.Next(repo.Length)]);
            }
            return strBuilder.ToString();
        }

        public static string Encrypt(List<byte> shellcode, byte[] key) {

            List<byte> output = new List<byte>();

            for (int i = 0; i < shellcode.Count; i++) {
                int result = shellcode[i] ^ key[i % key.Length];
                Debug.WriteLine("XORing " + shellcode[i] + " with " + key[i % key.Length] + " to " + result);
                output.Add((byte)result);
            }
            return Convert.ToBase64String(output.ToArray());
        }

        public static List<byte> ReadShellcodeFromFile(String filePath) {
            List<byte> shellcode = new List<byte>();
            List<string[]> lines = File.ReadLines(filePath)
                                    .Select(line => line.TrimEnd(','))
                                    .Select(line => line.Split(','))
                                    .ToList();

            foreach (string[] line in lines) {
                foreach (string element in line) {
                    shellcode.Add(Convert.ToByte(element, 16));
                }
            }
            return shellcode;
        }
    }
}