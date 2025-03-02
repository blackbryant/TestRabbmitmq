

using RestSharp;

public class HealthManageExample
{
    public static void Main(string[] args)
    {
         var client = new RestClient("http://172.19.156.99:15672/api/");
        var request = new RestRequest("overview", Method.Get);
        request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("guest:guest")));

        var response = client.Execute(request);
        Console.WriteLine(response.Content);

        Console.ReadLine();
    }
}