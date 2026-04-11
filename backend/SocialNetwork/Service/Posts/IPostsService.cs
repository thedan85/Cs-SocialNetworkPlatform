using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public interface IPostsService
{
    Task<ServiceResult<IReadOnlyList<PostResponse>>> GetPostsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<ServiceResult<PostResponse>> GetPostByIdAsync(string postId, CancellationToken ct = default);
    Task<ServiceResult<PostResponse>> CreatePostAsync(string actorUserId, PostCreateRequest request, CancellationToken ct = default);
    Task<ServiceResult<PostResponse>> UpdatePostAsync(
        string actorUserId,
        string postId,
        PostUpdateRequest request,
        bool isAdmin,
        CancellationToken ct = default);
    Task<ServiceResult<string>> DeletePostAsync(
        string actorUserId,
        string postId,
        bool isAdmin,
        CancellationToken ct = default);

    Task<ServiceResult<IReadOnlyList<CommentResponse>>> GetPostCommentsAsync(
        string postId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<ServiceResult<CommentResponse>> CreateCommentAsync(
        string actorUserId,
        string postId,
        CommentCreateRequest request,
        CancellationToken ct = default);
    Task<ServiceResult<string>> DeleteCommentAsync(string postId, string commentId, CancellationToken ct = default);

    Task<ServiceResult<LikePostResult>> LikePostAsync(
        string actorUserId,
        string postId,
        LikeCreateRequest request,
        CancellationToken ct = default);
    Task<ServiceResult<PostReportResponse>> ReportPostAsync(
        string actorUserId,
        string postId,
        PostReportCreateRequest request,
        CancellationToken ct = default);
}
