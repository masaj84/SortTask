using AltiumTask.Utils;
using System.Text;

public class CreateOutput
{
    static Random random = new Random();
    static HashSet<int> currentNumeberList = new HashSet<int>();
    static string[] sampleStrings = { "Apple", "Banana is yellow", "Cherry is the best", "Something something something", "Blueberry muffin", "Home", "Red car", "Pineapple on pizza", "Titanic is sinking" };
    private const int maxNumber = 100000000;

    public void CreateTextFile(int fileSizeInMB, string filePath, string fileName)
    {
        if (fileSizeInMB > 0 && filePath != null)
        {
            Util.CreatePath(filePath);
            Util.CreatePath($"{filePath}/tmp");
            GenerateFile(fileSizeInMB, $"{filePath}/{fileName}");
        }
    }

    private void GenerateFile(int fileSize, string filePath)
    {
        var maxFileSizeInBytes = fileSize * 1024 * 1024;
        var currFileSize = 0;

        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            while (currFileSize <= maxFileSizeInBytes)
            {
                var randomString = GenerateRandomLine();
                byte[] stringBytes = Encoding.UTF8.GetBytes(randomString + Environment.NewLine);

                if (currFileSize + stringBytes.Length > maxFileSizeInBytes)
                {
                    Console.WriteLine("Test file created.");
                    break; // Jeœli przekroczymy maksymalny rozmiar, przerywamy zapis
                }
                fs.Write(stringBytes, 0, stringBytes.Length);
                currFileSize += stringBytes.Length;
            }
        }
    }

    private string GenerateRandomLine()
    {
        var randomNumber = GenerateNewLineNo();
        string randomString = sampleStrings[random.Next(sampleStrings.Length)];
        return $"{randomNumber}.{randomString}";
    }

    private int GenerateNewLineNo()
    {
        int number = 0;
        do
        {
            number = random.Next(0, maxNumber);
        } while (currentNumeberList.Contains(number));

        currentNumeberList.Add(number);
        return number;
    }
}