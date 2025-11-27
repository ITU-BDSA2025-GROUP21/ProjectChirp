using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

public class UITests : PageTest
{
    public async Task AuthenticatedUsersCanSendCheeps()
    {
        // Wait till UI has been implemented
    }

    public async Task AuthenticatedUsersCannotSendCheepsPast160Length()
    {        
        // Wait till UI has been implemented
    }
}