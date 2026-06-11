using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ActualizadorProyectoVB2022
{

    public class ConfigIni
    {
        private readonly Dictionary<string, string> _config =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public ConfigIni(string archivoIni)
        {
            foreach (var linea in File.ReadAllLines(archivoIni))
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                if (linea.StartsWith(";"))
                    continue;

                int pos = linea.IndexOf('=');

                if (pos <= 0)
                    continue;

                string clave = linea.Substring(0, pos).Trim();
                string valor = linea.Substring(pos + 1).Trim();

                _config[clave] = valor;
            }
        }

        public string GetValor(string clave)
        {
            return _config.TryGetValue(clave, out string valor)
                ? valor
                : string.Empty;
        }

        public int GetEntero(string clave)
        {
            return int.TryParse(GetValor(clave), out int valor)
                ? valor
                : 0;
        }
    }

}


