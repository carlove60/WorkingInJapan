namespace WaitingList.Contracts.DTOs;

public class SseDto<T>
{
    public string Name { get; set; }
    
    public T Dto { get; set; }
    
    public SseDto(T dto, string name)
    {
        Dto = dto;
        Name = name;
    }
}