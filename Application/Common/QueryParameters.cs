namespace TaskFlow.Application.Common;

public abstract class QueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    
    public int Page { get; set; } = 1;
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }
    
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public string? SearchTerm { get; set; }
}