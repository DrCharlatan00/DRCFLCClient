using System;
using System.IO;

class EnvReader
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
#warning Change
            //Change !!!
            string ContentBase = @"IP_SERV = 192.168.1.70
PORT = 5000";
            File.WriteAllText(filePath,ContentBase);
            Console.WriteLine("Please Setup .env File");
            throw new FileNotFoundException($"The file '{filePath}' does not exist.");
        }
        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue; // Skip empty lines and comments

            var parts = line.Split('=', 2);
            if (parts.Length != 2)
                continue; // Skip lines that are not key-value pairs

            var key = parts[0].Trim();
            var value = parts[1].Trim();
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}

