using HelpDesk.Results.Results;

namespace HelpDesk.Results.Results;

public static class HtmlResultExtensions
{
    public static IResult Html(this IResultExtensions extensions, string html)
    {
        return new HtmlResult(html);
    }
}