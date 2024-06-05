using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

string[] excludedDirectories = new []{ "bin", "obj", ".git", "Properties" };
string[] excludedFiles = new []{ "Program.cs", "ToC.json" };

if (args.Length == 0)
{
    Console.WriteLine("No arguments provided.");
    return;
}

Console.WriteLine($"Running Table of Contents generation on {args[0]}");

// Get all .cs files in the directory
IEnumerable<string> csFiles = Directory.EnumerateFiles(args[0], "*.cs", SearchOption.AllDirectories);

// Root object to store namespace hierarchy
JObject root = new JObject();

// Read each file and extract namespaces and classes
foreach (string file in csFiles)
{
    string? namespaceDeclaration = null;
    string? className = null;

    string[] lines = File.ReadAllLines(file);
    foreach (string line in lines)
    {
        string trimmedLine = line.Trim();

        if (trimmedLine.StartsWith("namespace"))
        {
            namespaceDeclaration = trimmedLine.Replace("namespace", "").Trim();
        }
        else if ((trimmedLine.Contains(" class ") || trimmedLine.Contains(" interface ")) 
                 && (trimmedLine.Contains("public") || trimmedLine.Contains("internal") || trimmedLine.Contains("protected") || trimmedLine.Contains("private")))
        {
            className = trimmedLine.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (className.Trim().Equals("class"))
            {
                className = trimmedLine.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries)[3];
            }
            
            if (namespaceDeclaration != null && className != null)
            {
                InsertNamespace(root, namespaceDeclaration, className);
                break; // Assumes one namespace and one class per file, remove break if not
            }
        }
    }
}

// Remove the base object from the root
JObject finalJson = new JObject
{
    { "items", new JArray() }
};
JArray? items = (JArray?)finalJson["items"];

JObject? ecommerceObject = (JObject?)root["Ecommerce"];

foreach (JProperty? property in ecommerceObject.Properties())
{
    items.Add( new JObject
    {
        { property.Name, property.Value }
    });
}

ExpandoObjectConverter expConverter = new ExpandoObjectConverter();
dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(finalJson.ToString(), expConverter);


// Create a new Yaml serializer
ISerializer serializer = new SerializerBuilder()
    .WithNamingConvention(CamelCaseNamingConvention.Instance)
    .Build();

// Serialize the object to YAML
string yaml = serializer.Serialize(deserializedObject);

// Output all found namespaces
Console.WriteLine("Namespaces organized:");
//Console.WriteLine(root.ToString());
Console.WriteLine(yaml);
return;

void InsertNamespace(JObject root, string namespaceDeclaration, string className)
{
    string[] parts = namespaceDeclaration.Split('.');
    JObject current = root;

    foreach (string part in parts)
    {
        if (current[part] == null)
        {
            current[part] = new JObject();
        }
        current = (JObject)current[part];
    }

    current["uuid"] = namespaceDeclaration;
    current["name"] = namespaceDeclaration.Split('.').Last();
    
    // Adding full namespace and class
    JObject classInfo = new JObject();
    classInfo["uuid"] = className.StartsWith(namespaceDeclaration) ? className : namespaceDeclaration + "." + className;
    classInfo["name"] = className.StartsWith(namespaceDeclaration) ? className.Substring(className.LastIndexOf('.')) : className;
    
    if (current["items"] == null)
    {
        current["items"] = new JArray();
    }
    
    ((JArray)current["items"]).Add(classInfo);
}