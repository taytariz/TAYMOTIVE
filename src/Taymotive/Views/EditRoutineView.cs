
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
            this.Text = "TAYMOTIVE - Modificar rutina";
            this.Size = new Size(550, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "MODIFICAR RUTINA";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(125, 25);

            AddLabel("Nombre:", 55, 110);
            txtName.Location = new Point(220, 105);
            txtName.Width = 250;

            AddLabel("Descripción:", 55, 165);
            txtDescription.Location = new Point(220, 160);
            txtDescription.Size = new Size(250, 65);
            txtDescription.Multiline = true;

            AddLabel("Duración (min):", 55, 255);
            numDuration.Location = new Point(220, 250);
            numDuration.Width = 250;
            numDuration.Minimum = 1;
            numDuration.Maximum = 600;

            AddLabel("Fecha inicio:", 55, 310);
            dtpStartDate.Location = new Point(220, 305);
            dtpStartDate.Width = 250;
            dtpStartDate.Format = DateTimePickerFormat.Short;

            AddLabel("Fecha fin:", 55, 365);
            dtpEndDate.Location = new Point(220, 360);
            dtpEndDate.Width = 250;
            dtpEndDate.Format = DateTimePickerFormat.Short;

            btnSave.Text = "Guardar cambios";
            btnSave.Location = new Point(115, 415);
            btnSave.Size = new Size(140, 40);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancelar";
            btnCancel.Location = new Point(285, 415);
            btnCancel.Size = new Size(140, 40);
            btnCancel.Click += BtnCancel_Click;

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtName);
            this.Controls.Add(txtDescription);
            this.Controls.Add(numDuration);
            this.Controls.Add(dtpStartDate);
            this.Controls.Add(dtpEndDate);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label label = new Label();

            label.Text = text;
            label.Location = new Point(x, y);
            label.AutoSize = true;

            this.Controls.Add(label);
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
                    dtpEndDate.Value.Date
                );

                if (updated)
                {
                    MessageBox.Show(
                        "Rutina modificada correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    this.DialogResult = DialogResult.OK;
                    this.Close();
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
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
