using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Appwrite;

namespace playground_for_dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.SetEndPoint("[ENDPOINT]");
            client.SetProject("[PROJECT_ID]");
            client.SetKey("[API_KEY]");
            client.SetSelfSigned(true);

            string response;
            string collection;
            JObject parsed;

            Database database = new Database(client);
            Users users = new Users(client);

            /**
                Create User
            */
            try
            {
                Console.WriteLine("Running Create Users API");
                RunTask(users.Create($"{DateTime.Now.ToFileTime()}@example.com", "*******", "Lorem Ipsum")).GetAwaiter().GetResult();
                Console.WriteLine("Done");
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                List Documents
            */
            try
            {
                Console.WriteLine("Running List Documents API");
                response = RunTask(users.List()).GetAwaiter().GetResult();
                parsed = JObject.Parse(response);
                foreach (dynamic element in parsed["users"])
                {
                    Console.WriteLine($"- {element["name"]} ({element["email"]})");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                Create Collection
            */
            List<object> perms = new List<object>() {"*"};
            List<object> rules = new List<object>();

            Rule ruleName = new Rule();
            ruleName.Label = "Name";
            ruleName.Key = "name";
            ruleName.Type = "text";
            ruleName.Default = "Empty Name";
            ruleName.Required = true;
            ruleName.Array = false;
            rules.Add(ruleName);

            Rule ruleYear = new Rule();
            ruleYear.Label = "Release Year";
            ruleYear.Key = "release_year";
            ruleYear.Type = "numeric";
            ruleYear.Default = "1970";
            ruleYear.Required = true;
            ruleYear.Array = false;
            rules.Add(ruleYear);

            try
            {
                Console.WriteLine("Running Create Collection API");
                response = RunTask(database.CreateCollection("Movies", perms, perms, rules)).GetAwaiter().GetResult();
                parsed = JObject.Parse(response);
                collection = (string) parsed["$id"];
                Console.WriteLine("Done");
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                List Collection
            */
            try
            {
                Console.WriteLine("Running List Collection API");
                response = RunTask(database.ListCollections()).GetAwaiter().GetResult();
                parsed = JObject.Parse(response);
                foreach (dynamic element in parsed["collections"])
                {
                    Console.WriteLine($"- {element["name"]}");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                Add Document
            */
            Movie movie1 = new Movie("Alien", 1979);
            Movie movie2 = new Movie("Equilibrium", 2002);
            try
            {
                Console.WriteLine("Running Create Documents API");
                RunTask(database.CreateDocument(collection, movie1, perms, perms)).GetAwaiter().GetResult();
                RunTask(database.CreateDocument(collection, movie2, perms, perms)).GetAwaiter().GetResult();
                Console.WriteLine("Done");
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                List Documents
            */
            try
            {
                Console.WriteLine("Running List Documents API");
                response = RunTask(database.ListDocuments(collection)).GetAwaiter().GetResult();
                parsed = JObject.Parse(response);
                foreach (dynamic element in parsed["documents"])
                {
                    Console.WriteLine($"- {element["name"]} ({element["release_year"]})");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }
        }

        static async Task<string> RunTask(Task<HttpResponseMessage> task) 
        {
            HttpResponseMessage response = await task;
            return await response.Content.ReadAsStringAsync();
        }
    }
    public class Movie {
        public Movie(string name, int releaseYear)
        {
            Name = name;
            release_year = releaseYear;
        }
        public string Name { get; }
        public int release_year { get; }

    }
}
