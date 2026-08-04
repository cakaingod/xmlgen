using System;
using System.IO;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {
        string csvFile = "numerosacceso.csv";

        if (!File.Exists(csvFile))
        {
            Console.WriteLine($"Error: Coloca el archivo '{csvFile}' en la misma carpeta del ejecutable.");
            return;
        }

        string[] lineas = File.ReadAllLines(csvFile);

        foreach (string linea in lineas)
        {
            string accessionNumber = linea.Trim();

            // Salta líneas vacías o la cabecera si existe
            if (string.IsNullOrWhiteSpace(accessionNumber) || 
                accessionNumber.Equals("ACCESSION_NUMBER", StringComparison.OrdinalIgnoreCase))
                continue;

            // Construcción del XML con la plantilla exacta
            XElement xml = new XElement("MWL_ITEM",
                new XElement("ACCESSION_NUMBER", accessionNumber),
                new XElement("SERIES_NUMBER", "1000"),
                new XElement("SERIES_DESC", "pedido"),
                new XElement("SERIES_IMAGE_COUNT", "1"),
                new XElement("LAUDO_PDF", "true"),
                new XElement("LOGIN_MED", "MED_MIGRACION"),
                new XElement("URGENT", "false"),
                new XElement("IP", "127.0.0.1")
            );

            // Genera un archivo XML nombrado por su Accession Number
            string outputXml = $"{accessionNumber}.xml";
            xml.Save(outputXml);
            Console.WriteLine($"XML generado: {outputXml}");
        }

        Console.WriteLine("\n¡Proceso completado con éxito!");
    }
}
