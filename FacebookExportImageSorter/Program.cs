
using Newtonsoft.Json;
using static FbModel;

const string DataPath = "D:/FacebookData";
const string TargetPath = "D:/FacebookDataProcessed";

var jsonFiles = Directory.GetFiles(DataPath, "*.json", SearchOption.AllDirectories)
    .Where(f => f.Contains("message_")).ToList();

var imageFiles = 
    Directory.GetFiles(DataPath, "*.jpg", SearchOption.AllDirectories).Union(
    Directory.GetFiles(DataPath, "*.gif", SearchOption.AllDirectories).Union(
    Directory.GetFiles(DataPath, "*.png", SearchOption.AllDirectories))).ToList();


var messages = new List<Message>();

foreach (var jsonFile in jsonFiles)
{
    var json = File.ReadAllText(jsonFile);
    var content = JsonConvert.DeserializeObject<FbModel>(json);
    var messagesWithPhotos = content.Messages.Where(m => m.Photos != null).ToList();
    messages.AddRange(messagesWithPhotos);
}

foreach (var message in messages)
{
    // datetime from message.timstamp
    var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    date = date.AddMilliseconds(message.Timestamp_Ms);

    var folderPath = @$"{TargetPath}/{date.Year}/";
    var fileName = $@"{date.Year}-{date.Month.ToString().PadLeft(2, '0')}-{date.Day.ToString().PadLeft(2, '0')}-{message.Timestamp_Ms}";

    var imageNr = 0;
    foreach (var image in message.Photos)
    {
        if (image.Uri.StartsWith("https://"))
            continue;

        var sourceAbsolutePath = $"{DataPath}/{image.Uri}";
        var extension = image.Uri[^4..];
        var targetAbsolutePath = $"{folderPath}/{fileName}-{(++imageNr).ToString().PadLeft(4, '0')}{extension}";

        Console.WriteLine(targetAbsolutePath);
        new FileInfo(targetAbsolutePath).Directory.Create();
        File.Copy(
            sourceAbsolutePath,
            targetAbsolutePath,
            overwrite: true);
    }
}




public class FbModel
{
    public List<Message> Messages { get; set; }

    public class Message
    {
        public long Timestamp_Ms { get; set; }
        public List<Photo> Photos { get; set; }

        public class Photo
        {
            public string Uri { get; set; }
        }
    }
}
