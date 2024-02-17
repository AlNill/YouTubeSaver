

using YoutubeExplode;

internal class Program
{
    static async Task Main(string[] args)
    {
        string outDir = @"D:\ProjectTestsFolder";

        List<string> urls = new List<string>()
        {
            //Insert urls there
            "", ""
        };

        try
        {
            foreach (var videoUrl in urls)
            {
                await DownloadYouTubeVideo(videoUrl, outDir);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while downloading the videos: " + ex.Message);
        }
    }

    static async Task DownloadYouTubeVideo(string url, string outDir)
    {
        var youtube = new YoutubeClient();
        var video = await youtube.Videos.GetAsync(url);

        string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
        var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

        if (muxedStreams.Any())
        {
            var streamInfo = muxedStreams.First();
            using var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(streamInfo.Url);
            var datetime = DateTime.Now;

            string outputFilePath = Path.Combine(outDir, $"{sanitizedTitle}.{streamInfo.Container}");
            using var outputStream = File.Create(outputFilePath);
            await stream.CopyToAsync(outputStream);

            Console.WriteLine("Download completed!");
            Console.WriteLine($"Video saved as: {outputFilePath}{datetime}");
        }
        else
        {
            Console.WriteLine($"No suitable video stream found for {video.Title}.");
        }
    }
}