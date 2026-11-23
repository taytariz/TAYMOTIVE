
import json
import shutil
from collections import Counter
from pathlib import Path

# Rutas del proyecto
BASE_DIR = Path(__file__).resolve().parent
USERS_PATH = BASE_DIR / "Files" / "users.json"
TRAINERS_PATH = BASE_DIR / "Files" / "trainers.json"
BACKUP_PATH = BASE_DIR / "Backups" / "users_antes_asignacion_python.json"
TEMP_PATH = BASE_DIR / "Files" / "users_temp.json"


def main():
    # Leer los archivos JSON
    with USERS_PATH.open("r", encoding="utf-8-sig") as file:
        users = json.load(file)

    with TRAINERS_PATH.open("r", encoding="utf-8-sig") as file:
        trainers = json.load(file)

    # Verificar que sean listas
    if not isinstance(users, list):
        raise ValueError("users.json no contiene una lista.")

    if not isinstance(trainers, list):
        raise ValueError("trainers.json no contiene una lista.")

    # Obtener los IDs de entrenadores
    trainer_ids = sorted(trainer["Id"] for trainer in trainers)

    if not trainer_ids:
        raise ValueError("No existen entrenadores registrados.")

    if len(trainer_ids) != len(set(trainer_ids)):
        raise ValueError("Hay IDs de entrenadores duplicados.")

    # Obtener todas las rutinas
    routines = [
        routine
        for user in users
        for routine in (user.get("Routines") or [])
    ]

    # Validar asignaciones existentes
    for routine in routines:
        trainer_id = routine.get("TrainerId", 0) or 0

        if trainer_id > 0 and trainer_id not in trainer_ids:
            raise ValueError(
                f"La rutina {routine['Id']} tiene "
                "un entrenador inexistente."
            )

    # Contar rutinas asignadas por entrenador
    assignments = Counter({
        trainer_id: 0 for trainer_id in trainer_ids
    })

    for routine in routines:
        trainer_id = routine.get("TrainerId", 0) or 0

        if trainer_id > 0:
            assignments[trainer_id] += 1

    # Distribuir las rutinas pendientes
    assigned_count = 0

    for routine in routines:
        trainer_id = routine.get("TrainerId", 0) or 0

        if trainer_id <= 0:
            selected_trainer = min(
                trainer_ids,
                key=lambda trainer_id: (
                    assignments[trainer_id],
                    trainer_id
                )
            )

            routine["TrainerId"] = selected_trainer
            assignments[selected_trainer] += 1
            assigned_count += 1

    # Crear respaldo antes de guardar
    BACKUP_PATH.parent.mkdir(
        parents=True,
        exist_ok=True
    )

    shutil.copy2(USERS_PATH, BACKUP_PATH)

    # Guardar primero en un archivo temporal
    with TEMP_PATH.open("w", encoding="utf-8") as file:
        json.dump(
            users,
            file,
            ensure_ascii=False,
            indent=2
        )

    # Verificar el archivo generado
    with TEMP_PATH.open("r", encoding="utf-8") as file:
        verified_users = json.load(file)

    if not isinstance(verified_users, list):
        raise ValueError(
            "El archivo generado no contiene una lista."
        )

    if len(verified_users) != len(users):
        raise ValueError(
            "Cambió la cantidad de usuarios."
        )

    verified_routines = sum(
        len(user.get("Routines") or [])
        for user in verified_users
    )

    if verified_routines != len(routines):
        raise ValueError(
            "Cambió la cantidad de rutinas."
        )

    # Reemplazar el archivo original
    TEMP_PATH.replace(USERS_PATH)

    # Mostrar resultados
    print("\nAsignación completada correctamente.")
    print(f"Usuarios: {len(users)}")
    print(f"Entrenadores: {len(trainers)}")
    print(f"Rutinas: {len(routines)}")
    print(f"Nuevas asignaciones: {assigned_count}")

    assigned_routines = sum(
        1 for routine in routines
        if routine.get("TrainerId", 0) > 0
    )

    print(f"Rutinas con entrenador: {assigned_routines}")
    print(
        "Entrenadores utilizados: "
        f"{sum(1 for count in assignments.values() if count > 0)}"
    )


if __name__ == "__main__":
    try:
        main()
    except Exception as error:
        print(f"\nERROR: {error}")
        print("No se completó la asignación.")
