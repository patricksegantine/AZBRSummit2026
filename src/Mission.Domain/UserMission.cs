namespace Mission.Domain;

public enum UserMissionStatus { Pending, Completed }

public class UserMission
{
    public Guid Id { get; init; }

    // Para missões do tipo MGM, necessário associar
    public Guid? IndicationTokenId { get; init; }
    
    // Usuário que foi indicado, ou seja, o usuário
    // que completou a missão e pode ser elegível
    // para recompensas caso a missão seja completada com sucesso.
    public Guid? ReferredUserId { get; init; } 

    // Para missões do tipo Quiz, necessário associar a um quiz específico
    public Guid? QuizAnswerId { get; init; }

    // Para associar a missão a um treinamento específico
    public Guid? TrainingId { get; init; }

    public UserMissionStatus Status { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; set; }
}
