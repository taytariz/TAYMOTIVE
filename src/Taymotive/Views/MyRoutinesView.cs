
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class MyRoutinesView : Form
    {
        private readonly User currentUser;
        private readonly RoutinesController routinesController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblUsername = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvRoutines =
            new DataGridView();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        public MyRoutinesView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(
                    nameof(currentUser)
                );

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            routinesController =
                new RoutinesController(filePath);

            InitializeComponents();
            LoadRoutines();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Mis rutinas";
            ClientSize = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MIS RUTINAS";
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

            lblTotal.Text = "Total de rutinas: 0";
            lblTotal.Location = new Point(35, 130);
            lblTotal.AutoSize = true;

            dgvRoutines.Location = new Point(35, 165);
            dgvRoutines.Size = new Size(880, 290);

            dgvRoutines.ReadOnly = true;
            dgvRoutines.AllowUserToAddRows = false;
            dgvRoutines.AllowUserToDeleteRows = false;
            dgvRoutines.MultiSelect = false;
            dgvRoutines.RowHeadersVisible = false;

            dgvRoutines.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvRoutines.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvRoutines.Columns.Add("Id", "ID");
            dgvRoutines.Columns.Add("Name", "Rutina");
            dgvRoutines.Columns.Add(
                "Description",
                "Descripción"
            );
            dgvRoutines.Columns.Add(
                "Duration",
                "Duración (min)"
            );
            dgvRoutines.Columns.Add(
                "StartDate",
                "Fecha de inicio"
            );
            dgvRoutines.Columns.Add(
                "EndDate",
                "Fecha de finalización"
            );

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(325, 480);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(485, 480);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(lblTotal);
            Controls.Add(dgvRoutines);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();

            ClientSizeChanged += (sender, e) =>
            {
                CenterTitle();
            };
        }

        // Cargar únicamente las rutinas del usuario
        private void LoadRoutines()
        {
            try
            {
                dgvRoutines.Rows.Clear();

                var routines =
                    routinesController.GetRoutinesByUser(
                        currentUser.Id
                    );

                foreach (Routine routine in routines)
                {
                    dgvRoutines.Rows.Add(
                        routine.Id,
                        routine.Name,
                        routine.Description,
                        routine.Duration,
                        routine.StartDate.ToString(
                            "dd/MM/yyyy"
                        ),
                        routine.EndDate.ToString(
                            "dd/MM/yyyy"
                        )
                    );
                }

                lblTotal.Text =
                    "Total de rutinas: " + routines.Count;

                dgvRoutines.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar las rutinas: " +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadRoutines();
        }

        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }

        private void CenterTitle()
        {
            lblTitle.Left =
                (ClientSize.Width - lblTitle.Width) / 2;
        }
    }
}
