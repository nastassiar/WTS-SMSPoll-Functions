using System;
using System.Web;
using System.Web.Http;
using System.Net;

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    string restMethod = "GET";
    string urlPath = string.Format("https://m-swali-hrd.appspot.com/api/cms/msglog?auth=JXEIUOVNQLKJDDHA2J&eid=2439502&password=l73ofp4c99pmv9s&source=5&since=1498055474&until=1498056074&page=0&max=25");
    Uri uri = new Uri(urlPath);
    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
    request.Method = restMethod;

    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
    {
        Console.WriteLine($"Executed Call: {response.StatusCode}");

        Stream Answer = response.GetResponseStream();
        StreamReader _Answer = new StreamReader(Answer);

        log.Info($"Got This: {_Answer.ReadToEnd()}");
        _Answer.Close();
    }
}
