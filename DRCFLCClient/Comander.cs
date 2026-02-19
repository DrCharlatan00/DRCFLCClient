using System.Data;

namespace DRCFLCClient;

public class Comander
{
    public enum EnumAnswersReturnCommander
    {
        IsOk = 0,
        Error = -1,
        Skip = -2
    }
    
    public static (EnumAnswersReturnCommander Answer, string? Messange) CommanderExec(EnumParamtr? Command)
    {
        if (Command is null || Command is not Enum)
        {
            return (EnumAnswersReturnCommander.Skip,"Skip. Is null");
        }

        switch (Command)
        {
            case EnumParamtr.REMOVE_FILES: 
                string[]? flacFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.flac");
                if (flacFiles is null || flacFiles.Length == 0)
                {
                    return (EnumAnswersReturnCommander.Skip, "Files is null");
                }
                try
                {
                    foreach (var item in flacFiles)
                    {
                        Console.WriteLine($"Remove Files: {item}");
                        File.Delete(item); 
                    }

                    return (EnumAnswersReturnCommander.IsOk, null);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cancel delete files: Files in use");
                    Console.ForegroundColor = ConsoleColor.White;
                    return (EnumAnswersReturnCommander.Error,"Catch, File is use another process or unexpected error ");
                }
                //break;
            
                
        }
      

        return (EnumAnswersReturnCommander.Skip,"Skip");

    }
}