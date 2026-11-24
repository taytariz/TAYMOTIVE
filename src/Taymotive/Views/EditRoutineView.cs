
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class EditRoutineView : Form
    {
        private readonly Label lblTitle = new Label();

        private readonly TextBox txtName = new TextBox();
        private readonly TextBox txtDescription = new TextBox();

        private readonly NumericUpDown numDuration = new NumericUpDown();
        private readonly NumericUpDown numSets = new NumericUpDown();
        private readonly NumericUpDown numRepetitions = new NumericUpDown();

        private readonly DateTimePicker dtpStartDate = new DateTimePicker();
        private readonly DateTimePicker dtpEndDate = new DateTimePicker();

        private readonly Button btnSave = new Button();
        private readonly Button btnCancel = new Button();

        private readonly RoutinesController routinesController;
        private readonly int routineId;

        public EditRoutineView(int id)
        {
            routineId = id;

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            routinesController = new RoutinesController(filePath);

            InitializeComponents();
            LoadRoutine();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Modificar rutina";
            Size = new Size(550, 660);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MODIFICAR RUTINA";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(125, 25);

            AddLabel("Nombre:", 55, 105);
            txtName.Location = new Point(220, 100);
            txtName.Width = 250;

            AddLabel("Descripción:", 55, 155);
            txtDescription.Location = new Point(220, 150);
            txtDescription.Size = new Size(250, 65);
            txtDescription.Multiline = true;

            AddLabel("Duración (min):", 55, 245);
            numDuration.Location = new Point(220, 240);
            numDuration.Width = 250;
            numDuration.Minimum = 1;
            numDuration.Maximum = 600;

            AddLabel("Series:", 55, 295);
            numSets.Location = new Point(220, 290);
            numSets.Width = 250;
            numSets.Minimum = 1;
            numSets.Maximum = 100;
            numSets.Value = 3;

            AddLabel("Repeticiones:", 55, 345);
            numRepetitions.Location = new Point(220, 340);
            numRepetitions.Width = 250;
            numRepetitions.Minimum = 1;
            numRepetitions.Maximum = 1000;
            numRepetitions.Value = 12;

            AddLabel("Fecha inicio:", 55, 395);
            dtpStartDate.Location = new Point(220, 390);
            dtpStartDate.Width = 250;
            dtpStartDate.Format = DateTimePickerFormat.Short;

            AddLabel("Fecha fin:", 55, 445);
            dtpEndDate.Location = new Point(220, 440);
            dtpEndDate.Width = 250;
            dtpEndDate.Format = DateTimePickerFormat.Short;

            btnSave.Text = "Guardar cambios";
            btnSave.Location = new Point(115, 520);
            btnSave.Size = new Size(140, 40);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancelar";
            btnCancel.Location = new Point(285, 520);
            btnCancel.Size = new Size(140, 40);
            btnCancel.Click += BtnCancel_Click;

            AcceptButton = btnSave;
            CancelButton = btnCancel;

            Controls.Add(lblTitle);
            Controls.Add(txtName);
            Controls.Add(txtDescription);
            Controls.Add(numDuration);
            Controls.Add(numSets);
            Controls.Add(numRepetitions);
            Controls.Add(dtpStartDate);
            Controls.Add(dtpEndDate);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label label = new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true
            };

            Controls.Add(label);
        }

        private void LoadRoutine()
        {
            try
            {
                Routine? routine =
                    routinesController.GetRoutineById(routineId);

                if (routine == null)
                {
                    MessageBox.Show(
                        "No se encontró la rutina.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    btnSave.Enabled = false;
                    return;
                }

                txtName.Text = routine.Name;
                txtDescription.Text = routine.Description;

                numDuration.Value = Math.Clamp(
                    (decimal)routine.Duration,
                    numDuration.Minimum,
                    numDuration.Maximum
                );

                numSets.Value = Math.Clamp(
                    (decimal)routine.Sets,
                    numSets.Minimum,
                    numSets.Maximum
                );

                numRepetitions.Value = Math.Clamp(
                    (decimal)routine.Repetitions,
                    numRepetitions.Minimum,
                    numRepetitions.Maximum
                );

                dtpStartDate.Value = routine.StartDate;
                dtpEndDate.Value = routine.EndDate;
            }
            catch (Exception ex)
            {
                btnSave.Enabled = false;

                MessageBox.Show(
                    "Error al cargar la rutina: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el nombre de la rutina.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtName.Focus();
                return;
            }

            if (dtpEndDate.Value.Date < dtpStartDate.Value.Date)
            {
                MessageBox.Show(
                    "La fecha final no puede ser anterior " +
                    "a la fecha de inicio.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                bool updated = routinesController.UpdateRoutine(
                    routineId,
                    txtName.Text,
                    txtDescription.Text,
                    (int)numDuration.Value,
                    dtpStartDate.Value.Date,
                    dtpEndDate.Value.Date,
                    (int)numSets.Value,
                    (int)numRepetitions.Value
                );

                if (updated)
                {
                    MessageBox.Show(
                        "Rutina modificada correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo modificar la rutina.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al modificar la rutina: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
