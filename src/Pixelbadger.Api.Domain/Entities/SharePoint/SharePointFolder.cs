namespace Pixelbadger.Api.Domain.Entities.SharePoint;

public class SharePointFolder
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string WebUrl { get; set; } = string.Empty;
    public string ParentPath { get; set; } = string.Empty;
    public int ChildCount { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime LastModifiedDateTime { get; set; }
}