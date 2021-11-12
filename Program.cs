using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Models;

namespace PlaygroundForDotNet
{
    class Program
    {
        const string Endpoint = "http://localhost/v1";
        const string Key = "32b273c69d6f36fb7bd3b273150876966ade2721becbdf7b913a629664b4b176d79e06187d10a9e068ae018d720d991f99d22e3a29adfca30a220d233f6e412b773ccbe3aaaa4960a2382785a74406629a685367c37325dc016797ae73d68345caedfca10bbd8acd319e411f4cc0757b9c489a3e552c40443378349c4eeb1136";
        const string ProjectId = "618dd805ca196";


        static async Task Main(string[] args)
        {
            var client = new Client()
                .SetEndPoint(Endpoint)
                .SetProject(ProjectId)
                .SetKey(Key)
                .SetSelfSigned(true);

            var database = new Database(client);
            var users = new Users(client);
            var functions = new Functions(client);

            string collectionId;

            /**
                Create User
            */
            try
            {
                Console.WriteLine("Running Create Users API");
                var user = await users.Create($"{DateTime.Now.ToFileTime()}@example.com", "*******", "Lorem Ipsum");
                Console.WriteLine("Done");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                List Users
            */
            try
            {
                Console.WriteLine("Running List Documents API");
                var userList = await users.List();
                foreach (var user in userList.Users)
                {
                    Console.WriteLine($"- {user.Name} ({user.Email})");
                }
            }
            catch (AppwriteException e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                Create Collection
            */
            var perms = new List<object>() {"*"};
            var rules = new List<object>();

            var nameRule = new Rule(
                id: "",
                collection: "",
                type: "text",
                key: "name",
                label: "Name",
                xdefault: "Empty Name",
                required: true,
                array: false,
                list: null);

            rules.Add(nameRule);

            var yearRule = new Rule(
                id: "",
                collection: "",
                type: "text",
                key: "release_year",
                label: "Release Year",
                xdefault: "1970",
                required: true,
                array: false,
                list: null);

            rules.Add(yearRule);

            try
            {
                Console.WriteLine("Running Create Collection API");
                var collection = await database.CreateCollection("Movies", perms, perms, rules);
                collectionId = collection.Id;
                Console.WriteLine("Done");
            }
            catch (AppwriteException e)
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
                var collectionList = await database.ListCollections();
                
                foreach (var collection in collectionList.Collections)
                {
                    Console.WriteLine($"- {collection.Name}");
                }
            }
            catch (AppwriteException e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                Add Document
            */
            var movie1 = new Movie(
                name: "Alien",
                releaseYear: 1979);

            var movie2 = new Movie(
                name: "Equilibrium",
                releaseYear: 2002);

            try
            {
                Console.WriteLine("Running Create Documents API");
                await database.CreateDocument(collectionId, movie1, perms, perms);
                await database.CreateDocument(collectionId, movie2, perms, perms);
                Console.WriteLine("Done");
            }
            catch (AppwriteException e)
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
                var documentList = await database.ListDocuments(collectionId);
                foreach (var document in documentList.Documents)
                {
                    Console.WriteLine($"- {document.Data["name"]} ({document.Data["release_year"]})");
                }
            }
            catch (AppwriteException e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                List Functions
            */
            try
            {
                Console.WriteLine("Running List Functions API");
                var functionList = await functions.List();
                
                foreach (var function in functionList.Functions)
                {
                    Console.WriteLine($"- {function.Name} ({function.Runtime})");
                }
            }
            catch (AppwriteException e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }
        }
    }

    public class Movie
    {
        public string Name { get; }
        public int ReleaseYear { get; }

        public Movie(string name, int releaseYear)
        {
            Name = name;
            ReleaseYear = releaseYear;
        }
    }
}
