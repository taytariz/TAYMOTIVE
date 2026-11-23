
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class EditTrainerView : Form
    {
        private readonly Label lblTitle = new Label();

        private readonly TextBox txtName = new TextBox();

        private readonly NumericUpDown numAge = new NumericUpDown();
        private readonly NumericUpDown numWeight = new NumericUpDown();
        private readonly NumericUpDown numHeight = new NumericUpDown();

        private readonly ComboBox cmbSpecialty = new ComboBox();
        private readonly CheckBox chkPersonalTrainer = new CheckBox();

        private readonly Button btnSave = new Button();
        private readonly Button btnCancel = new Button();

        private readonly TrainersController trainersController;
        private readonly int trainerId;

        public EditTrainerView(int id)
        {
            trainerId = id;

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "trainers.json"
            );

            trainersController = new TrainersController(filePath);

            InitializeComponents();
            LoadTrainer();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Modificar entrenador";
            this.Size = new Size(520, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "MODIFICAR ENTRENADOR";
            lblTitle.Font = new Font(
                "Arial",
                18,
                FontStyle.Bold
            );
            lblTitle.Location = new Point(75, 25);
            lblTitle.AutoSize = true;

            AddLabel("Nombre:", 60, 100);
            txtName.Location = new Point(220, 95);
            txtName.Width = 220;

            AddLabel("Edad:", 60, 145);
            numAge.Location = new Point(220, 140);
            numAge.Width = 220;
            numAge.Minimum = 18;
            numAge.Maximum = 100;

            AddLabel("Peso (kg):", 60, 190);
            numWeight.Location = new Point(220, 185);
            numWeight.Width = 220;
            numWeight.Minimum = 1;
            numWeight.Maximum = 500;
            numWeight.DecimalPlaces = 2;

            AddLabel("Estatura (m):", 60, 235);
            numHeight.Location = new Point(220, 230);
            numHeight.Width = 220;
            numHeight.Minimum = 0.50M;
            numHeight.Maximum = 2.50M;
            numHeight.DecimalPlaces = 2;
            numHeight.Increment = 0.01M;

            AddLabel("Especialidad:", 60, 280);
            cmbSpecialty.Location = new Point(220, 275);
            cmbSpecialty.Width = 220;
            cmbSpecialty.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbSpecialty.Items.AddRange(new object[]
            {
                "Musculacion",
                "CrossFit",
                "Yoga",
                "Spinning",
                "Entrenamiento funcional"
            });

            chkPersonalTrainer.Text = "Entrenador personal";
            chkPersonalTrainer.Location = new Point(220, 320);
            chkPersonalTrainer.AutoSize = true;

            btnSave.Text = "Guardar cambios";
            btnSave.Location = new Point(95, 380);
            btnSave.Size = new Size(140, 40);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancelar";
            btnCancel.Location = new Point(265, 380);
            btnCancel.Size = new Size(140, 40);
            btnCancel.Click += BtnCancel_Click;

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtName);
            this.Controls.Add(numAge);
            this.Controls.Add(numWeight);
            this.Controls.Add(numHeight);
            this.Controls.Add(cmbSpecialty);
            this.Controls.Add(chkPersonalTrainer);
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

        // Cargar datos del entrenador seleccionado
        private void LoadTrainer()
        {
            try
            {
                Trainer? trainer =
                    trainersController.GetTrainerById(trainerId);

                if (trainer == null)
                {
                    MessageBox.Show(
                        "No se encontró el entrenador.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    btnSave.Enabled = false;
                    return;
                }

                txtName.Text = trainer.Name;

                numAge.Value = Math.Clamp(
                    (decimal)trainer.Age,
                    numAge.Minimum,
                    numAge.Maximum
                );

                numWeight.Value = Math.Clamp(
                    (decimal)trainer.Weight,
                    numWeight.Minimum,
                    numWeight.Maximum
                );

                numHeight.Value = Math.Clamp(
                    (decimal)trainer.Height,
                    numHeight.Minimum,
                    numHeight.Maximum
                );

                if (!cmbSpecialty.Items.Contains(trainer.Specialty))
                {
                    cmbSpecialty.Items.Add(trainer.Specialty);
                }

                cmbSpecialty.SelectedItem = trainer.Specialty;

                chkPersonalTrainer.Checked =
                    trainer.IsPersonalTrainer;
            }
            catch (Exception ex)
            {
                btnSave.Enabled = false;

                MessageBox.Show(
                    "Error al cargar entrenador: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Guardar cambios
        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                bool updated = trainersController.UpdateTrainer(
                    trainerId,
                    txtName.Text,
                    (int)numAge.Value,
                    (double)numWeight.Value,
                    (double)numHeight.Value,
                    cmbSpecialty.Text,
                    chkPersonalTrainer.Checked
                );

                if (updated)
                {
                    MessageBox.Show(
                        "Entrenador modificado correctamente.",
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
                        "No se pudo modificar el entrenador. " +
                        "Verifique los datos ingresados.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al modificar entrenador: " + ex.Message,
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

