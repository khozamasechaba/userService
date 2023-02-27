using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace userService
{
    public class Function
    {
        public string? userName;
        public string? Name;
        public string? Surname;
        private ServiceCollection _serviceCollection;
        private ServiceProvider _serviceProvider;
        private IUserService _userService;
        private IDatabaseAccess _databaseAccess;
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// Credit is given to Jevan Naidoo for this section of code : jevann@ozow.com
        /// </summary>
        public Function()
        {
            _serviceProvider = ConfigureServices();
            _userService = _serviceProvider.GetService<IUserService>();
        }
        /// <summary>
        /// Registering dependencies
        /// </summary>
        /// <returns></returns>
        /// Credit Provided to Jevan Naidoo for this section of code
        private ServiceProvider ConfigureServices()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddTransient<IUserService, UserService>();
            _serviceCollection.AddTransient<IDatabaseAccess, DatabaseAccess>();
            return _serviceCollection.BuildServiceProvider();
        }
        //Credit Provided to Jevan Naidoo for this section of code
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
          
            foreach (var message in evnt.Records)
            {
               
                await ProcessMessageAsync(message, context);
            }
            using (ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider())
            {
                
            }

        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            context.Logger.LogInformation($"{message.Body}");

            string data = message.Body.ToString();

            Student? jObj = JsonConvert.DeserializeObject<Student>(data);

            string? uname = jObj.username;
            Console.WriteLine("You have entered the following details: ");
            Console.WriteLine($"Username: {uname}");

            string? name = jObj.name;

           Console.WriteLine($"Name: {name}");

            string? sname= jObj.surname;

           Console.WriteLine($"Surname: {sname}");
       
            _userService.addToDb(uname, name, sname);

            await Task.CompletedTask;
        }
    }
}
