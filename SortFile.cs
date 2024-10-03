using AltiumTask.Utils;


namespace AltiumTask
{
    public class SortFile
    {
        private const string _tmpFilePath = "D://altium/tmp/";
        private const string _inFilePath = "D://altium/test.txt";
        private const string _outFilePath = "D://altium/test_result.txt";

        public async Task Sort()
        {
            await StartSortingOperationInParallell();
        }

        private async Task StartSortingOperationInParallell()
        {
            await ParallelSort();
        }

        private async Task ParallelSort()
        {
            var bufferSize = 100 * 1024 * 1024;

            var chunksList = new List<List<string>>();

            using (FileStream fs = new FileStream(_inFilePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[bufferSize]; 
                int bytesRead;
                string leftover = "";

                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string text = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    text = leftover + text;

                    string[] lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None);

                    leftover = lines[lines.Length - 1];

                    var chunk = new List<string>(lines[..^1]); 
                    chunksList.Add(chunk);
                }
            }


            var sortTasks = new List<Task<List<string>>>();
            foreach (var chunk in chunksList)
            {
                sortTasks.Add(Task.Run(() => SortChunk(chunk)));
            }
            List<string>[] sortedChunks = await Task.WhenAll(sortTasks);


            var saveTasks = new List<Task<string>>();
            foreach (var sortedChunk in sortedChunks)
            {
                saveTasks.Add(SaveChunkAsTmpFileAsync(sortedChunk)); 
            }
            string[] savedFiles = await Task.WhenAll(saveTasks);

            
            MergeSortedFiles(savedFiles, _outFilePath);
        }
               

        private List<string> SortChunk(List<string> currentChunk)
        {
            return currentChunk
                .OrderBy(item => SplitItem(item).Item2) 
                .ThenBy(item => int.Parse(SplitItem(item).Item1))
                .ToList();
        }


        public async Task<string> SaveChunkAsTmpFileAsync(List<string> sortedChunk)
        {
            int bSize = 2 * 1024 * 1024;
            string tmpFile = Path.Combine(_tmpFilePath, Path.GetRandomFileName());

            using (FileStream fs = new FileStream(tmpFile, FileMode.Create, FileAccess.Write, FileShare.None, bSize, useAsync: true))
            {
                var sb = new System.Text.StringBuilder();
                foreach (var line in sortedChunk)
                {
                    sb.AppendLine(line);
                }
                byte[] chunkBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());

                await fs.WriteAsync(chunkBytes, 0, chunkBytes.Length);
            }

            return tmpFile;
        }

        static Tuple<string, string> SplitItem(string line)
        {
            int dotIndex = line.IndexOf('.');
            string numberPart = line.Substring(0, dotIndex).Trim();
            string stringPart = line.Substring(dotIndex + 1).Trim();
            return Tuple.Create(numberPart, stringPart);
        }

        private void MergeSortedFiles(string[] sortedFiles, string outFilePath)
        {
            var readers = new List<StreamReader>();
            PriorityQueue<TmpFile, CustomPriority> priorityQueue = new PriorityQueue<TmpFile, CustomPriority>();

            try
            {
                for (int i = 0; i < sortedFiles.Length; i++)
                {
                    StreamReader sr = new StreamReader(sortedFiles[i]);
                    readers.Add(sr);
                    var line = sr.ReadLine();
                    var lineArr = line.Split('.');
                    var currLine = new CustomPriority(int.Parse(lineArr[0]), lineArr[1]);
                    if (line != null)
                    {
                        priorityQueue.Enqueue(new TmpFile { Line = line, Reader = sr }, currLine);
                    }
                }

                using (StreamWriter sw = new StreamWriter(outFilePath))
                {
                    while (priorityQueue.Count > 0)
                    {
                        TmpFile currItem = priorityQueue.Dequeue();
                        sw.WriteLine(currItem.Line);

                        string nextLine = currItem.Reader.ReadLine();
                        if (nextLine != null)
                        {
                            priorityQueue.Enqueue(new TmpFile { Line = nextLine, Reader = currItem.Reader }, ExtractPriority(nextLine));
                        }
                    }
                }
            }
            finally
            {
                foreach (var reader in readers)
                {
                    reader.Dispose();
                }
            }
        }

        private CustomPriority ExtractPriority(string line)
        {
            var currArr = line.Split('.');
            return new CustomPriority(int.Parse(currArr[0]), currArr[1]);
        }
    }
}