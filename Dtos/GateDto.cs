namespace PortHub.Api.Dtos;

public record RequestGateDto(string Name, string Location);
public record ResponseGateDto(int Id,string Name, string Location);