using SistemaFarmacia.Formularios;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActualizadorProyectoVB2022
{
    public partial class FrmActualiza : Form
    {
        public FrmActualiza()
        {
            InitializeComponent();
           // this.Hide();
            this.ShowInTaskbar = false;
            this.Opacity = 0;
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            await CargarDatos();
        }

        private async Task CargarDatos()
        {
            FrmCarga frmCarga = new FrmCarga();

            try
            {
                frmCarga.Show();

                //frmCarga.Show(this);
                frmCarga.BringToFront();
                frmCarga.CambiarMensaje("Buscando actualizaciones...");
                frmCarga.Refresh();

                var progress = new Progress<string>(mensaje =>
                {
                    frmCarga.CambiarMensaje(mensaje);
                });

                await Task.Run(() => LeeIni(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
               
                if (!frmCarga.IsDisposed)
                    frmCarga.Close();
            }
        }

        private void LeeIni(IProgress<string> progress)
        {
            progress.Report("Leyendo archivo de configuración...");

            string rutaIni = Path.Combine(
                Application.StartupPath,
                "configFarmacia.ini");

            if (!File.Exists(rutaIni))
            {
                throw new Exception(
                    "No se encontró el archivo configFarmacia.ini.");
            }

            ConfigIni ini = new ConfigIni(rutaIni);

            string exeDestino = ini.GetValor("PATHdestino");
            string exeOrigen = ini.GetValor("PATHorigen");

            progress.Report("Comparando versiones...");

            Version versionOrigen = ObtenerVersion(exeOrigen);
            Version versionDestino = ObtenerVersion(exeDestino);

            if (versionOrigen > versionDestino)
            {
                progress.Report(
                    $"Actualizando ejecutable ({versionDestino} -> {versionOrigen})...");

                string carpetaDestino = Path.GetDirectoryName(exeDestino);

                if (!Directory.Exists(carpetaDestino))
                    Directory.CreateDirectory(carpetaDestino);

                string nombreProceso = Path.GetFileNameWithoutExtension(exeDestino);

                Process[] procesos = Process.GetProcessesByName(nombreProceso);

                if (procesos.Length > 0)
                {
                    foreach (Process p in procesos)
                    {
                        p.Kill();
                        p.WaitForExit();
                    }
                }



                File.Copy(exeOrigen, exeDestino, true);
            }

            string baseLocal = ini.GetValor("BaseLocal");

            if (baseLocal.Equals("S",
                StringComparison.OrdinalIgnoreCase))
            {
                progress.Report("Actualizando base local...");

                string bdOrigen =
                    ini.GetValor("PATHorigenBdLocal");

                string bdDestino =
                    ini.GetValor("PATHdestinoBdLocal");

                if (File.Exists(bdOrigen))
                {
                    File.Copy(
                        bdOrigen,
                        bdDestino,
                        true);
                }
            }

            progress.Report("Actualizando informes...");

            int cantidadInformes =
                ini.GetEntero("NumeroInformes");

            string pathInformesOrigen =
                ini.GetValor("PATHInformesOrigen");

            string pathInformesDestino =
                ini.GetValor("PATHInformesDestino");

            for (int i = 1; i <= cantidadInformes; i++)
            {
                string informe =
                    ini.GetValor($"Informe{i}");

                if (string.IsNullOrWhiteSpace(informe))
                    continue;

                progress.Report(
                    $"Actualizando informe {i}/{cantidadInformes}");

                string origen = Path.Combine(
                    pathInformesOrigen,
                    informe.TrimStart('\\'));

                string destino = Path.Combine(
                    pathInformesDestino,
                    informe.TrimStart('\\'));

                if (File.Exists(origen))
                {
                    string carpetaDestino =
                        Path.GetDirectoryName(destino);

                    if (!Directory.Exists(carpetaDestino))
                        Directory.CreateDirectory(carpetaDestino);

                    File.Copy(origen, destino, true);
                }
            }

            progress.Report("Finalizando...");

            if (File.Exists(exeDestino))
            {
                //Process.Start(exeDestino); // levanto ejecutable
            }

            this.Invoke(new Action(() =>
            {
                Application.Exit();
            }));
        }

        private Version ObtenerVersion(string rutaExe)
        {
            if (!File.Exists(rutaExe))
                return new Version(0, 0, 0, 0);

            FileVersionInfo info =
                FileVersionInfo.GetVersionInfo(rutaExe);

            return new Version(info.FileVersion);
        }

        private void FrmActualiza_Load(object sender, EventArgs e)
        {

        }
    }
}