
using System;
using TaymotiveModels.Models;
using Xunit;

namespace TaymotiveTests
{
    public class MeasurementTests
    {
        // Verificar el cálculo correcto del IMC
        [Fact]
        public void CalculateBMI_ReturnsCorrectValue()
        {
            Measurement measurement = new Measurement(
                1,
                1,
                DateTime.Today,
                70,
                1.75
            );

            double result = measurement.CalculateBMI();

            Assert.Equal(22.86, result, 2);
        }

        // Verificar que el constructor calcule el IMC
        [Fact]
        public void Constructor_CalculatesBMI()
        {
            Measurement measurement = new Measurement(
                2,
                1,
                DateTime.Today,
                80,
                1.80
            );

            Assert.Equal(24.69, measurement.BMI, 2);
        }

        // Evitar división entre cero
        [Fact]
        public void CalculateBMI_WithZeroHeight_ReturnsZero()
        {
            Measurement measurement = new Measurement(
                3,
                1,
                DateTime.Today,
                70,
                0
            );

            Assert.Equal(0, measurement.CalculateBMI());
        }

        // Verificar estatura negativa
        [Fact]
        public void CalculateBMI_WithNegativeHeight_ReturnsZero()
        {
            Measurement measurement = new Measurement(
                4,
                1,
                DateTime.Today,
                70,
                -1.75
            );

            Assert.Equal(0, measurement.CalculateBMI());
        }
    }
}

