
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class MyVisitsView : Form
    {
        private readonly User currentUser;
        private readonly VisitsController visitsController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblUsername = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvVisits =
            new DataGridView();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        public MyVisitsView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(
                    nameof(currentUser)
                );

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "visits.json"
            );

            visitsController = new VisitsController(filePath);

            InitializeComponents();
            LoadVisits();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Mis visitas y clases";
            ClientSize = new Size(900, 550);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MIS VISITAS Y CLASES";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Top = 25;

            lblUsername.Text =
                "Usuario: " + currentUser.Name;

            lblUsername.Font = new Font("Arial", 11);
            lblUsername.Location = new Point(35, 95);
            lblUsername.AutoSize = true;

            lblTotal.Text = "Total de visitas: 0";
            lblTotal.Location = new Point(35, 130);
            lblTotal.AutoSize = true;

            dgvVisits.Location = new Point(35, 165);
            dgvVisits.Size = new Size(830, 280);

            dgvVisits.ReadOnly = true;
            dgvVisits.AllowUserToAddRows = false;
            dgvVisits.AllowUserToDeleteRows = false;
            dgvVisits.MultiSelect = false;
            dgvVisits.RowHeadersVisible = false;

            dgvVisits.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvVisits.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvVisits.Columns.Add("Id", "ID");
            dgvVisits.Columns.Add("Date", "Fecha de visita");
            dgvVisits.Columns.Add("Class", "Clase o actividad");

            dgvVisits.Columns["Id"]!.FillWeight = 40;
            dgvVisits.Columns["Date"]!.FillWeight = 120;
            dgvVisits.Columns["Class"]!.FillWeight = 240;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(295, 470);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(455, 470);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(lblTotal);
            Controls.Add(dgvVisits);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();

            ClientSizeChanged += (sender, e) =>
            {
                CenterTitle();
            };
        }

        // Cargar las visitas del usuario
        private void LoadVisits()
        {
            try
            {
                dgvVisits.Rows.Clear();

                var visits = visitsController.GetVisitsByUser(
                    currentUser.Id
                );

                foreach (Visit visit in visits)
                {
                    dgvVisits.Rows.Add(
                        visit.Id,
                        visit.VisitDate.ToString("dd/MM/yyyy"),
                        visit.ClassName
                    );
                }

                lblTotal.Text =
                    "Total de visitas: " + visits.Count;

                dgvVisits.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar las visitas:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Actualizar visitas
        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadVisits();
        }

        // Cerrar ventana
        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }

        // Centrar título
        private void CenterTitle()
        {
            lblTitle.Left =
                (ClientSize.Width - lblTitle.Width) / 2;
        }
    }
}
