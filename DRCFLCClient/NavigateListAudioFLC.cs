namespace DRCFLCClient;

public static class NavigateListAudio
{
    public static string? NavigateList<T>(IList<T> items)
    {
        if (items.Count == 0)
        {
            Console.WriteLine("List Is Null.");
            return "";
        }

        int currentIndex = 0;
        bool exit = false;

        while (!exit)
        {
            // Очистка и вывод текущего элемента
            Console.Clear();
            Console.WriteLine($"[{currentIndex + 1} / {items.Count}] {items[currentIndex]}");
            Console.WriteLine("\n↑/↓ — Navigate, Enter — Select, Esc — Exit");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    currentIndex = (currentIndex - 1 + items.Count) % items.Count;
                    break;
                case ConsoleKey.DownArrow:
                    currentIndex = (currentIndex + 1) % items.Count;
                    break;
                case ConsoleKey.Enter:
                    // Можно вернуть выбранный элемент или вызвать callback
                    Console.WriteLine($"\n Select: {items[currentIndex]}");
                    return items[currentIndex] as string;
                    break;
                case ConsoleKey.Escape:
              
                    Console.WriteLine("\n Exit I mode");
                    Console.Clear();    
                    foreach (var Audio in items)
                    {
                        Console.WriteLine(Audio);
                    }
                    exit = true;
                    
                    
                    break;
            }
        }
        return null;
    }
}