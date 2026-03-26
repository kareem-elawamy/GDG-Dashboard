using Microsoft.AspNetCore.Http;

namespace GDG_DashBoard.BLL.Services.Ai;

public interface ICvParserService
{
    Task<string> ParseCvToJsonAsync(IFormFile cvFile);
}
