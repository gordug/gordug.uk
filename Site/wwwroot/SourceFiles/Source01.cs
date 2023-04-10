using System.Text;

internal static class InputScrambler
{
    private static readonly char[] AcceptedCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    static InputScrambler()
    {
        AcceptedCharacters = AcceptedCharacters.Concat(" !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~".ToCharArray()).ToArray();
    }

    public static void StartScramble()
    {
        ConsoleKeyInfo key;
        var characterIndex = 0;
        var sb = new StringBuilder();
        do
        {
            key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Escape)
            {
                var index = Array.IndexOf(AcceptedCharacters, key.KeyChar);
                if (index == -1)
                {
                    continue;
                }

                switch (key.Key)
                {
                    case ConsoleKey.Backspace when characterIndex > 0:
                        Console.Write("\b \b");
                        characterIndex--;
                        sb.Remove(sb.Length - 1, 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        sb.AppendLine();
                        break;
                    default:
                        Console.Write(AcceptedCharacters[(index + characterIndex) % AcceptedCharacters.Length]);
                        characterIndex++;
                        sb.Append(AcceptedCharacters[(index + characterIndex) % AcceptedCharacters.Length]);
                        break;
                }
            }
        } while (key.Key != ConsoleKey.Escape);

        Console.Clear();
        Console.WriteLine(sb.ToString());

        if (Confirm("Do you want to decode the string? (y/n)"))
        {
            Console.WriteLine("Enter the string to decode:");
            var input = Console.ReadLine();
            if (input is { })
                Console.WriteLine(Decode(input));
        }

        if (Confirm("Do you want to save the string? (y/n)"))
        {
            Console.WriteLine("Enter the path to save the string:");
            string? path;
            do
            {
                path = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                try
                {
                    File.WriteAllText(path, sb.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine($@"An error occured while saving the file: {e.Message}");
                }
            } while (string.IsNullOrWhiteSpace(path));
        }

        static bool Confirm(string message)
        {
            Console.WriteLine(message);
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Y:
                        return true;
                    case ConsoleKey.N:
                        return false;
                    default:
                        continue;
                }
            } while (key != ConsoleKey.Escape);

            return false;
        }

        static string Decode(string input)
        {
            var sb = new StringBuilder();
            var characterIndex = 0;
            foreach (var index in input.Select(c => Array.IndexOf(AcceptedCharacters, c)).Where(index => index != -1))
            {
                var i = (index - characterIndex++) % AcceptedCharacters.Length;
                if (i >= 0 && i < AcceptedCharacters.Length) sb.Append(AcceptedCharacters[i]);
            }

            return sb.ToString();
        }
    }
}