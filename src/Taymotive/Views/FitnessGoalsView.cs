
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class FitnessGoalsView : Form
    {
        private readonly Label lblTitle = new Label();
        private readonly Label lblTotal = new Label();
        private readonly DataGridView dgvGoals = new DataGridView();

        private readonly Button btnRegister = new Button();
        private readonly Button btnEdit = new Button();
        private readonly Button btnDelete = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private readonly FitnessGoalsController goalsController;
        private readonly UsersController usersController;

        public FitnessGoalsView()
        {
            string goalsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "fitnessgoals.json"
            );

            string usersPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            goalsController = new FitnessGoalsController(goalsPath);
            usersController = new UsersController(usersPath);

            InitializeComponents();
            LoadGoals();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Gestión de objetivos físicos";
            Size = new Size(1050, 600);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "GESTIÓN DE OBJETIVOS FÍSICOS";
            lblTitle.Font = new Font("Arial", 20, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(265, 30);

            lblTotal.Text = "Total de objetivos: 0";
            lblTotal.Location = new Point(35, 100);
            lblTotal.AutoSize = true;

            dgvGoals.Location = new Point(35, 135);
            dgvGoals.Size = new Size(960, 330);
            dgvGoals.ReadOnly = true;
            dgvGoals.AllowUserToAddRows = false;
            dgvGoals.AllowUserToDeleteRows = false;
            dgvGoals.MultiSelect = false;
            dgvGoals.RowHeadersVisible = false;

            dgvGoals.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvGoals.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvGoals.Columns.Add("Id", "ID");
            dgvGoals.Columns.Add("User", "Usuario");
            dgvGoals.Columns.Add("Goal", "Objetivo");
            dgvGoals.Columns.Add("Initial", "Inicial");
            dgvGoals.Columns.Add("Target", "Meta");
            dgvGoals.Columns.Add("Current", "Actual");
            dgvGoals.Columns.Add("Progress", "Progreso (%)");
            dgvGoals.Columns.Add("Status", "Estado");

            ConfigureButton(
                btnRegister, "Registrar objetivo", 35);
            ConfigureButton(
                btnEdit, "Modificar objetivo", 230);
            ConfigureButton(
                btnDelete, "Eliminar objetivo", 425);
            ConfigureButton(
                btnRefresh, "Actualizar", 620);
            ConfigureButton(
                btnClose, "Cerrar", 815);

            btnRegister.Click += BtnRegister_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => LoadGoals();
            btnClose.Click += (s, e) => Close();

            Controls.Add(lblTitle);
            Controls.Add(lblTotal);
            Controls.Add(dgvGoals);
            Controls.Add(btnRegister);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);
        }

        private static void ConfigureButton(
            Button button, string text, int x)
        {
            button.Text = text;
            button.Location = new Point(x, 495);
            button.Size = new Size(180, 40);
        }

        private void LoadGoals()
        {
            try
            {
                dgvGoals.Rows.Clear();

                var goals = goalsController.GetGoals();
                var users = usersController.GetUsers();

                foreach (var goal in goals)
                {
                    var user = users.Find(
                        u => u.Id == goal.UserId);

                    string userName = user != null
                        ? user.Name
                        : "Usuario no encontrado";

                    dgvGoals.Rows.Add(
                        goal.Id,
                        userName,
                        goal.GoalType,
                        goal.InitialValue.ToString("F2"),
                        goal.TargetValue.ToString("F2"),
                        goal.CurrentValue.ToString("F2"),
                        goal.CalculateProgress().ToString("F2"),
                        goal.IsCompleted
                            ? "Completado"
                            : "En progreso"
                    );
                }

                lblTotal.Text =
                    "Total de objetivos: " + goals.Count;

                dgvGoals.ClearSelection();
            }
            catch (Exception ex)
            {
                ShowError("Error al cargar objetivos", ex);
            }
        }

        private int? GetSelectedGoalId()
        {
            if (dgvGoals.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un objetivo.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return null;
            }

            return Convert.ToInt32(
                dgvGoals.SelectedRows[0].Cells["Id"].Value);
        }

        private void BtnRegister_Click(
            object? sender, EventArgs e)
        {
            OpenGoalForm(null);
        }

        private void BtnEdit_Click(
            object? sender, EventArgs e)
        {
            int? id = GetSelectedGoalId();

            if (id.HasValue)
            {
                OpenGoalForm(id.Value);
            }
        }

        private void OpenGoalForm(int? goalId)
        {
            try
            {
                FitnessGoal? existingGoal = goalId.HasValue
                    ? goalsController.GetGoalById(goalId.Value)
                    : null;

                if (goalId.HasValue && existingGoal == null)
                {
                    MessageBox.Show(
                        "No se encontró el objetivo.",
                        "TAYMOTIVE");
                    return;
                }

                var users = usersController.GetUsers();

                if (users.Count == 0)
                {
                    MessageBox.Show(
                        "Primero debe registrar usuarios.",
                        "TAYMOTIVE");
                    return;
                }

                using Form form = new Form
                {
                    Text = existingGoal == null
                        ? "TAYMOTIVE - Registrar objetivo"
                        : "TAYMOTIVE - Modificar objetivo",
                    Size = new Size(520, 550),
                    StartPosition =
                        FormStartPosition.CenterParent,
                    FormBorderStyle =
                        FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                ComboBox cboUsers = new ComboBox
                {
                    Location = new Point(210, 45),
                    Size = new Size(250, 28),
                    DropDownStyle =
                        ComboBoxStyle.DropDownList
                };

                foreach (var user in users)
                {
                    cboUsers.Items.Add(new UserOption(
                        user.Id, user.Name));
                }

                ComboBox cboType = new ComboBox
                {
                    Location = new Point(210, 95),
                    Size = new Size(250, 28),
                    DropDownStyle =
                        ComboBoxStyle.DropDown
                };

                cboType.Items.AddRange(new object[]
                {
                    "Ganar musculatura",
                    "Perder grasa",
                    "Mantener peso",
                    "Aumentar visitas",
                    "Mejorar condición física"
                });

                NumericUpDown nudInitial = CreateNumeric(145);
                NumericUpDown nudTarget = CreateNumeric(195);
                NumericUpDown nudCurrent = CreateNumeric(245);

                DateTimePicker dtpStart = new DateTimePicker
                {
                    Location = new Point(210, 295),
                    Size = new Size(250, 28),
                    Format = DateTimePickerFormat.Short
                };

                DateTimePicker dtpEnd = new DateTimePicker
                {
                    Location = new Point(210, 345),
                    Size = new Size(250, 28),
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today.AddMonths(1)
                };

                AddField(form, "Usuario:", 45, cboUsers);
                AddField(form, "Tipo de objetivo:", 95, cboType);
                AddField(form, "Valor inicial:", 145, nudInitial);
                AddField(form, "Valor meta:", 195, nudTarget);
                AddField(form, "Valor actual:", 245, nudCurrent);
                AddField(form, "Fecha inicio:", 295, dtpStart);
                AddField(form, "Fecha fin:", 345, dtpEnd);

                if (existingGoal != null)
                {
                    for (int i = 0; i < cboUsers.Items.Count; i++)
                    {
                        if (((UserOption)cboUsers.Items[i]!)
                            .Id == existingGoal.UserId)
                        {
                            cboUsers.SelectedIndex = i;
                            break;
                        }
                    }

                    cboType.Text = existingGoal.GoalType;
                    nudInitial.Value =
                        ClampValue(nudInitial, existingGoal.InitialValue);
                    nudTarget.Value =
                        ClampValue(nudTarget, existingGoal.TargetValue);
                    nudCurrent.Value =
                        ClampValue(nudCurrent, existingGoal.CurrentValue);
                    dtpStart.Value = existingGoal.StartDate;
                    dtpEnd.Value = existingGoal.EndDate;
                }
                else
                {
                    cboUsers.SelectedIndex = 0;
                    cboType.SelectedIndex = 0;
                }

                Button btnSave = new Button
                {
                    Text = "Guardar",
                    Location = new Point(100, 420),
                    Size = new Size(140, 40)
                };

                Button btnCancel = new Button
                {
                    Text = "Cancelar",
                    Location = new Point(265, 420),
                    Size = new Size(140, 40),
                    DialogResult = DialogResult.Cancel
                };

                btnSave.Click += (s, e) =>
                {
                    if (cboUsers.SelectedItem is not UserOption selectedUser)
                    {
                        MessageBox.Show("Seleccione un usuario.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(cboType.Text))
                    {
                        MessageBox.Show("Ingrese el tipo de objetivo.");
                        return;
                    }

                    if (dtpStart.Value.Date > dtpEnd.Value.Date)
                    {
                        MessageBox.Show(
                            "La fecha final debe ser igual o posterior a la inicial.");
                        return;
                    }

                    try
                    {
                        bool saved;

                        if (existingGoal == null)
                        {
                            saved = goalsController.RegisterGoal(
                                selectedUser.Id,
                                cboType.Text,
                                (double)nudInitial.Value,
                                (double)nudTarget.Value,
                                dtpStart.Value,
                                dtpEnd.Value);

                            if (saved)
                            {
                                int newId = goalsController.GetGoals()
                                    .Max(g => g.Id);

                                saved = goalsController.UpdateGoalProgress(
                                    newId,
                                    (double)nudCurrent.Value);
                            }
                        }
                        else
                        {
                            saved = goalsController.UpdateGoal(
                                existingGoal.Id,
                                selectedUser.Id,
                                cboType.Text,
                                (double)nudInitial.Value,
                                (double)nudTarget.Value,
                                dtpStart.Value,
                                dtpEnd.Value);

                            if (saved)
                            {
                                saved = goalsController.UpdateGoalProgress(
                                    existingGoal.Id,
                                    (double)nudCurrent.Value);
                            }
                        }

                        if (saved)
                        {
                            form.DialogResult = DialogResult.OK;
                            form.Close();
                        }
                        else
                        {
                            MessageBox.Show(
                                "No se pudo guardar el objetivo.",
                                "TAYMOTIVE");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError("Error al guardar objetivo", ex);
                    }
                };

                form.Controls.Add(btnSave);
                form.Controls.Add(btnCancel);
                form.CancelButton = btnCancel;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadGoals();
                }
            }
            catch (Exception ex)
            {
                ShowError("Error al abrir formulario", ex);
            }
        }

        private NumericUpDown CreateNumeric(int y)
        {
            return new NumericUpDown
            {
                Location = new Point(210, y),
                Size = new Size(250, 28),
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1000000,
                Increment = 0.5M
            };
        }

        private static decimal ClampValue(
            NumericUpDown control, double value)
        {
            decimal number = (decimal)value;

            return Math.Min(
                control.Maximum,
                Math.Max(control.Minimum, number));
        }

        private void AddField(
            Form form, string title, int y, Control input)
        {
            Label label = new Label
            {
                Text = title,
                Location = new Point(35, y + 4),
                Size = new Size(160, 25)
            };

            form.Controls.Add(label);
            form.Controls.Add(input);
        }

        private void BtnDelete_Click(
            object? sender, EventArgs e)
        {
            int? id = GetSelectedGoalId();

            if (!id.HasValue)
            {
                return;
            }

            DialogResult confirmation = MessageBox.Show(
                "¿Está seguro de que desea eliminar este objetivo?\n\n" +
                "ID: " + id.Value + "\n\n" +
                "Esta acción no se puede deshacer.",
                "TAYMOTIVE - Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (goalsController.DeleteGoal(id.Value))
                {
                    MessageBox.Show(
                        "Objetivo eliminado correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadGoals();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo eliminar el objetivo.",
                        "TAYMOTIVE");
                }
            }
            catch (Exception ex)
            {
                ShowError("Error al eliminar objetivo", ex);
            }
        }

        private static void ShowError(string message, Exception ex)
        {
            MessageBox.Show(
                message + ": " + ex.Message,
                "TAYMOTIVE",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private sealed class UserOption
        {
            public int Id { get; }
            public string Name { get; }

            public UserOption(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
