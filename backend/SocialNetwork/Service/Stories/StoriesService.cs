using SocialNetwork.Dtos;
using SocialNetwork.Extensions;
using SocialNetwork.Model;
using SocialNetwork.Repository;

namespace SocialNetwork.Service;

public class StoriesService : IStoriesService
{
    private readonly IStoryRepository _storyRepository;
    private readonly IUserRepository _userRepository;

    public StoriesService(IStoryRepository storyRepository, IUserRepository userRepository)
    {
        _storyRepository = storyRepository;
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<StoryResponse>>> GetStoriesAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var stories = await _storyRepository.GetActiveAsync(pageNumber, pageSize, ct);
        var responses = stories
            .Select(story => story.ToStoryResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<StoryResponse>>.Ok(responses);
    }

    public async Task<ServiceResult<StoryResponse>> GetStoryByIdAsync(string storyId, CancellationToken ct = default)
    {
        var story = await _storyRepository.GetByIdAsync(storyId, ct);
        if (story is null)
        {
            return ServiceResult<StoryResponse>.Fail(ServiceErrorType.NotFound, "Story not found.");
        }

        return ServiceResult<StoryResponse>.Ok(story.ToStoryResponse());
    }

    public async Task<ServiceResult<IReadOnlyList<StoryResponse>>> GetStoriesForUserAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(userId, ct);
        if (!userExists)
        {
            return ServiceResult<IReadOnlyList<StoryResponse>>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var stories = await _storyRepository.GetActiveByUserIdAsync(userId, pageNumber, pageSize, ct);
        var responses = stories
            .Select(story => story.ToStoryResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<StoryResponse>>.Ok(responses);
    }

    public async Task<ServiceResult<StoryResponse>> CreateStoryAsync(
        string actorUserId,
        StoryCreateRequest request,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(actorUserId, ct);
        if (!userExists)
        {
            return ServiceResult<StoryResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        if (request.ExpiresAt.HasValue && request.ExpiresAt.Value <= DateTime.UtcNow)
        {
            return ServiceResult<StoryResponse>.Fail(
                ServiceErrorType.Validation,
                "Story expiration must be in the future.");
        }

        var story = new Story
        {
            UserId = actorUserId,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = request.ExpiresAt ?? DateTime.UtcNow.AddHours(24)
        };

        await _storyRepository.AddAsync(story, ct);

        return ServiceResult<StoryResponse>.Ok(story.ToStoryResponse());
    }

    public async Task<ServiceResult<string>> DeleteStoryAsync(
        string actorUserId,
        string storyId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<string>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        var story = await _storyRepository.GetByIdAsync(storyId, ct);
        if (story is null)
        {
            return ServiceResult<string>.Fail(ServiceErrorType.NotFound, "Story not found.");
        }

        if (!isAdmin && !string.Equals(story.UserId, actorUserId, StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<string>.Fail(ServiceErrorType.Unauthorized, "You are not allowed to delete this story.");
        }

        var deleted = await _storyRepository.DeleteAsync(storyId, ct);
        if (!deleted)
        {
            return ServiceResult<string>.Fail(ServiceErrorType.NotFound, "Story not found.");
        }

        return ServiceResult<string>.Ok("Story deleted.");
    }
}
