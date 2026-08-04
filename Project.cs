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

        // 2. Consultar al usuario la ruta de destino
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
                Console.WriteLine($"\n[INFO] La carpeta de destino fue creada: {targetDirectory}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError al acceder/crear la carpeta: {ex.Message}");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }

        // 3. Preparar el archivo LOG
        string logFilePath = Path.Combine(targetDirectory, "log_ejecucion.txt");
        StringBuilder logContent = new StringBuilder();
        
        logContent.AppendLine("==================================================================");
        logContent.AppendLine($"LOG DE PROCESAMIENTO XML - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        logContent.AppendLine("==================================================================");

        // 4. Leer registros del CSV
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

        // 5. Procesar cada registro y registrar en LOG
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
                
                // Detalle en el LOG
                logContent.AppendLine($"[{timestamp}] [ÉXITO] Código: {accessionNumber} -> Generado correctamente como '{fileName}'");
            }
            catch (Exception ex)
            {
                fallidos++;

                // Consola
                Console.WriteLine($"[{procesados}/{totalRegistros}] [{barra}] {porcentaje:0.0}% | ERROR en Código: {accessionNumber}");

                // Detalle en el LOG
                logContent.AppendLine($"[{timestamp}] [ERROR] Código: {accessionNumber} -> Falló la creación. Motivo: {ex.Message}");
            }
        }

        // 6. Resumen final en el LOG y en consola
        logContent.AppendLine("\n==================================================================");
        logContent.AppendLine("RESUMEN DE RESULTADOS:");
        logContent.AppendLine($"Total procesados: {totalRegistros}");
        logContent.AppendLine($"Exitosos:         {exitosos}");
        logContent.AppendLine($"Fallidos:         {fallidos}");
        logContent.AppendLine("==================================================================");

        // Guardar el archivo LOG físico
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
