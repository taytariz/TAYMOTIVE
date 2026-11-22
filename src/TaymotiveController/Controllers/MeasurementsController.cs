
using System;
using System.Collections.Generic;
using System.Linq;
using TaymotiveController.Data;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class MeasurementsController
    {
        private readonly FileHandler<Measurement> fileHandler;

        public MeasurementsController(string filePath)
        {
            fileHandler = new FileHandler<Measurement>(filePath);
        }

        // Obtener todas las mediciones
        public List<Measurement> GetMeasurements()
        {
            return fileHandler.Read();
        }

        // Buscar una medición por ID
        public Measurement? GetMeasurementById(int id)
        {
            List<Measurement> measurements = fileHandler.Read();

            return measurements.FirstOrDefault(
                measurement => measurement.Id == id
            );
        }

        // Registrar una nueva medición
        public bool RegisterMeasurement(
            int userId,
            DateTime measurementDate,
            double weight,
            double height)
        {
            if (userId <= 0 || weight <= 0 || height <= 0)
            {
                return false;
            }

            List<Measurement> measurements = fileHandler.Read();

            int newId = measurements.Count > 0
                ? measurements.Max(m => m.Id) + 1
                : 1;

            Measurement newMeasurement = new Measurement(
                newId,
                userId,
                measurementDate,
                weight,
                height
            );

            measurements.Add(newMeasurement);

            fileHandler.Save(measurements);

            return true;
        }

        // Modificar una medición existente
        public bool UpdateMeasurement(
            int id,
            int userId,
            DateTime measurementDate,
            double weight,
            double height)
        {
            if (userId <= 0 || weight <= 0 || height <= 0)
            {
                return false;
            }

            List<Measurement> measurements = fileHandler.Read();

            Measurement? measurement = measurements.FirstOrDefault(
                m => m.Id == id
            );

            if (measurement == null)
            {
                return false;
            }

            measurement.UserId = userId;
            measurement.MeasurementDate = measurementDate;
            measurement.Weight = weight;
            measurement.Height = height;

            measurement.BMI = measurement.CalculateBMI();

            fileHandler.Save(measurements);

            return true;
        }

        // Eliminar una medición
        public bool DeleteMeasurement(int id)
        {
            List<Measurement> measurements = fileHandler.Read();

            Measurement? measurement = measurements.FirstOrDefault(
                m => m.Id == id
            );

            if (measurement == null)
            {
                return false;
            }

            measurements.Remove(measurement);

            fileHandler.Save(measurements);

            return true;
        }
    }
}
