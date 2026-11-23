
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class TrainersView : Form
    {
        private readonly Label lblTitle = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvTrainers =
            new DataGridView();

        private readonly Button btnRegister = new Button();
        private readonly Button btnEdit = new Button();
        private readonly Button btnDelete = new Button();
        private readonly Button btnNotifications = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private readonly TrainersController trainersController;

        public TrainersView()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "trainers.json"
            );

            trainersController =
                new TrainersController(filePath);

            InitializeComponents();
            LoadTrainers();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Gestión de entrenadores";
            ClientSize = new Size(950, 590);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "GESTIÓN DE ENTRENADORES";
            lblTitle.Font = new Font(
                "Arial",
                18,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Top = 25;

            lblTotal.Text = "Total de entrenadores: 0";
            lblTotal.Location = new Point(30, 85);
            lblTotal.AutoSize = true;

            dgvTrainers.Location = new Point(30, 120);
            dgvTrainers.Size = new Size(890, 310);

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
            dgvTrainers.Columns.Add(
                "Specialty", "Especialidad"
            );
            dgvTrainers.Columns.Add(
                "IsPersonalTrainer",
                "Entrenador personal"
            );

            btnRegister.Text = "Registrar entrenador";
            btnRegister.Location = new Point(30, 455);
            btnRegister.Size = new Size(180, 40);
            btnRegister.Click += BtnRegister_Click;

            btnEdit.Text = "Modificar entrenador";
            btnEdit.Location = new Point(220, 455);
            btnEdit.Size = new Size(180, 40);
            btnEdit.Click += BtnEdit_Click;

            btnDelete.Text = "Eliminar entrenador";
            btnDelete.Location = new Point(410, 455);
            btnDelete.Size = new Size(180, 40);
            btnDelete.Click += BtnDelete_Click;

            btnNotifications.Text = "Ver notificaciones";
            btnNotifications.Location = new Point(600, 455);
            btnNotifications.Size = new Size(180, 40);
            btnNotifications.Click += BtnNotifications_Click;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(305, 515);
            btnRefresh.Size = new Size(150, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(495, 515);
            btnClose.Size = new Size(150, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblTotal);
            Controls.Add(dgvTrainers);
            Controls.Add(btnRegister);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            Controls.Add(btnNotifications);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();
        }

        // Cargar entrenadores desde JSON
        private void LoadTrainers()
        {
            try
            {
                dgvTrainers.Rows.Clear();

                var trainers =
                    trainersController.GetTrainers();

                foreach (Trainer trainer in trainers)
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
                    "Total de entrenadores: " +
                    trainers.Count;

                dgvTrainers.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar entrenadores:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Obtener el entrenador seleccionado
        private int? GetSelectedTrainerId()
        {
            if (dgvTrainers.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un entrenador primero.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return null;
            }

            return Convert.ToInt32(
                dgvTrainers.SelectedRows[0]
                    .Cells["Id"].Value
            );
        }

        // Registrar entrenador
        private void BtnRegister_Click(
            object? sender,
            EventArgs e)
        {
            using (RegisterTrainerView view =
                new RegisterTrainerView())
            {
                if (view.ShowDialog(this) ==
                    DialogResult.OK)
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
            int? trainerId = GetSelectedTrainerId();

            if (!trainerId.HasValue)
            {
                return;
            }

            using (EditTrainerView view =
                new EditTrainerView(trainerId.Value))
            {
                if (view.ShowDialog(this) ==
                    DialogResult.OK)
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
            int? trainerId = GetSelectedTrainerId();

            if (!trainerId.HasValue)
            {
                return;
            }

            string trainerName =
                Convert.ToString(
                    dgvTrainers.SelectedRows[0]
                        .Cells["Name"].Value
                ) ?? string.Empty;

            DialogResult confirmation =
                MessageBox.Show(
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
                    trainersController.DeleteTrainer(
                        trainerId.Value
                    );

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
                    "Error al eliminar entrenador:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Ver notificaciones del entrenador
        private void BtnNotifications_Click(
            object? sender,
            EventArgs e)
        {
            int? trainerId = GetSelectedTrainerId();

            if (!trainerId.HasValue)
            {
                return;
            }

            try
            {
                Trainer? trainer =
                    trainersController.GetTrainerById(
                        trainerId.Value
                    );

                if (trainer == null)
                {
                    MessageBox.Show(
                        "No se encontró el entrenador seleccionado.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                using (TrainerNotificationsView view =
                    new TrainerNotificationsView(trainer))
                {
                    view.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al abrir las notificaciones:\n" +
                    ex.Message,
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
