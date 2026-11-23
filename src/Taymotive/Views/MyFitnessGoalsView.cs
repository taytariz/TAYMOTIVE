
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class MyFitnessGoalsView : Form
    {
        private readonly User currentUser;
        private readonly FitnessGoalsController goalsController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblUsername = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvGoals =
            new DataGridView();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        public MyFitnessGoalsView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(
                    nameof(currentUser)
                );

            // Utilizar el mismo archivo que el administrador
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "fitnessgoals.json"
            );

            goalsController =
                new FitnessGoalsController(filePath);

            InitializeComponents();
            LoadGoals();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Mis objetivos físicos";
            ClientSize = new Size(1100, 580);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MIS OBJETIVOS FÍSICOS";
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

            lblTotal.Text = "Total de objetivos: 0";
            lblTotal.Location = new Point(35, 130);
            lblTotal.AutoSize = true;

            // Tabla de objetivos
            dgvGoals.Location = new Point(35, 165);
            dgvGoals.Size = new Size(1030, 300);

            dgvGoals.ReadOnly = true;
            dgvGoals.AllowUserToAddRows = false;
            dgvGoals.AllowUserToDeleteRows = false;
            dgvGoals.MultiSelect = false;
            dgvGoals.RowHeadersVisible = false;

            dgvGoals.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvGoals.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvGoals.AllowUserToResizeColumns = true;

            dgvGoals.Columns.Add("Id", "ID");
            dgvGoals.Columns.Add("Type", "Objetivo");
            dgvGoals.Columns.Add("Initial", "Valor inicial");
            dgvGoals.Columns.Add("Target", "Valor meta");
            dgvGoals.Columns.Add("Current", "Valor actual");
            dgvGoals.Columns.Add("Progress", "Progreso (%)");
            dgvGoals.Columns.Add("Start", "Fecha inicio");
            dgvGoals.Columns.Add("End", "Fecha fin");
            dgvGoals.Columns.Add("Status", "Estado");

            // Dar más espacio al nombre del objetivo
            dgvGoals.Columns["Id"]!.FillWeight = 40;
            dgvGoals.Columns["Type"]!.FillWeight = 190;
            dgvGoals.Columns["Initial"]!.FillWeight = 85;
            dgvGoals.Columns["Target"]!.FillWeight = 85;
            dgvGoals.Columns["Current"]!.FillWeight = 85;
            dgvGoals.Columns["Progress"]!.FillWeight = 95;
            dgvGoals.Columns["Start"]!.FillWeight = 105;
            dgvGoals.Columns["End"]!.FillWeight = 105;
            dgvGoals.Columns["Status"]!.FillWeight = 105;

            // Mostrar descripción completa al pasar el mouse
            dgvGoals.ShowCellToolTips = true;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(395, 495);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(555, 495);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(lblTotal);
            Controls.Add(dgvGoals);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();

            ClientSizeChanged += (sender, e) =>
            {
                CenterTitle();
            };
        }

        // Cargar únicamente los objetivos del usuario
        private void LoadGoals()
        {
            try
            {
                dgvGoals.Rows.Clear();

                var goals =
                    goalsController.GetGoalsByUser(
                        currentUser.Id
                    );

                foreach (FitnessGoal goal in goals)
                {
                    string status;

                    if (goal.IsCompleted)
                    {
                        status = "Completado";
                    }
                    else if (
                        DateTime.Today > goal.EndDate.Date)
                    {
                        status = "Vencido";
                    }
                    else
                    {
                        status = "En progreso";
                    }

                    int rowIndex = dgvGoals.Rows.Add(
                        goal.Id,
                        goal.GoalType,
                        goal.InitialValue.ToString("F2"),
                        goal.TargetValue.ToString("F2"),
                        goal.CurrentValue.ToString("F2"),
                        goal.CalculateProgress().ToString("F2"),
                        goal.StartDate.ToString("dd/MM/yyyy"),
                        goal.EndDate.ToString("dd/MM/yyyy"),
                        status
                    );

                    // Mostrar nombre completo al pasar el mouse
                    dgvGoals.Rows[rowIndex]
                        .Cells["Type"].ToolTipText =
                        goal.GoalType;
                }

                lblTotal.Text =
                    "Total de objetivos: " +
                    goals.Count;

                dgvGoals.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar los objetivos:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Actualizar listado
        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadGoals();
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
