

using RestSharp;

/// <summary>
/// The <c>HealthManageExample</c> class provides an example of how to use the RestSharp library
/// to make a GET request to a RabbitMQ management API endpoint and print the response content.
/// </summary>
public class HealthManageExample
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">An array of command-line argument strings.</param>
    public static void Main(string[] args)
    {
        // Creates a new RestClient instance with the base URL of the RabbitMQ management API.
        var client = new RestClient("http://172.19.156.99:15672/api/");
        
        // Creates a new RestRequest instance for the "overview" endpoint using the GET method.
        var request = new RestRequest("overview", Method.Get);
        
        // Adds an Authorization header with Basic authentication using the guest credentials.
        request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("guest:guest")));
        
        // Executes the request and stores the response.
        var response = client.Execute(request);
        
        // Prints the content of the response to the console.
        Console.WriteLine(response.Content);
        
        // Waits for the user to press Enter before closing the console window.
        Console.ReadLine();
    }
}