using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using TIKSN.DependencyInjection;
using TIKSN.Mapping;

var services = new ServiceCollection();
services.AddFrameworkCore();

// Register Mappers
services.AddSingleton<IMapper<UserEntity, UserDto>, UserEntityToDtoMapper>();
services.AddSingleton<IMapper<UserDto, UserEntity>, UserDtoToEntityMapper>();

await using var serviceProvider = services.BuildServiceProvider();

AnsiConsole.MarkupLine("[bold cyan]TIKSN Mapping Example[/]");
AnsiConsole.MarkupLine("[grey]-----------------------------------------[/]");

var entity = new UserEntity { Id = Guid.NewGuid(), FullName = "John Doe", DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
AnsiConsole.MarkupLine($"[yellow]Original Entity:[/] {entity.Id} - {entity.FullName}");

// Map Entity to DTO
var entityToDtoMapper = serviceProvider.GetRequiredService<IMapper<UserEntity, UserDto>>();
var dto = entityToDtoMapper.Map(entity);
AnsiConsole.MarkupLine($"[green]Mapped to DTO:[/] {dto.UserId} - {dto.Name} (Age: {dto.Age})");

// Map DTO back to Entity
var dtoToEntityMapper = serviceProvider.GetRequiredService<IMapper<UserDto, UserEntity>>();
var newEntity = dtoToEntityMapper.Map(dto);
AnsiConsole.MarkupLine($"[fuchsia]Mapped back to Entity:[/] {newEntity.Id} - {newEntity.FullName}");

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable S3903 // Types should be defined in named namespaces
#pragma warning disable RCS1110 // Declare type inside namespace
public class UserEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

public class UserDto
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class UserEntityToDtoMapper : IMapper<UserEntity, UserDto>
{
    public UserDto Map(UserEntity source)
    {
        return new UserDto
        {
            UserId = source.Id.ToString(),
            Name = source.FullName,
            Age = DateTime.Now.Year - source.DateOfBirth.Year
        };
    }
}

public class UserDtoToEntityMapper : IMapper<UserDto, UserEntity>
{
    public UserEntity Map(UserDto source)
    {
        return new UserEntity
        {
            Id = Guid.TryParse(source.UserId, out var id) ? id : Guid.Empty,
            FullName = source.Name,
            // Date of birth is lost in DTO, we can't accurately recover it
            DateOfBirth = DateTime.Now.AddYears(-source.Age)
        };
    }
}
#pragma warning restore RCS1110 // Declare type inside namespace
#pragma warning restore S3903 // Types should be defined in named namespaces
#pragma warning restore CA1050 // Declare types in namespaces
