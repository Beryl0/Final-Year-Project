namespace CM4700.Api.Repository
{
    using CM4700.Api.Data;
    using CM4700.Api.Repository.Interfaces;
    using Deque.AxeCore.Commons;
    using Deque.AxeCore.Playwright;
    using Microsoft.Playwright;

    public class BaselineAccessibilityScanner : IBaselineAccessibilityScanner
    {
        public async Task<List<BaselineFinding>> ScanAsync(int scanRequestId, string url)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(scanRequestId);
            ArgumentException.ThrowIfNullOrWhiteSpace(url);

            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                throw new ArgumentException("The URL must be a valid absolute URI.", nameof(url));
            }

            using IPlaywright playwright = await Playwright.CreateAsync();

            await using IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            await using IPage page = await browser.NewPageAsync();

            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            AxeResult axeResult = await page.RunAxe();
            DateTime createdAt = DateTime.UtcNow;

            List<BaselineFinding> findings = new();

            foreach (AxeResultItem violation in axeResult.Violations)
            {
                foreach (AxeResultNode? node in violation.Nodes)
                {
                    if (node is null)
                    {
                        continue;
                    }

                    findings.Add(CreateFinding(scanRequestId, violation, node, createdAt));
                }
            }

            return findings;
        }

        private static BaselineFinding CreateFinding(int scanRequestId, AxeResultItem violation, AxeResultNode node, DateTime createdAt)
        {
            return new BaselineFinding
            {
                ScanRequestId = scanRequestId,
                RuleId = violation.Id ?? string.Empty,
                Impact = violation.Impact ?? string.Empty,
                Description = violation.Description ?? string.Empty,
                Help = violation.Help ?? string.Empty,
                HelpUrl = violation.HelpUrl ?? string.Empty,
                ElementHtml = node.Html ?? string.Empty,
                Target = node.Target?.ToString() ?? string.Empty,
                CreatedAt = createdAt
            };
        }
    }
}
