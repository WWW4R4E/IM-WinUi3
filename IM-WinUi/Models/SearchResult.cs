using System.Collections.Generic;

namespace IMWinUi.Models;

public class SearchResult
{
    public bool Success { get; set; }
    public string Type { get; set; }
    public List<ResultInformation>? Result { get; set; }
}
