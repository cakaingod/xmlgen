using System;
using System.IO;
using System.Linq;
using System.Text;
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

        // 2. Consultar al usuario la ruta de destino para los XML
        Console.WriteLine("=============================================");
        Console.WriteLine("        GENERADOR DE ARCHIVOS XML            ");
        Console.WriteLine("=============================================\n");

        Console.Write("Ingresa la ruta de la carpeta donde deseas guardar los XML (o presiona Enter para usar la carpeta actual): ");
        string? targetDirectory = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(targetDirectory))
        {
            targetDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        // Crear la carpeta de destino para XMLs si no existe
        try
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
                Console.WriteLine($"\n[INFO] La carpeta de destino XML fue creada: {targetDirectory}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError al acceder/crear la carpeta de XMLs: {ex.Message}");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }

        // 3. Crear carpeta "log" dentro de la carpeta del .exe si no existe
        string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string logFolder = Path.Combine(exeDirectory, "log");

        try
        {
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError al crear la carpeta 'log': {ex.Message}");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }

        // 4. Nombre dinámico del archivo LOG con fecha y hora
        string logFileName = $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
        string logFilePath = Path.Combine(logFolder, logFileName);

        StringBuilder logContent = new StringBuilder();
        logContent.AppendLine("==================================================================");
        logContent.AppendLine($"LOG DE PROCESAMIENTO XML - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        logContent.AppendLine("==================================================================");

        // 5. Leer registros del CSV
        string[] todasLasLineas = File.ReadAllLines(csvFile);
        var registros = todasLasLineas
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l) && !l.Equals("ACCESSION_NUMBER", StringComparison.OrdinalIgnoreCase))
            .ToList();

        int totalRegistros = registros.Count;

        if (totalRegistros == 0)
        {
            Console.WriteLine("\nEl archivo CSV está vacío o no contiene registros válidos.");
            logContent.AppendLine("ERROR: El archivo CSV no contiene registros válidos.");
            File.WriteAllText(logFilePath, logContent.ToString());
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\nSe encontraron {totalRegistros} registros para procesar.\n");

        int exitosos = 0;
        int fallidos = 0;
        int procesados = 0;

        // 6. Procesar cada registro
        foreach (string accessionNumber in registros)
        {
            procesados++;
            double porcentaje = (double)procesados / totalRegistros * 100;
            int bloquesLlenos = (int)(porcentaje / 5);
            string barra = new string('█', bloquesLlenos) + new string('░', 20 - bloquesLlenos);
            string timestamp = DateTime.Now.ToString("HH:mm:ss");

            try
            {
                // Generar XML
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

                string fileName = $"{accessionNumber}.xml";
                string fullPath = Path.Combine(targetDirectory, fileName);
                
                xml.Save(fullPath);

                exitosos++;
                
                // Consola
                Console.WriteLine($"[{procesados}/{totalRegistros}] [{barra}] {porcentaje:0.0}% | OK: {fileName}");
                
                // LOG
                logContent.AppendLine($"[{timestamp}] [ÉXITO] Código: {accessionNumber} -> Generado correctamente como '{fileName}'");
            }
            catch (Exception ex)
            {
                fallidos++;

                // Consola
                Console.WriteLine($"[{procesados}/{totalRegistros}] [{barra}] {porcentaje:0.0}% | ERROR en Código: {accessionNumber}");

                // LOG
                logContent.AppendLine($"[{timestamp}] [ERROR] Código: {accessionNumber} -> Falló la creación. Motivo: {ex.Message}");
            }
        }

        // 7. Resumen final en LOG y consola
        logContent.AppendLine("\n==================================================================");
        logContent.AppendLine("RESUMEN DE RESULTADOS:");
        logContent.AppendLine($"Total procesados: {totalRegistros}");
        logContent.AppendLine($"Exitosos:         {exitosos}");
        logContent.AppendLine($"Fallidos:         {fallidos}");
        logContent.AppendLine("==================================================================");

        // Guardar el archivo LOG dentro de la carpeta /log/
        File.WriteAllText(logFilePath, logContent.ToString());

        Console.WriteLine("\n=============================================");
        Console.WriteLine($"¡Proceso completado al 100%!");
        Console.WriteLine($"Éxitos: {exitosos} | Fallos: {fallidos}");
        Console.WriteLine($"Ubicación XMLs: {targetDirectory}");
        Console.WriteLine($"Archivo Log:    {logFilePath}");
        Console.WriteLine("=============================================");
        Console.WriteLine("\nPresiona cualquier tecla para salir...");
        Console.ReadKey();
    }
}
