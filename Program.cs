using System.IO;
using System.Text;
using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

GenerateTotalSalesReport(salesFiles, salesTotalDir);



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

static void GenerateTotalSalesReport(IEnumerable<string> salesFiles, string outputDir)
{

    double total = 0;
    StringBuilder builder = new StringBuilder();

    builder.AppendLine("Total Sales Summary");
    builder.Append("-----------------");

    StringBuilder detailsBuilder = new StringBuilder();
    detailsBuilder.AppendLine("Details: ");

    foreach (string file in salesFiles)
    {
        string totals = File.ReadAllText(file);

        SalesData? sdata = JsonConvert.DeserializeObject<SalesData?>(totals);
        double fileTotal = sdata?.Total ?? 0;

        total += fileTotal;

        detailsBuilder.AppendLine($"{Path.GetFileName(file)},{fileTotal:c}");


    }

    builder.AppendLine($"Total Sales: {total:c}");
    builder.AppendLine();
    builder.Append(detailsBuilder.ToString());

    string reportPath = Path.Combine(outputDir, "salesSummary.txt");
    File.WriteAllText(reportPath, builder.ToString());
}



record SalesData(double Total);