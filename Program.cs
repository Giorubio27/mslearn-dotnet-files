using System.IO;
using System.Text;
using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);
var salesSummary = GenerateTotalSalesReport(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

static void GenerateTotalSalesReport(IEnumerable<string> salesFiles)
{
    
    double total = 0;
    StringBuilder builder = new StringBuilder();

    builder.AppendLine("Total Sales Summary");
    builder.Append("-----------------");

    foreach (string file in salesFiles)
    {
        string totals = File.ReadAllText(file);

        SalesData? sdata = JsonConvert.DeserializeObject<SalesData?>(totals);
        total += sdata?.Total ?? 0;

        builder.AppendLine($"{Path.GetFileName(file)},{sdata:c}");


    }

    builder.Insert(0, $"Total: {total:c}\n");
}

record SalesData(double Total);