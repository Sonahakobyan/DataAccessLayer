using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = ""; //@"Data Source=LAPTOP-FUK5GBF7\SQLEXPRESS;Initial Catalog=Market;Integrated Security=true";
            
            DataService dataService = new DataService(connectionString);
            Dictionary<String, Object> parameters = new Dictionary<string, object>
            {
                { "csID", 1 }
            };

            IEnumerable<CategoryModel> categories = dataService.GetData<CategoryModel>("GetCategory", parameters);
            //IEnumerable<CategoryModel> categories = dataService.GetData<CategoryModel>("GetCategories", null);
            foreach (var category in categories)
            {
                Console.WriteLine(category);
            }

            Console.Read();
        }
    }
}
