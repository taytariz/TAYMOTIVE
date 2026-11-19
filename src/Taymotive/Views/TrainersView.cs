
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class TrainersView : Form
    {
        private Label lblTitle = new Label();
        private Label lblTotal = new Label();

        private DataGridView dgvTrainers = new DataGridView();

        private Button btnRegister = new Button();
        private Button btnEdit = new Button();
        private Button btnDelete = new Button();
        private Button btnRefresh = new Button();
        private Button btnClose = new Button();

        private readonly TrainersController trainersController;

        public TrainersView()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "trainers.json"
            );

            trainersController = new TrainersController(filePath);

            InitializeComponents();
            LoadTrainers();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Gestión de entrenadores";
            this.Size = new Size(950, 570);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "GESTIÓN DE ENTRENADORES";
            lblTitle.Font = new Font(
                "Arial",
                18,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(240, 25);

            lblTotal.Text = "Total de entrenadores: 0";
            lblTotal.Location = new Point(30, 85);
            lblTotal.AutoSize = true;

            dgvTrainers.Location = new Point(30, 120);
            dgvTrainers.Size = new Size(870, 300);

            dgvTrainers.ReadOnly = true;
            dgvTrainers.AllowUserToAddRows = false;
            dgvTrainers.AllowUserToDeleteRows = false;
            dgvTrainers.MultiSelect = false;
            dgvTrainers.RowHeadersVisible = false;

            dgvTrainers.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvTrainers.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvTrainers.Columns.Add("Id", "ID");
            dgvTrainers.Columns.Add("Name", "Nombre");
            dgvTrainers.Columns.Add("Age", "Edad");
            dgvTrainers.Columns.Add("Weight", "Peso (kg)");
            dgvTrainers.Columns.Add("Height", "Estatura (m)");
            dgvTrainers.Columns.Add("Specialty", "Especialidad");

            dgvTrainers.Columns.Add(
                "IsPersonalTrainer",
                "Entrenador personal"
            );

            btnRegister.Text = "Registrar entrenador";
            btnRegister.Location = new Point(25, 445);
            btnRegister.Size = new Size(180, 40);
            btnRegister.Click += BtnRegister_Click;

            btnEdit.Text = "Modificar entrenador";
            btnEdit.Location = new Point(215, 445);
            btnEdit.Size = new Size(180, 40);
            btnEdit.Click += BtnEdit_Click;

            btnDelete.Text = "Eliminar entrenador";
            btnDelete.Location = new Point(405, 445);
            btnDelete.Size = new Size(180, 40);
            btnDelete.Click += BtnDelete_Click;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(595, 445);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(745, 445);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblTotal);
            this.Controls.Add(dgvTrainers);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
        }

        // Cargar entrenadores desde JSON
        private void LoadTrainers()
        {
            try
            {
                dgvTrainers.Rows.Clear();

                var trainers = trainersController.GetTrainers();

                foreach (var trainer in trainers)
                {
                    dgvTrainers.Rows.Add(
                        trainer.Id,
                        trainer.Name,
                        trainer.Age,
                        trainer.Weight.ToString("F2"),
                        trainer.Height.ToString("F2"),
                        trainer.Specialty,
                        trainer.IsPersonalTrainer ? "Sí" : "No"
                    );
                }

                lblTotal.Text =
                    "Total de entrenadores: " + trainers.Count;

                dgvTrainers.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar entrenadores: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Registrar entrenador
        private void BtnRegister_Click(
            object? sender,
            EventArgs e)
        {
            using (RegisterTrainerView registerView =
                new RegisterTrainerView())
            {
                if (registerView.ShowDialog(this) == DialogResult.OK)
                {
                    LoadTrainers();
                }
            }
        }

        // Modificar entrenador
        private void BtnEdit_Click(
            object? sender,
            EventArgs e)
        {
            if (dgvTrainers.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un entrenador para modificar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int trainerId = Convert.ToInt32(
                dgvTrainers.SelectedRows[0].Cells["Id"].Value
            );

            using (EditTrainerView editView =
                new EditTrainerView(trainerId))
            {
                if (editView.ShowDialog(this) == DialogResult.OK)
                {
                    LoadTrainers();
                }
            }
        }

        // Eliminar entrenador
        private void BtnDelete_Click(
            object? sender,
            EventArgs e)
        {
            if (dgvTrainers.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un entrenador para eliminar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int trainerId = Convert.ToInt32(
                dgvTrainers.SelectedRows[0].Cells["Id"].Value
            );

            string trainerName = Convert.ToString(
                dgvTrainers.SelectedRows[0].Cells["Name"].Value
            ) ?? string.Empty;

            DialogResult confirmation = MessageBox.Show(
                "¿Está seguro de eliminar al entrenador " +
                trainerName + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool deleted =
                    trainersController.DeleteTrainer(trainerId);

                if (deleted)
                {
                    MessageBox.Show(
                        "Entrenador eliminado correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadTrainers();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo eliminar el entrenador.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar entrenador: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Actualizar tabla
        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadTrainers();
        }

        // Cerrar ventana
        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            this.Close();
        }
    }
}
