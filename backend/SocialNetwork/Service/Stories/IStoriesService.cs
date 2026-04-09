using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public interface IStoriesService
{
    Task<ServiceResult<IReadOnlyList<StoryResponse>>> GetStoriesAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default);

    Task<ServiceResult<StoryResponse>> GetStoryByIdAsync(string storyId, CancellationToken ct = default);

    Task<ServiceResult<IReadOnlyList<StoryResponse>>> GetStoriesForUserAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default);

    Task<ServiceResult<StoryResponse>> CreateStoryAsync(
        string actorUserId,
        StoryCreateRequest request,
        CancellationToken ct = default);
    Task<ServiceResult<string>> DeleteStoryAsync(string storyId, CancellationToken ct = default);
}
