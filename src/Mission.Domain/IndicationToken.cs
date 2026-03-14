namespace Mission.Domain;

public enum IndicationTokenStatus { Active, Expired, Revoked }

public class IndicationToken
{
    public Guid Id { get; init; }
    public string Token { get; init; } = string.Empty;
    public Guid UserId { get; init; } // Usuário que gerou o token de indicação, ou seja, o usuário que fez a indicação e pode ser elegível para recompensas caso a missão seja completada com sucesso pelo indicado.
    public Guid MissionId { get; init; }
    public IndicationTokenStatus Status { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}
