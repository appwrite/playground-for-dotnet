using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Services;
using Appwrite.Models;
using Newtonsoft.Json;

namespace playground_for_dotnet
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Client client = new Client();
            client.SetEndpoint("[ENDPOINT]");
            client.SetProject("[PROJECT_ID]");
            client.SetKey("[API_KEY]");

            Databases databases = new Databases(client);

            Database database;
            Collection collection;

            /**
                Create Database
            */
            try
            {
                Console.WriteLine("Running Create Database API");
                database = await databases.Create(
                    databaseId: ID.Unique(),
                    name: "MoviesDB"
                );
                Console.WriteLine("Done");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                Create Collection
            */

            try
            {
                Console.WriteLine("Running Create Collection API");
                collection = await databases.CreateCollection(
                    databaseId: database.Id,
                    collectionId: ID.Unique(),
                    name: "Movies",
                    permissions: new List<string> { Permission.Read(Role.Any()), Permission.Write(Role.Any()) }
                );
                Console.WriteLine("Done");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }

            /**
                List Collections
            */
            try
            {
                Console.WriteLine("Running List Collection API");
                var collectionsList = await databases.ListCollections(
                    databaseId: database.Id
                );
                foreach (var element in collectionsList.Collections)
                {
                    Console.WriteLine($"- {element.Name}");
                }
            }
            catch (Exception e)
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
                await databases.CreateDocument(
                    databaseId: database.Id,
                    collectionId: collection.Id,
                    documentId: ID.Unique(),
                    data: movie1
                );
                await databases.CreateDocument(
                    databaseId: database.Id,
                    collectionId: collection.Id,
                    documentId: ID.Unique(),
                    data: movie2
                );
                Console.WriteLine("Done");
            }
            catch (Exception e)
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
                var documentsList = await databases.ListDocuments(
                    databaseId: database.Id,
                    collectionId: collection.Id
                );
                foreach (var element in documentsList.Documents)
                {
                    var movie = JsonConvert.DeserializeObject<Movie>(JsonConvert.SerializeObject(element));
                    Console.WriteLine($"- {movie.Name} ({movie.release_year})");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                throw;
            }
        }
    }
    public class Movie
    {
        public Movie(string name, int releaseYear)
        {
            Name = name;
            release_year = releaseYear;
        }
        public string Name { get; }
        public int release_year { get; }

    }
}
