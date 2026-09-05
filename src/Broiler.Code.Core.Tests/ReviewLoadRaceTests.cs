using Broiler.Code.Core.Review;
using Broiler.Code.Review;
using Broiler.Code.Workspaces;
using Broiler.Code.Workspaces.Model;
using Broiler.Code.Workspaces.Storage;
using Broiler.UI;

namespace Broiler.Code.Core.Tests;

/// <summary>
/// What happens to a review recorded while the workspace-wide load is still in
/// flight.
///
/// The shell starts that load from AttachWorkspace and does not wait for it, so
/// the very first thing a reviewer does in a freshly opened folder races it. The
/// load reads a snapshot and publishes it whole; if it published over a decision
/// recorded after the read, the record on disk would be right while the explorer
/// badge and the coverage number said the file was untouched — and because the
/// snapshot it came from was empty, nothing would put the state back.
///
/// Ordering here is driven, not waited for: the dispatcher holds the publish
/// until the test releases it, so the interleaving that used to fail once in a
/// while on a loaded CI runner happens every run.
/// </summary>
public sealed class ReviewLoadRaceTests : IDisposable
{
    private const string AlphaSource = "class Alpha { }\n";

    private readonly string _root = Path.Combine(
        Path.GetTempPath(), "broiler-review-race", Guid.NewGuid().ToString("n"));

    public ReviewLoadRaceTests()
    {
        Directory.CreateDirectory(Path.Combine(_root, "src"));
        File.WriteAllText(Path.Combine(_root, "src", "Alpha.cs"), AlphaSource);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_root))
                Directory.Delete(_root, recursive: true);
        }
        catch (IOException)
        {
        }
    }

    [Fact(Timeout = 600000)]
    public async Task A_Review_Recorded_While_The_Load_Is_In_Flight_Survives_It()
    {
        (ReviewController review, HeldDispatcher dispatcher) = await CreateAsync();

        // The load a freshly attached workspace starts, before anything is
        // reviewed: it finds no records at all.
        await review.LoadAsync();
        Assert.Equal(1, dispatcher.Pending);

        Assert.True((await review.RecordDecisionAsync(ReviewStatus.Reviewed)).Succeeded);
        Assert.True(review.StateFor("src/Alpha.cs").IsVerified);

        // The load lands afterwards, carrying its now-stale empty snapshot.
        dispatcher.Release();

        Assert.True(review.StateFor("src/Alpha.cs").IsVerified);
        Assert.Equal(ReviewStatus.Reviewed, review.ReviewFor("src/Alpha.cs").Status);
        review.Dispose();
    }

    /// <summary>
    /// The same protection in the other direction. Withdrawing an approval is
    /// removal, not a value, so a snapshot taken before it must not put the record
    /// back — an approval the reviewer took away is the one thing worse to
    /// resurrect than one it forgets.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task A_Review_Cleared_While_The_Load_Is_In_Flight_Stays_Cleared()
    {
        (ReviewController review, HeldDispatcher dispatcher) = await CreateAsync();

        Assert.True((await review.RecordDecisionAsync(ReviewStatus.Reviewed)).Succeeded);

        // This load does see the record, so its snapshot is the approval.
        await review.LoadAsync();
        Assert.Equal(1, dispatcher.Pending);

        Assert.True((await review.RecordDecisionAsync(ReviewStatus.Unreviewed)).Succeeded);
        Assert.False(review.StateFor("src/Alpha.cs").IsVerified);

        dispatcher.Release();

        Assert.False(review.StateFor("src/Alpha.cs").IsVerified);
        review.Dispose();
    }

    /// <summary>
    /// The load is not simply ignored once anything has been recorded: a path it
    /// read and nobody has touched since is still published. Without this the fix
    /// could pass the two tests above by dropping loads altogether.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task A_Load_Still_Publishes_The_Paths_Nobody_Touched()
    {
        File.WriteAllText(Path.Combine(_root, "src", "Beta.cs"), "class Beta { }\n");
        (ReviewController review, HeldDispatcher dispatcher) = await CreateAsync();

        // Beta is reviewed through a separate controller, so the one under test
        // learns about it only by loading.
        CodeWorkspace elsewhere = CreateWorkspace();
        var other = new ReviewController(elsewhere, reviewer: "Enrico");
        await OpenAsync(elsewhere, other, "src/Beta.cs");
        Assert.True((await other.RecordDecisionAsync(ReviewStatus.Reviewed)).Succeeded);
        other.Dispose();

        await review.LoadAsync();
        Assert.True((await review.RecordDecisionAsync(ReviewStatus.Reviewed)).Succeeded);
        dispatcher.Release();

        Assert.Equal(ReviewStatus.Reviewed, review.ReviewFor("src/Alpha.cs").Status);
        Assert.Equal(ReviewStatus.Reviewed, review.ReviewFor("src/Beta.cs").Status);
        review.Dispose();
    }

    private async Task<(ReviewController Review, HeldDispatcher Dispatcher)> CreateAsync()
    {
        var dispatcher = new HeldDispatcher();
        CodeWorkspace workspace = CreateWorkspace();
        var review = new ReviewController(workspace, reviewer: "Enrico", dispatcher: dispatcher);

        await OpenAsync(workspace, review, "src/Alpha.cs");
        return (review, dispatcher);
    }

    private CodeWorkspace CreateWorkspace()
    {
        var workspace = new CodeWorkspace(new FileSystemWorkspaceStorage(_root));
        workspace.AddItem("src/Alpha.cs", WorkspaceItemKind.SourceDocument);
        if (File.Exists(Path.Combine(_root, "src", "Beta.cs")))
            workspace.AddItem("src/Beta.cs", WorkspaceItemKind.SourceDocument);
        return workspace;
    }

    private static async Task OpenAsync(
        CodeWorkspace workspace, ReviewController review, string relativePath)
    {
        WorkspaceItem item = workspace.FindItem(relativePath)!;
        Assert.True((await workspace.OpenDocumentAsync(item.Id)).Succeeded);
        review.SetCurrentDocument(item.Id);
    }

    /// <summary>
    /// Holds every posted callback until the test asks for it, which is what turns
    /// "the load might land after the decision" into "the load lands after the
    /// decision".
    ///
    /// CheckAccess answers false until <see cref="Release"/> runs and true while it
    /// does, which is what a real dispatcher looks like from either side: work
    /// posted from off the UI thread is queued, and the same work run on the UI
    /// thread proceeds. Answering false throughout would make a released callback
    /// post itself straight back onto the queue.
    /// </summary>
    private sealed class HeldDispatcher : IUiDispatcher
    {
        private readonly Queue<Action> _posted = new();
        private bool _onUiThread;

        public int Pending => _posted.Count;

        public bool CheckAccess() => _onUiThread;

        public void Post(Action callback) => _posted.Enqueue(callback);

        public void Release()
        {
            _onUiThread = true;
            try
            {
                while (_posted.Count > 0)
                    _posted.Dequeue()();
            }
            finally
            {
                _onUiThread = false;
            }
        }
    }
}
