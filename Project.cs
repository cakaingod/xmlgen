using System;
using System.IO;
using System.Linq;
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

        if (string.IsNullOrWhiteSpace(targetDirectory))
        {
            targetDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        // Crear la carpeta de destino si no existe
        try
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

        // 3. Leer y filtrar las líneas del CSV para conocer el total real de registros
        string[] todasLasLineas = File.ReadAllLines(csvFile);
        var registros = todasLasLineas
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l) && !l.Equals("ACCESSION_NUMBER", StringComparison.OrdinalIgnoreCase))
            .ToList();

        int totalRegistros = registros.Count;

        if (totalRegistros == 0)
        {
            Console.WriteLine("\nEl archivo CSV está vacío o no contiene registros válidos.");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\nSe encontraron {totalRegistros} registros para procesar.\n");

        int procesados = 0;

        // 4. Procesar con porcentaje y barra de progreso
        foreach (string accessionNumber in registros)
        {
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

            // Generar archivo XML
            string fileName = $"{accessionNumber}.xml";
            string fullPath = Path.Combine(targetDirectory, fileName);
            xml.Save(fullPath);

            procesados++;

            // Cálculo de porcentaje
            double porcentaje = (double)procesados / totalRegistros * 100;

            // Dibujar barra de progreso (longitud de 20 caracteres)
            int bloquesLlenos = (int)(porcentaje / 5); // 100% / 20 = 5% por bloque
            string barra = new string('█', bloquesLlenos) + new string('░', 20 - bloquesLlenos);

            // Imprimir línea formateada
            Console.WriteLine($"[{procesados}/{totalRegistros}] [{barra}] {porcentaje:0.0}% | Generado: {fileName}");
        }

        Console.WriteLine("\n=============================================");
        Console.WriteLine($"¡Proceso completado al 100%! Total XMLs creados: {procesados}");
        Console.WriteLine($"Ubicación: {targetDirectory}");
        Console.WriteLine("=============================================");
        Console.WriteLine("\nPresiona cualquier tecla para salir...");
        Console.ReadKey();
    }
}
