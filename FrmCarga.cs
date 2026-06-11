using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SistemaFarmacia.Formularios
{
    public partial class FrmCarga : Form
    {
        private PictureBox picLoading;
        private Label lblCarga;
        private System.Windows.Forms.Timer fadeTimer;

        public FrmCarga()
        {
            InitializeComponent();

            CrearPantalla();
            CrearAnimacionFade();
        }

        #region FORMULARIO

        private void CrearPantalla()
        {
            // CONFIG FORM
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.DoubleBuffered = true;

            this.Width = 340;
            this.Height = 240;

            // COLOR FONDO
            this.BackColor = Color.WhiteSmoke;

            // TRANSPARENCIA INICIAL
            this.Opacity = 0;

            // BORDES REDONDEADOS
            this.Region = new Region(
                CrearBordesRedondeados(this.ClientRectangle, 25));

            // GIF
            picLoading = new PictureBox();
            picLoading.Size = new Size(150, 150);
            picLoading.SizeMode = PictureBoxSizeMode.Zoom;
            picLoading.BackColor = Color.Transparent;

            // RECURSO GIF
            picLoading.Image = ActualizadorProyectoVB2022.ResourceMensaje.carga_11;
                

            // LABEL
            lblCarga = new Label();
            lblCarga.Text = "Cargando información...";
            lblCarga.ForeColor = Color.FromArgb(50,150, 255);/*Color.FromArgb(30, 89, 215);*/
            lblCarga.BackColor = Color.Transparent;
            lblCarga.AutoSize = true;

            lblCarga.Font = new Font(
                "Segoe UI",
                15,
                FontStyle.Bold);

            // AGREGAR CONTROLES
            this.Controls.Add(picLoading);
            this.Controls.Add(lblCarga);

            // CENTRAR CONTROLES
            CentrarControles();
        }

        #endregion

        #region EFECTO FADE

        private void CrearAnimacionFade()
        {
            fadeTimer = new System.Windows.Forms.Timer();

            fadeTimer.Interval = 25;

            fadeTimer.Tick += (s, e) =>
            {
                if (this.Opacity < 0.95)
                {
                    this.Opacity += 0.05;
                }
                else
                {
                    fadeTimer.Stop();
                }
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            fadeTimer.Start();
        }

        #endregion

        #region BORDES REDONDEADOS

        private GraphicsPath CrearBordesRedondeados(
            Rectangle rect,
            int radio)
        {
            int d = radio * 2;

            GraphicsPath path = new GraphicsPath();

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);

            path.CloseFigure();

            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.Region = new Region(
                CrearBordesRedondeados(this.ClientRectangle, 25));

            CentrarControles();
        }

        #endregion

        #region DIBUJAR BORDE

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            Rectangle rect =
                new Rectangle(1, 1, this.Width - 3, this.Height - 3);

            using (Pen pen = new Pen(Color.FromArgb(50, 150, 255), 3)) /*Color.FromArgb(30, 89, 215)*/
            using (GraphicsPath path =
                CrearBordesRedondeados(rect, 25))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        #endregion

        #region MÉTODOS

        private void CentrarControles()
        {
            if (picLoading != null)
            {
                picLoading.Left =
                    (this.ClientSize.Width - picLoading.Width) / 2;

                picLoading.Top = 20;
            }

            if (lblCarga != null)
            {
                lblCarga.Left =
                    (this.ClientSize.Width - lblCarga.PreferredWidth) / 2;

                lblCarga.Top = picLoading.Bottom + 10;
            }
        }

        public void CambiarMensaje(string mensaje)
        {
            lblCarga.Text = mensaje;

            lblCarga.Left =
                (this.ClientSize.Width - lblCarga.PreferredWidth) / 2;
        }

        private void FrmCarga_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region SOMBRA WINDOWS

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.ClassStyle |= 0x20000;

                return cp;
            }
        }

        #endregion

        #region LIBERAR MEMORIA

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        fadeTimer?.Dispose();

        //        picLoading?.Dispose();

        //        lblCarga?.Dispose();
        //    }

        //    base.Dispose(disposing);
        //}

        #endregion
    }
}
