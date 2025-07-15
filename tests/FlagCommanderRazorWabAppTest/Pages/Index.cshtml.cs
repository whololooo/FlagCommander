using FlagCommander;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlagCommanderRazorWabAppTest.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IFlagCommander _flagCommander;
    
    [BindProperty]
    public bool TestFlagValue { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IFlagCommander flagCommander)
    {
        _logger = logger;
        _flagCommander = flagCommander;
    }

    public async Task OnGet()
    {
        TestFlagValue = await _flagCommander.IsEnabledAsync("test");
    }
}