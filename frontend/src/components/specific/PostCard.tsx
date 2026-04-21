import { Comment, Post } from '../../types';
import { Flag, Heart, MessageCircle, Share2, Send, Trash2 } from 'lucide-react';
import { FormEvent, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import {
  createComment,
  deleteComment,
  getPostComments,
  likePost,
  reportPost,
  sharePost,
  unlikePost,
  unsharePost
} from '../../services/posts';

interface PostCardProps {
  post: Post;
  showCommentsByDefault?: boolean;
}

const PostCard: React.FC<PostCardProps> = ({ post, showCommentsByDefault = false }) => {
  const { user } = useAuth();
  const [isLiked, setIsLiked] = useState(Boolean(post.isLiked));
  const [likesCount, setLikesCount] = useState(post.likeCount);
  const [likeSubmitting, setLikeSubmitting] = useState(false);
  const [isShared, setIsShared] = useState(Boolean(post.isShared));
  const [shareCount, setShareCount] = useState(post.shareCount ?? 0);
  const [shareSubmitting, setShareSubmitting] = useState(false);
  const [commentsOpen, setCommentsOpen] = useState(showCommentsByDefault);
  const [comments, setComments] = useState<Comment[]>([]);
  const [commentsLoading, setCommentsLoading] = useState(false);
  const [commentsLoaded, setCommentsLoaded] = useState(false);
  const [commentsError, setCommentsError] = useState<string | null>(null);
  const [commentDraft, setCommentDraft] = useState('');
  const [commentSubmitting, setCommentSubmitting] = useState(false);
  const [reportOpen, setReportOpen] = useState(false);
  const [reportReason, setReportReason] = useState('');
  const [reportDescription, setReportDescription] = useState('');
  const [reportSubmitting, setReportSubmitting] = useState(false);
  const [reportError, setReportError] = useState<string | null>(null);
  const [reportSuccess, setReportSuccess] = useState<string | null>(null);

  const buildDisplayName = (
    firstName?: string | null,
    lastName?: string | null,
    userName?: string | null,
    fallbackId?: string
  ) => {
    const fullName = [firstName, lastName].filter(Boolean).join(' ').trim();
    if (fullName) return fullName;
    if (userName) return userName;
    if (fallbackId) return `User ${fallbackId.slice(0, 8)}`;
    return 'Unknown user';
  };

  const authorLabel = buildDisplayName(post.firstName, post.lastName, post.userName, post.userId);
  const profilePath = user?.userId === post.userId ? '/profile' : `/users/${post.userId}`;
  const canReport = user?.userId !== post.userId;

  useEffect(() => {
    setIsLiked(Boolean(post.isLiked));
    setLikesCount(post.likeCount);
    setIsShared(Boolean(post.isShared));
    setShareCount(post.shareCount ?? 0);
  }, [post.postId, post.isLiked, post.likeCount, post.isShared, post.shareCount]);

  const handleLike = async () => {
    if (likeSubmitting) return;
    setLikeSubmitting(true);
    try {
      if (isLiked) {
        await unlikePost(post.postId);
        setIsLiked(false);
        setLikesCount((current) => Math.max(0, current - 1));
      } else {
        await likePost(post.postId);
        setIsLiked(true);
        setLikesCount((current) => current + 1);
      }
    } catch (err) {
      alert(isLiked ? 'Unable to unlike this post.' : 'Unable to like this post.');
    } finally {
      setLikeSubmitting(false);
    }
  };

  const handleShare = async () => {
    if (shareSubmitting) return;
    setShareSubmitting(true);
    try {
      if (isShared) {
        await unsharePost(post.postId);
        setIsShared(false);
        setShareCount((current) => Math.max(0, current - 1));
      } else {
        await sharePost(post.postId);
        setIsShared(true);
        setShareCount((current) => current + 1);
      }
    } catch (err) {
      alert(isShared ? 'Unable to unshare this post.' : 'Unable to share this post.');
    } finally {
      setShareSubmitting(false);
    }
  };

  const toggleReport = () => {
    setReportError(null);
    setReportSuccess(null);
    setReportOpen((current) => !current);
  };

  const handleReportSubmit = async (event: FormEvent) => {
    event.preventDefault();
    if (reportSubmitting) return;

    setReportSubmitting(true);
    setReportError(null);
    setReportSuccess(null);
    try {
      await reportPost(post.postId, {
        reason: reportReason.trim() || null,
        description: reportDescription.trim() || null
      });
      setReportReason('');
      setReportDescription('');
      setReportSuccess('Report submitted. Thank you for letting us know.');
    } catch (err: any) {
      setReportError(err?.message || 'Unable to report this post.');
    } finally {
      setReportSubmitting(false);
    }
  };

  const loadComments = async () => {
    setCommentsLoading(true);
    setCommentsError(null);
    try {
      const data = await getPostComments(post.postId);
      setComments(data);
    } catch (err: any) {
      setCommentsError(err?.message || 'Unable to load comments.');
    } finally {
      setCommentsLoading(false);
      setCommentsLoaded(true);
    }
  };

  const toggleComments = () => {
    setCommentsOpen((current) => !current);
  };

  useEffect(() => {
    if (commentsOpen && !commentsLoaded && !commentsLoading) {
      loadComments();
    }
  }, [commentsOpen, commentsLoaded, commentsLoading]);

  const handleAddComment = async (event: FormEvent) => {
    event.preventDefault();
    const trimmed = commentDraft.trim();
    if (!trimmed) return;
    setCommentSubmitting(true);
    try {
      const created = await createComment(post.postId, { content: trimmed });
      setComments((current) => [created, ...current]);
      setCommentDraft('');
    } catch (err: any) {
      setCommentsError(err?.message || 'Unable to add comment.');
    } finally {
      setCommentSubmitting(false);
    }
  };

  const handleDeleteComment = async (commentId: string) => {
    try {
      await deleteComment(post.postId, commentId);
      setComments((current) => current.filter((item) => item.commentId !== commentId));
    } catch (err: any) {
      setCommentsError(err?.message || 'Unable to delete comment.');
    }
  };

  // 3. Chuẩn hóa dữ liệu từ props post sang định dạng mà PostDetailModal yêu cầu
  // (Do interface trong postdetail.tsx hơi khác với interface Post của bạn)
  const formattedPostForModal = {
    id: post.id || "",
    user: {
      name: post.author.username,
      avatar:
        post.author.avatarUrl ||
        `https://ui-avatars.com/api/?name=${post.author.username}`,
    },
    image: post.imageUrl || "",
    caption: post.content,
    likes: likesCount,
    timestamp: new Date(post.createdAt).toLocaleString(),
    comments: [], // Bạn có thể truyền post.comments nếu interface Post có chứa
  };

  return (
    <div className="bg-white/70 backdrop-blur-xl rounded-2xl shadow-[0_16px_40px_rgba(15,23,42,0.08)] border border-white/60 p-5 transition-all hover:-translate-y-0.5 hover:shadow-[0_20px_50px_rgba(15,23,42,0.12)] dark:bg-slate-900/60 dark:border-slate-800/60">
      <div className="flex items-center gap-3 mb-3">
        <Link to={profilePath} className="flex items-center gap-3 group">
          <img
            src={`https://ui-avatars.com/api/?name=${encodeURIComponent(authorLabel)}`}
            className="w-10 h-10 rounded-full object-cover ring-2 ring-transparent transition group-hover:ring-cyan-300/60"
            alt={authorLabel}
          />
          <div>
            <p className="font-semibold text-slate-900 transition group-hover:text-cyan-700 dark:text-slate-100 dark:group-hover:text-cyan-200">
              {authorLabel}
            </p>
            <span className="text-xs text-slate-400 dark:text-slate-500">
              {new Date(post.createdAt).toLocaleString()}
            </span>
          </div>
        </Link>
        {post.privacy && (
          <span className="ml-auto rounded-full border border-white/70 bg-white/80 px-3 py-1 text-xs font-semibold text-slate-600 dark:border-slate-700/60 dark:bg-slate-900/70 dark:text-slate-300">
            {post.privacy}
          </span>
        )}
      </div>

      <p className="whitespace-pre-wrap text-slate-700 mb-3 dark:text-slate-200">{post.content}</p>

      {post.imageUrl && (
        <img
          src={post.imageUrl}
          className="w-full rounded-lg mb-3 max-h-96 object-cover"
          alt="Post image"
        />
      )}

      <div className="flex items-center gap-6 text-slate-500 text-sm border-t border-white/60 pt-3 dark:text-slate-400 dark:border-slate-800/60">
        <button
          onClick={handleLike}
          disabled={likeSubmitting}
          aria-pressed={isLiked}
          className={`flex items-center gap-1 transition-colors disabled:opacity-60 ${
            isLiked ? 'text-rose-500 font-semibold' : 'hover:text-rose-500'
          }`}
        >
          <Heart className={`w-4 h-4 ${isLiked ? "fill-current" : ""}`} />
          {likesCount}
        </button>

        <button
          onClick={toggleComments}
          className="flex items-center gap-1 hover:text-cyan-600 transition-colors dark:hover:text-cyan-300"
        >
          <MessageCircle className="w-4 h-4" />
          {comments.length > 0 ? `Comments (${comments.length})` : 'Comment'}
        </button>

        <button
          onClick={handleShare}
          disabled={shareSubmitting}
          aria-pressed={isShared}
          className={`flex items-center gap-1 transition-colors disabled:opacity-60 ${
            isShared ? 'text-emerald-600 font-semibold' : 'hover:text-emerald-600'
          }`}
        >
          <Share2 className={`w-4 h-4 ${isShared ? 'fill-current' : ''}`} />
          {shareCount}
        </button>

        {canReport && (
          <button
            onClick={toggleReport}
            className="ml-auto flex items-center gap-1 text-xs text-rose-500 hover:text-rose-600 dark:text-rose-400 dark:hover:text-rose-300"
            type="button"
          >
            <Flag className="w-4 h-4" />
            Report
          </button>
        )}
      </div>

      {reportOpen && canReport && (
        <div className="mt-4 rounded-2xl border border-rose-200/70 bg-rose-50/70 p-4 dark:border-rose-500/30 dark:bg-rose-500/10">
          <form onSubmit={handleReportSubmit} className="space-y-3">
            <div>
              <label className="text-xs font-semibold uppercase tracking-wide text-rose-600 dark:text-rose-300">
                Reason
              </label>
              <input
                value={reportReason}
                onChange={(event) => setReportReason(event.target.value)}
                placeholder="Spam, harassment, misinformation..."
                className="mt-1 w-full rounded-xl border border-rose-200/70 bg-white/80 px-3 py-2 text-sm text-slate-700 outline-none focus:border-rose-300 focus:ring-2 focus:ring-rose-200/70 dark:border-rose-500/30 dark:bg-slate-900/70 dark:text-slate-100 dark:focus:border-rose-400/50 dark:focus:ring-rose-500/20"
              />
            </div>
            <div>
              <label className="text-xs font-semibold uppercase tracking-wide text-rose-600 dark:text-rose-300">
                Description
              </label>
              <textarea
                value={reportDescription}
                onChange={(event) => setReportDescription(event.target.value)}
                placeholder="Add any details to help us review."
                rows={3}
                className="mt-1 w-full rounded-xl border border-rose-200/70 bg-white/80 px-3 py-2 text-sm text-slate-700 outline-none focus:border-rose-300 focus:ring-2 focus:ring-rose-200/70 dark:border-rose-500/30 dark:bg-slate-900/70 dark:text-slate-100 dark:focus:border-rose-400/50 dark:focus:ring-rose-500/20"
              />
            </div>

            {reportError && (
              <div className="rounded-xl border border-rose-200/70 bg-white/80 px-3 py-2 text-xs text-rose-700 dark:border-rose-500/30 dark:bg-slate-900/60 dark:text-rose-200">
                {reportError}
              </div>
            )}
            {reportSuccess && (
              <div className="rounded-xl border border-emerald-200/70 bg-white/80 px-3 py-2 text-xs text-emerald-700 dark:border-emerald-500/30 dark:bg-slate-900/60 dark:text-emerald-200">
                {reportSuccess}
              </div>
            )}

            <div className="flex flex-wrap items-center gap-2">
              <button
                type="submit"
                disabled={reportSubmitting}
                className="rounded-xl bg-rose-500 px-4 py-2 text-xs font-semibold text-white shadow-sm transition-colors hover:bg-rose-600 disabled:bg-rose-300"
              >
                {reportSubmitting ? 'Submitting...' : 'Submit report'}
              </button>
              <button
                type="button"
                onClick={() => setReportOpen(false)}
                className="rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {commentsOpen && (
        <div className="mt-4 border-t border-white/60 pt-4 dark:border-slate-800/60">
          <form onSubmit={handleAddComment} className="flex flex-col gap-3 sm:flex-row">
            <input
              value={commentDraft}
              onChange={(event) => setCommentDraft(event.target.value)}
              placeholder="Write a comment..."
              className="flex-1 rounded-xl border border-white/70 bg-white/80 px-3 py-2 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-300/70 dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-100 dark:placeholder:text-slate-500 dark:focus:ring-cyan-500/40"
            />
            <button
              type="submit"
              disabled={!commentDraft.trim() || commentSubmitting}
              className="flex items-center justify-center gap-2 rounded-xl bg-gradient-to-r from-teal-500 via-cyan-500 to-amber-400 px-4 py-2 text-sm font-semibold text-white shadow-lg shadow-cyan-500/25 hover:from-teal-600 hover:via-cyan-600 hover:to-amber-500 disabled:from-slate-300 disabled:via-slate-300 disabled:to-slate-300 disabled:text-slate-500 disabled:shadow-none"
            >
              <Send className="w-4 h-4" />
              {commentSubmitting ? 'Posting...' : 'Post'}
            </button>
          </form>

          {commentsLoading && (
            <div className="mt-3 text-sm text-slate-500 dark:text-slate-400">Loading comments...</div>
          )}
          {commentsError && (
            <div className="mt-3 rounded-xl border border-rose-200/70 bg-rose-50/70 px-3 py-2 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
              {commentsError}
            </div>
          )}

          {!commentsLoading && comments.length === 0 && !commentsError && (
            <div className="mt-3 text-sm text-slate-500 dark:text-slate-400">No comments yet.</div>
          )}

          <div className="mt-4 space-y-3">
            {comments.map((comment) => {
              const commentAuthor = buildDisplayName(
                comment.firstName,
                comment.lastName,
                comment.userName,
                comment.userId
              );
              const canDelete = user?.userId === comment.userId;

              return (
                <div key={comment.commentId} className="rounded-xl border border-white/60 bg-white/70 px-3 py-3 shadow-sm dark:border-slate-800/60 dark:bg-slate-900/60">
                  <div className="flex items-start justify-between gap-4">
                    <div>
                      <Link
                        to={`/users/${comment.userId}`}
                        className="text-sm font-semibold text-slate-900 hover:text-cyan-700 dark:text-slate-100 dark:hover:text-cyan-200"
                      >
                        {commentAuthor}
                      </Link>
                      <p className="text-xs text-slate-400 dark:text-slate-500">
                        {new Date(comment.createdAt).toLocaleString()}
                      </p>
                    </div>
                    {canDelete && (
                      <button
                        onClick={() => handleDeleteComment(comment.commentId)}
                        className="flex items-center gap-1 text-xs text-rose-500 hover:text-rose-600 dark:text-rose-400 dark:hover:text-rose-300"
                        type="button"
                      >
                        <Trash2 className="w-3.5 h-3.5" />
                        Delete
                      </button>
                    )}
                  </div>
                  <p className="mt-2 text-sm text-slate-700 dark:text-slate-200">{comment.content}</p>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
};

export default PostCard;
