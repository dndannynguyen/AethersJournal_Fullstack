public class JournalEntryRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Date { get; set; } // Consider using DateTime if you expect a proper date format
}