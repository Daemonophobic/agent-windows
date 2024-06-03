using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WINREM
{
    internal class Program
    {
        private static byte[] password = new byte[] { 187, 211, 40, 42, 46, 113, 15, 154, 236, 228, 44, 170, 158, 63, 229, 23, 20, 226, 136, 171, 219, 19, 246, 139, 57, 100, 13, 96, 140, 138, 209, 113, 246, 62, 41, 41, 70, 138, 119, 125, 38, 225, 145, 188, 146, 164, 160, 48, 234, 130, 57, 114, 105 };
        private static byte[] key = new byte[] { 0xd9, 0xb2, 0x4b, 0x41, 0x5d, 0x05, 0x6e, 0xfd, 0x89, 0xc4, 0x41, 0xdf, 0xed, 0x4b, 0x80, 0x65, 0x34, 0x90, 0xed, 0xc6, 0xb4, 0x67, 0x93, 0xab, 0x5d, 0x16, 0x62, 0x16, 0xe9, 0xaa, 0xa1, 0x03, 0x93, 0x5d, 0x46, 0x46, 0x2d, 0xaa, 0x1e, 0x10, 0x56, 0x93, 0xfe, 0xd1, 0xe2, 0xd0, 0xd5, 0x10, 0x98, 0xed, 0x5a, 0x19, 0x55, 0x1b, 0x1e, 0x88, 0x3e, 0x41, 0x31, 0x92, 0x22, 0x70, 0x9c, 0x1a };
        static void Main(string[] args)
        {
            
            AppDomain.CurrentDomain.ProcessExit += ProccessExitHandler;
            string SecureFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\win.rem";
            try
            {
                var decryptedPassword = xOrPassword(password, key);
                byte[] Paskey = new Rfc2898DeriveBytes(decryptedPassword, Encoding.UTF8.GetBytes("SecretValue")).GetBytes(32);
                byte[] iv = new Rfc2898DeriveBytes(decryptedPassword, Encoding.UTF8.GetBytes("SecretValue")).GetBytes(16);



                if (args.Length == 0)
                {
                    string GetPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    bool StorageFileExist = System.IO.File.Exists(GetPath + "\\win.rem");
                    if (StorageFileExist)
                    {
                        using (AesManaged aes = new AesManaged())
                        {
                            aes.Key = Paskey;
                            aes.IV = iv;
                            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                            var apiKey = DecryptStorageFile(SecureFile, decryptor);
                            Worker(apiKey);
                        }

                    }
                    else
                    {
                        PrintHelp();
                    }
                    return;
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == "--help")
                        {
                            PrintHelp();
                            return;
                        }
                        else if (args[i] == "--key")
                        {
                            if (i + 1 < args.Length)
                            {
                                string apiKey = args[i + 1];
                                using (AesManaged aes = new AesManaged())
                                {
                                    aes.Key = Paskey;
                                    aes.IV = iv;
                                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                                    GenerateStorageFile(SecureFile, apiKey, encryptor);
                                    Worker(apiKey);
                                }
                                return;
                            }
                            else
                            {
                                throw new ArgumentNullException("API Key is missing");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        private static string xOrPassword(byte[] Password, byte[] key)
        {
            for (int i = 0; i < Password.Length - 1; i++)
            {
                Password[i] = (byte)(Password[i] ^ key[i]);
            }
            return Encoding.UTF8.GetString(Password);
        }
        static void GenerateStorageFile(string outputFile, string apikey, ICryptoTransform encryptor)
        {
            string tempFile = Path.GetTempFileName();
            using(var sw = new StreamWriter(tempFile, true))
            {
                sw.WriteLine(apikey);
            }

            using(FileStream inputFile = new FileStream(tempFile, FileMode.Open))
            {
                using(FileStream outPutFile = new FileStream(outputFile, FileMode.Create))
                {
                    using(CryptoStream cryptoStream = new CryptoStream(outPutFile, encryptor, CryptoStreamMode.Write))
                    {
                        int data;
                        while((data = inputFile.ReadByte()) != -1)
                        {
                            cryptoStream.WriteByte((byte)data);
                        }
                    }
                }
            }
            File.Delete(tempFile);
        }

        static string DecryptStorageFile(string inputFile, ICryptoTransform decryptor)
        {
            string tempFile = Path.GetTempFileName();
            using (FileStream inputFileEncrypted = new FileStream(inputFile, FileMode.Open))
            {
                using(FileStream outPutFile = new FileStream(tempFile, FileMode.Create))
                {
                    using(CryptoStream cryptoStream = new CryptoStream(outPutFile, decryptor, CryptoStreamMode.Write))
                    {
                        int data;
                        while((data = inputFileEncrypted.ReadByte()) != -1)
                        {
                            cryptoStream.WriteByte((byte)data);
                        }
                    }
                }
            }


            string result = "";
            using(var sr = new StreamReader(tempFile))
            {
                result = sr.ReadToEnd();
            }
            File.Delete(tempFile);
            return result;
        }

        static void Worker(string apiKey)
        {
            ApiProvider apiProvider = new ApiProvider();
            bool Online=true;
            while (Online)
            {
                Random random = new Random();
                int NextRun = random.Next(120000, 300000);
                apiProvider.CheckForJob(apiKey, key).Wait();
                System.Threading.Thread.Sleep(NextRun);
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("HELP:");
            Console.WriteLine("------------------");
            Console.WriteLine("Usage: winrem [options]");
            Console.WriteLine("------------------");
            Console.WriteLine("\t--help\t\tDisplay this help message");
            Console.WriteLine("\t--key [API-KEY]\tEnable verbose mode - This is only need on first time setup or on moving of the application");
        }

        public static void ProccessExitHandler(object sender , EventArgs e)
        {
            
        }
    }
}
