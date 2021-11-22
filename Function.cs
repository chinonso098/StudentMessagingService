using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;


using Amazon;
using System;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace StudentMessagingService
{

    public class Function
    {

        // Replace sender@example.com with your "From" address.
        // This address must be verified with Amazon SES.
        static readonly string senderAddress = "colam.aws.test@outlook.com";

        // Replace recipient@example.com with a "To" address. If your account
        // is still in the sandbox, this address must be verified.
        static readonly string receiverAddress = "chinonso1234@gmail.com";

        // The configuration set to use for this email. If you do not want to use a
        // configuration set, comment out the following property and the
        // ConfigurationSetName = configSet argument below. 
        //static readonly string configSet = "ConfigSet";

        // The subject line for the email.
        static readonly string subject = "Amazon SES test (AWS SDK for .NET)";

        // The email body for recipients with non-HTML email clients.
        static readonly string textBody = "Amazon SES Test (.NET)\r\n" 
                                        + "This email was sent through Amazon SES "
                                        + "using the AWS SDK for .NET.";
        
        // The HTML body of the email.
        static readonly string htmlBody = @"<html>
                                                <head></head>
                                                <body>
                                                <h1>Amazon SES Test (AWS SDK for .NET)</h1>
                                                <p>This email was sent with
                                                    <a href='https://aws.amazon.com/ses/'>Amazon SES</a> using the
                                                    <a href='https://aws.amazon.com/sdk-for-net/'>
                                                    AWS SDK for .NET</a>.</p>
                                                </body>
                                            </html>";

        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> GetCallingIP()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

            var msg = await client.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext:false);

            return msg.Replace("\n","");
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {

            //var location = await GetCallingIP();

            var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast2);

            var sendRequest = new SendEmailRequest
            {
                Source = senderAddress,
                Destination = new Destination
                {
                    ToAddresses =
                    new List<string> { receiverAddress }
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = htmlBody
                        },
                        Text = new Content
                        {
                            Charset = "UTF-8",
                            Data = textBody
                        }
                    }
                },
                // If you are not using a configuration set, comment
                // or remove the following line 
                //ConfigurationSetName = configSet
            };


            try
            {
                Console.WriteLine("Sending email using Amazon SES...");
                var response = await client.SendEmailAsync(sendRequest);
                //Console.WriteLine("The email was sent successfully.");


                var body = new Dictionary<string, string>
                {
                    { "message", "The email was sent successfully." },
                    { "location", " " }
                };

                return new APIGatewayProxyResponse
                {
                    Body = JsonSerializer.Serialize(body),
                    StatusCode = 200,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("The email was not sent.");
                Console.WriteLine("Error message: " + ex.Message);

                var body = new Dictionary<string, string>
                {
                    { "message", "The email was not sent." },
                    { "Error:", ex.Message }
                };

                return new APIGatewayProxyResponse
                {
                    Body = JsonSerializer.Serialize(body),
                    StatusCode = 500,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };

            }

        }
    }
}
