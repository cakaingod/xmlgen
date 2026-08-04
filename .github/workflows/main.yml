using System;
using System.IO;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {
        string csvFile = "numerosacceso.csv";

        // 1. Verificar si el archivo CSV existe
        if (!File.Exists(csvFile))
        {
            Console.WriteLine($"Error: No se encontró el archivo '{csvFile}'. Asegúrate de que esté en la misma carpeta que el .exe.");
            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }

        // 2. Consultar al usuario la ruta donde desea guardar los XML
        Console.WriteLine("=============================================");
        Console.WriteLine("        GENERADOR DE ARCHIVOS XML            ");
        Console.WriteLine("=============================================\n");
        
        Console.Write("Ingresa la ruta de la carpeta donde deseas guardar los XML (o presiona Enter para usar la carpeta actual): ");
        string? targetDirectory = Console.ReadLine()?.Trim();

        // Si el usuario no escribe nada, usa la carpeta donde corre el programa
        if (string.IsNullOrWhiteSpace(targetDirectory))
        {
            targetDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        // Crear la carpeta de destino si no existe
        try;
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
                Console.WriteLine($"\n[INFO] La carpeta de destino no existía y fue creada: {targetDirectory}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError al acceder/crear la carpeta: {ex.Message}");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }

        // 3. Procesar el archivo CSV
        string[] lineas = File.ReadAllLines(csvFile);
        int contador = 0;

        Console.WriteLine("\nProcesando números de acceso...\n");

        foreach (string linea in lineas)
        {
            string accessionNumber = linea.Trim();

            // Omitir líneas vacías o la cabecera "ACCESSION_NUMBER"
            if (string.IsNullOrWhiteSpace(accessionNumber) || 
                accessionNumber.Equals("ACCESSION_NUMBER", StringComparison.OrdinalIgnoreCase))
                continue;

            // Construir el XML
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

            // Generar ruta completa de salida
            string fileName = $"{accessionNumber}.xml";
            string fullPath = Path.Combine(targetDirectory, fileName);

            xml.Save(fullPath);
            Console.WriteLine($"[OK] Generado: {fileName}");
            contador++;
        }

        Console.WriteLine($"\n=============================================");
        Console.WriteLine($"¡Proceso completado! Total XMLs creados: {contador}");
        Console.WriteLine($"Ubicación: {targetDirectory}");
        Console.WriteLine("=============================================");
        Console.WriteLine("\nPresiona cualquier tecla para salir...");
        Console.ReadKey();
    }
}
