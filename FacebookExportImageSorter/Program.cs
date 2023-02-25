
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
