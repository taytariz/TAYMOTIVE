
using System;
using System.IO;
using TaymotiveController.Controllers;
using Xunit;

namespace TaymotiveTests
{
    public class ControllerValidationTests
    {
        // Rechazar mediciones con peso negativo
        [Fact]
        public void RegisterMeasurement_NegativeWeight_ReturnsFalse()
        {
            string filePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid() + ".json"
            );

            MeasurementsController controller =
                new MeasurementsController(filePath);

            bool result = controller.RegisterMeasurement(
                1,
                DateTime.Today,
                -70,
                1.75
            );

            Assert.False(result);
            Assert.False(File.Exists(filePath));
        }

        // Rechazar mediciones con estatura cero
        [Fact]
        public void RegisterMeasurement_ZeroHeight_ReturnsFalse()
        {
            string filePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid() + ".json"
            );

            MeasurementsController controller =
                new MeasurementsController(filePath);

            bool result = controller.RegisterMeasurement(
                1,
                DateTime.Today,
                70,
                0
            );

            Assert.False(result);
            Assert.False(File.Exists(filePath));
        }

        // Rechazar objetivos con fechas incorrectas
        [Fact]
        public void RegisterGoal_InvalidDates_ReturnsFalse()
        {
            string filePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid() + ".json"
            );

            FitnessGoalsController controller =
                new FitnessGoalsController(filePath);

            bool result = controller.RegisterGoal(
                1,
                "Ganar musculatura",
                60,
                70,
                DateTime.Today.AddDays(30),
                DateTime.Today
            );

            Assert.False(result);
            Assert.False(File.Exists(filePath));
        }

        // Rechazar objetivos con valores negativos
        [Fact]
        public void RegisterGoal_NegativeValue_ReturnsFalse()
        {
            string filePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid() + ".json"
            );

            FitnessGoalsController controller =
                new FitnessGoalsController(filePath);

            bool result = controller.RegisterGoal(
                1,
                "Ganar musculatura",
                -60,
                70,
                DateTime.Today,
                DateTime.Today.AddDays(30)
            );

            Assert.False(result);
            Assert.False(File.Exists(filePath));
        }
    }
}
