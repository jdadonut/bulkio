using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;
namespace bulkio
{
    class Downloader
    {
        private IEnumerable<string> uriList;
        private int maxThreads;
        private bool endOnFail;
        public Downloader(IEnumerable<string> uriList, int maxThreads = 5, bool ignoreFails = false)
        {
            Console.WriteLine();
            this.uriList = uriList;
            this.maxThreads = maxThreads;
            this.endOnFail = !ignoreFails;
        }
        public async Task DownloadAsync(string outdir)
        {
            HttpClient client = new HttpClient();
            List<Task> Tasks = new List<Task> {};
            foreach (string url in this.uriList)
                Tasks.Add(new DownloadTask(url, client).Download(outdir));
            Task.WaitAll(Tasks.ToArray());

        }
    }
    class DownloadTask
    {
        HttpClient httpAgent;
        Uri uri;
        public DownloadTask(string uri, HttpClient httpAgent)
        {
            this.httpAgent = httpAgent;
            this.uri = new Uri(uri);
        }
        public async Task Download(string folder)
        {
            HttpResponseMessage res =  await httpAgent.GetAsync(this.uri);
            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine("Download failed on URI " + uri.AbsoluteUri );
                return;
            }
            byte[] bytes = await res.Content.ReadAsByteArrayAsync();
            string hash = BitConverter.ToString(MD5.Create().ComputeHash(bytes));
            string filename = hash + Path.GetExtension(uri.AbsolutePath);
            if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(folder, filename)).ToString()))
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(folder, filename)).ToString());
            File.WriteAllBytes(Path.Combine(folder, filename), bytes);
            Console.Write("\rExported image " + filename);
            
            
        }
        
    }
}