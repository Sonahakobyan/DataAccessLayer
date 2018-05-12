using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace DataAccessLayer
{
    public class DataService : IDataService
    {
        private string connectionString;

        public DataService(string connectionString)
        {
            this.connectionString = connectionString;
        }
        
        public IEnumerable<T> GetData<T>(String code, Dictionary<String, Object> parameters)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\hakob\Desktop\C#\hw_11\DataAccessLayer\DataAccessLayer\DAL.xml");
            XmlNodeList queryNodes = doc.ChildNodes.Item(1).ChildNodes;

            XmlNode queryNode = null;

            foreach (XmlNode node in queryNodes)
            {
                if (node.Attributes["name"]?.Value == code)
                {
                    queryNode = node;
                    break;
                }
            }

            if (queryNode == null)
            {
                return null;
            }

            String command = queryNode.Attributes["command"]?.Value;
            Boolean isSP = queryNode.Attributes["issp"]?.Value == "true";
            Dictionary<String, String> mappedParameters = null;
            XmlNode parametersNode = queryNode["parameters"];
            if (parametersNode != null)
            {
                XmlNodeList childParams = parametersNode.ChildNodes;
                if (childParams != null)
                {
                    mappedParameters = new Dictionary<String, String>();

                    foreach (XmlNode node in childParams)
                    {
                        String key = node.Attributes["csname"]?.Value;
                        String value = node.Attributes["sqlname"]?.Value;

                        if (!String.IsNullOrWhiteSpace(key) && !String.IsNullOrWhiteSpace(value))
                        {
                            mappedParameters.Add(key, value);
                        }
                    }
                }
            }


            Dictionary<String, Object> sqlParams = null;

            if (parameters != null && parameters.Any() && mappedParameters != null && mappedParameters.Any())
            {
                sqlParams = parameters.ToDictionary(x => (mappedParameters.ContainsKey(x.Key) ? mappedParameters[x.Key] : x.Key), x => x.Value);
            }


            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                SqlCommand sqlCommand = new SqlCommand(command, connection)
                {
                    CommandType = isSP ? CommandType.StoredProcedure : CommandType.Text
                };

                if (isSP && sqlParams != null)
                {
                    foreach (KeyValuePair<string, object> param in sqlParams)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(param.Key, param.Value));
                    }
                }

                connection.Open();

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {

                    List<T> response = new List<T>();

                    // Get info about T
                    Type type = typeof(T);
                    PropertyInfo[] properties = type.GetProperties();
                    ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
                    // Init new instances of T
                    while (reader.Read())
                    {
                        T t = (T)ctor.Invoke(null); //Activator.CreateInstance(type);
                        foreach (var prop in properties)
                        {
                            prop.SetValue(t, reader[$"{prop.Name}"]);
                        }
                        response.Add(t);
                    }

                    return response;

                }
            }
        }
    }
}
