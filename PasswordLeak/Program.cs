using System;
using System.Net.Http; //NuGet install !!!!
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordLeak
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();

            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = sha1.ComputeHash(passwordBytes);

                string passwordSha1 = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                using (HttpClient client = new HttpClient())
                {
                    string range = passwordSha1.Substring(0, 5);
                    string apiUrl = $"https://api.pwnedpasswords.com/range/{range}";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    string responseText = await response.Content.ReadAsStringAsync();

                    foreach (var line in responseText.Split('\n'))
                    {
                        string[] parts = line.Split(':');
                        string hash = parts[0];
                        string count = parts[1];

                        if (hash.ToLower() == passwordSha1.Substring(5))
                        {
                            Console.WriteLine($"Found: {count}");
                            return;
                        }
                    }

                    Console.WriteLine("Good password");
                }
            }
        }
    }
}