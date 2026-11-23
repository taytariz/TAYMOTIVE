
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class RoutinesView : Form
    {
        private readonly Label lblTitle = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvRoutines =
            new DataGridView();

        private readonly Button btnRegister = new Button();
        private readonly Button btnEdit = new Button();
        private readonly Button btnDelete = new Button();
        private readonly Button btnAssign = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private readonly RoutinesController routinesController;
        private readonly TrainersController trainersController;

        public RoutinesView()
        {
            string filesPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files"
            );

            routinesController = new RoutinesController(
                Path.Combine(filesPath, "users.json")
            );

            trainersController = new TrainersController(
                Path.Combine(filesPath, "trainers.json")
            );

            InitializeComponents();
            LoadRoutines();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Gestión de rutinas";
            ClientSize = new Size(1150, 610);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "GESTIÓN DE RUTINAS";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Top = 25;

            lblTotal.Text = "Total de rutinas: 0";
            lblTotal.Location = new Point(30, 90);
            lblTotal.AutoSize = true;

            dgvRoutines.Location = new Point(30, 125);
            dgvRoutines.Size = new Size(1090, 340);
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
            dgvRoutines.Columns.Add("User", "Usuario");
            dgvRoutines.Columns.Add("Name", "Rutina");
            dgvRoutines.Columns.Add(
                "Description", "Descripción"
            );
            dgvRoutines.Columns.Add(
                "Duration", "Duración (min)"
            );
            dgvRoutines.Columns.Add(
                "StartDate", "Fecha inicio"
            );
            dgvRoutines.Columns.Add(
                "EndDate", "Fecha fin"
            );
            dgvRoutines.Columns.Add(
                "Trainer", "Entrenador"
            );

            btnRegister.Text = "Registrar rutina";
            ConfigureButton(btnRegister, 30);
            btnRegister.Click += BtnRegister_Click;

            btnEdit.Text = "Modificar rutina";
            ConfigureButton(btnEdit, 215);
            btnEdit.Click += BtnEdit_Click;

            btnDelete.Text = "Eliminar rutina";
            ConfigureButton(btnDelete, 400);
            btnDelete.Click += BtnDelete_Click;

            btnAssign.Text = "Asignar entrenador";
            ConfigureButton(btnAssign, 585);
            btnAssign.Click += BtnAssign_Click;

            btnRefresh.Text = "Actualizar";
            ConfigureButton(btnRefresh, 770);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            ConfigureButton(btnClose, 955);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblTotal);
            Controls.Add(dgvRoutines);
            Controls.Add(btnRegister);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            Controls.Add(btnAssign);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();
        }

        private static void ConfigureButton(Button button, int x)
        {
            button.Location = new Point(x, 500);
            button.Size = new Size(165, 42);
        }

        // Cargar rutinas y entrenadores
        private void LoadRoutines()
        {
            try
            {
                dgvRoutines.Rows.Clear();

                List<User> users =
                    routinesController.GetUsers();

                List<Trainer> trainers =
                    trainersController.GetTrainers();

                int totalRoutines = 0;

                foreach (User user in users)
                {
                    foreach (Routine routine in user.Routines)
                    {
                        Trainer? trainer = trainers
                            .FirstOrDefault(item =>
                                item.Id == routine.TrainerId
                            );

                        string trainerName = trainer == null
                            ? "Sin asignar"
                            : trainer.Name;

                        dgvRoutines.Rows.Add(
                            routine.Id,
                            user.Name,
                            routine.Name,
                            routine.Description,
                            routine.Duration,
                            routine.StartDate.ToString(
                                "dd/MM/yyyy"
                            ),
                            routine.EndDate.ToString(
                                "dd/MM/yyyy"
                            ),
                            trainerName
                        );

                        totalRoutines++;
                    }
                }

                lblTotal.Text =
                    "Total de rutinas: " + totalRoutines;

                dgvRoutines.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar las rutinas:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Registrar rutina
        private void BtnRegister_Click(
            object? sender,
            EventArgs e)
        {
            using (RegisterRoutineView view =
                new RegisterRoutineView())
            {
                if (view.ShowDialog(this) ==
                    DialogResult.OK)
                {
                    LoadRoutines();
                }
            }
        }

        // Modificar rutina
        private void BtnEdit_Click(
            object? sender,
            EventArgs e)
        {
            int? routineId = GetSelectedRoutineId();

            if (!routineId.HasValue)
            {
                return;
            }

            using (EditRoutineView view =
                new EditRoutineView(routineId.Value))
            {
                if (view.ShowDialog(this) ==
                    DialogResult.OK)
                {
                    LoadRoutines();
                }
            }
        }

        // Eliminar rutina
        private void BtnDelete_Click(
            object? sender,
            EventArgs e)
        {
            int? routineId = GetSelectedRoutineId();

            if (!routineId.HasValue)
            {
                return;
            }

            string routineName =
                Convert.ToString(
                    dgvRoutines.SelectedRows[0]
                        .Cells["Name"].Value
                ) ?? string.Empty;

            DialogResult confirmation =
                MessageBox.Show(
                    "¿Está seguro de eliminar la rutina " +
                    routineName + "?",
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
                    routinesController.DeleteRoutine(
                        routineId.Value
                    );

                if (deleted)
                {
                    MessageBox.Show(
                        "Rutina eliminada correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadRoutines();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo eliminar la rutina.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar la rutina:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Asignar entrenador
        private void BtnAssign_Click(
            object? sender,
            EventArgs e)
        {
            int? routineId = GetSelectedRoutineId();

            if (!routineId.HasValue)
            {
                return;
            }

            try
            {
                List<Trainer> trainers =
                    trainersController.GetTrainers();

                if (trainers.Count == 0)
                {
                    MessageBox.Show(
                        "No existen entrenadores registrados.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                using (Form dialog = new Form())
                {
                    dialog.Text = "Asignar entrenador";
                    dialog.ClientSize = new Size(420, 200);
                    dialog.StartPosition =
                        FormStartPosition.CenterParent;
                    dialog.FormBorderStyle =
                        FormBorderStyle.FixedDialog;
                    dialog.MaximizeBox = false;
                    dialog.MinimizeBox = false;

                    Label lblTrainer = new Label();
                    lblTrainer.Text =
                        "Seleccione el entrenador:";
                    lblTrainer.Location =
                        new Point(30, 30);
                    lblTrainer.AutoSize = true;

                    ComboBox cmbTrainers = new ComboBox();
                    cmbTrainers.Location =
                        new Point(30, 65);
                    cmbTrainers.Size = new Size(350, 30);
                    cmbTrainers.DropDownStyle =
                        ComboBoxStyle.DropDownList;

                    cmbTrainers.DataSource = trainers;
                    cmbTrainers.DisplayMember = "Name";
                    cmbTrainers.ValueMember = "Id";

                    Routine? selectedRoutine =
                        routinesController.GetRoutineById(
                            routineId.Value
                        );

                    if (selectedRoutine != null &&
                        selectedRoutine.TrainerId > 0)
                    {
                        cmbTrainers.SelectedValue =
                            selectedRoutine.TrainerId;
                    }

                    Button btnSave = new Button();
                    btnSave.Text = "Guardar";
                    btnSave.Location =
                        new Point(80, 125);
                    btnSave.Size = new Size(120, 40);

                    Button btnCancel = new Button();
                    btnCancel.Text = "Cancelar";
                    btnCancel.Location =
                        new Point(220, 125);
                    btnCancel.Size = new Size(120, 40);
                    btnCancel.DialogResult =
                        DialogResult.Cancel;

                    btnSave.Click += (s, args) =>
                    {
                        if (cmbTrainers.SelectedItem
                            is not Trainer trainer)
                        {
                            return;
                        }

                        bool assigned =
                            routinesController
                                .AssignTrainerToRoutine(
                                    routineId.Value,
                                    trainer.Id
                                );

                        if (assigned)
                        {
                            MessageBox.Show(
                                "Entrenador asignado correctamente.",
                                "TAYMOTIVE",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            dialog.DialogResult =
                                DialogResult.OK;

                            dialog.Close();
                        }
                        else
                        {
                            MessageBox.Show(
                                "No se pudo asignar el entrenador.",
                                "TAYMOTIVE",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                        }
                    };

                    dialog.Controls.Add(lblTrainer);
                    dialog.Controls.Add(cmbTrainers);
                    dialog.Controls.Add(btnSave);
                    dialog.Controls.Add(btnCancel);
                    dialog.CancelButton = btnCancel;

                    if (dialog.ShowDialog(this) ==
                        DialogResult.OK)
                    {
                        LoadRoutines();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al asignar entrenador:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Obtener rutina seleccionada
        private int? GetSelectedRoutineId()
        {
            if (dgvRoutines.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una rutina primero.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return null;
            }

            return Convert.ToInt32(
                dgvRoutines.SelectedRows[0]
                    .Cells["Id"].Value
            );
        }

        // Actualizar tabla
        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadRoutines();
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
