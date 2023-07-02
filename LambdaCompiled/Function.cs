using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SwadeshComp;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public CompiledTable FunctionHandler(APIGatewayHttpApiV2ProxyRequest input, ILambdaContext context)
    {
        string urls = input.QueryStringParameters["urls"];
        if (string.IsNullOrEmpty(urls)) return new CompiledTable();
        return Scraper.Compiled(urls.Split('~'));
    }
}
