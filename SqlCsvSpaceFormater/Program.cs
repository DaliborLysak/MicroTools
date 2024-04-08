using System.Diagnostics;
using System.Text;

var arguments = Environment.GetCommandLineArgs();

if (arguments.Length != 2)
    throw new ArgumentException("Only argument path, as one parameter is allowed");

var source = arguments[1];

#if DEBUG
if (!File.Exists(arguments[1]))
    source = Path.GetDirectoryName(arguments[0])?.Replace("exe", arguments[1]) ?? string.Empty;
#endif

if (!File.Exists(source))
    throw new ArgumentException("Invalid file name.");

var data = File.ReadAllLines(source);

if (data.Length == 0)
    throw new Exception("Missing header of CSV file");

// read header
var columns = data[0].Split(';');
var rows = new List<List<string>>();
var columnLengths = Enumerable.Range(1, columns.Length).Select(x => 0).ToArray();
var builder = new StringBuilder();

try
{
    // find the longest records in all columnes
    for (int i = 0; i < data.Length; i++)
    {
        var row = data[i].Split(';');
        for (int j = 0; j < row.Length; j++)
        {
            if (row[j].Length > columnLengths[j])
                columnLengths[j] = row[j].Length;
        }

        rows.Add(row.ToList());
    }

    // reformat data with spaces
    foreach (var row in rows)
    {
        var rowArray = row.ToArray();
        for (int i = 0; i < rowArray.Length; i++)
        {
            builder.Append($"{rowArray[i].Replace("\"", string.Empty).PadRight(columnLengths[i])}");
        }
        builder.AppendLine();
    }
}
catch (Exception ex)
{
    throw new Exception($"Invalid CSV file, ended with:{ex.ToString()}");
}

var path = $"{source}.txt";
File.WriteAllText(path, builder.ToString());

Console.WriteLine($"Done, result at: {path}");

Console.ReadKey();
