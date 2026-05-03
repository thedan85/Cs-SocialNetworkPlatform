import { Comment, Post } from '../../types';
import { Flag, Heart, MessageCircle, Share2, Send, Trash2, Edit, X, Upload, Image as ImageIcon } from 'lucide-react';
import { ChangeEvent, FormEvent, useEffect, useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { resolveImageUrl } from '../../utils/resolveImageUrl';
import {
  createComment,
  deleteComment,
  deletePost,
  getPostComments,
  getPostById,
  likePost,
  reportPost,
  sharePost,
  unlikePost,
  updatePost
} from '../../services/posts';
import { uploadImage } from '../../services/uploads';

interface PostCardProps {
  post: Post;
  showCommentsByDefault?: boolean;
}

const PostCard: React.FC<PostCardProps> = ({ post, showCommentsByDefault = false }) => {
  const { user } = useAuth();
  const [isLiked, setIsLiked] = useState(Boolean(post.isLiked));
  const [likesCount, setLikesCount] = useState(post.likeCount);
  const [likeSubmitting, setLikeSubmitting] = useState(false);
  const [shareSubmitting, setShareSubmitting] = useState(false);
  const [shareOpen, setShareOpen] = useState(false);
  const [shareDraft, setShareDraft] = useState('');
  const [sharePrivacy, setSharePrivacy] = useState<'Public' | 'Friends' | 'Private'>('Public');
  const [shareError, setShareError] = useState<string | null>(null);
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
  const [editOpen, setEditOpen] = useState(false);
  const [editContent, setEditContent] = useState(post.content);
  const [editImageUrl, setEditImageUrl] = useState(post.imageUrl ?? null);
  const [editImageFile, setEditImageFile] = useState<File | null>(null);
  const [editImagePreview, setEditImagePreview] = useState<string | null>(post.imageUrl ?? null);
  const [editImagePreviewIsLocal, setEditImagePreviewIsLocal] = useState(false);
  const editImageInputRef = useRef<HTMLInputElement | null>(null);
  const [editPrivacy, setEditPrivacy] = useState<'Public' | 'Friends' | 'Private'>(post.privacy as 'Public' | 'Friends' | 'Private' || 'Public');
  const [editSubmitting, setEditSubmitting] = useState(false);
  const [editError, setEditError] = useState<string | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState(false);
  const [deleting, setDeleting] = useState(false);

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
  const canEdit = user?.userId === post.userId;
  const isEdited = new Date(post.updatedAt).getTime() !== new Date(post.createdAt).getTime();
  const hasSharedPost = Boolean(post.sharedPostId);
  const postImageUrl = resolveImageUrl(post.imageUrl);
  const sharedPostImageUrl = resolveImageUrl(post.sharedPost?.imageUrl);

  useEffect(() => {
    setIsLiked(Boolean(post.isLiked));
    setLikesCount(post.likeCount);
    setShareOpen(false);
    setShareDraft('');
    setSharePrivacy('Public');
    setShareError(null);
  }, [post.postId, post.isLiked, post.likeCount]);

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

  const handleShareToggle = () => {
    setShareError(null);
    setShareOpen((current) => !current);
  };

  const handleShareSubmit = async (event: FormEvent) => {
    event.preventDefault();
    if (shareSubmitting) return;

    setShareSubmitting(true);
    setShareError(null);

    try {
      const content = shareDraft.trim();
      await sharePost(post.postId, {
        content: content.length > 0 ? content : null,
        privacy: sharePrivacy
      });
      setShareDraft('');
      setShareOpen(false);
    } catch (err: any) {
      setShareError(err?.message || 'Unable to share this post.');
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

  const handleEditSubmit = async (event: FormEvent) => {
    event.preventDefault();
    if (editSubmitting) return;

    setEditSubmitting(true);
    setEditError(null);
    try {
      let imageUrl = editImageUrl;
      
      if (editImageFile) {
        const uploaded = await uploadImage(editImageFile);
        imageUrl = uploaded.url;
      }

      await updatePost(post.postId, {
        content: editContent.trim(),
        imageUrl: imageUrl,
        privacy: editPrivacy
      });
      setEditOpen(false);
      // Reload post to get updated data
      const updated = await getPostById(post.postId);
      Object.assign(post, updated);
    } catch (err: any) {
      setEditError(err?.message || 'Unable to update post.');
    } finally {
      setEditSubmitting(false);
    }
  };

  const handleEditImageChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) {
      return;
    }

    if (editImagePreviewIsLocal && editImagePreview) {
      URL.revokeObjectURL(editImagePreview);
    }

    setEditImageFile(file);
    setEditImagePreview(URL.createObjectURL(file));
    setEditImagePreviewIsLocal(true);
  };

  const handleRemoveEditImage = () => {
    if (editImagePreviewIsLocal && editImagePreview) {
      URL.revokeObjectURL(editImagePreview);
    }

    setEditImageFile(null);
    setEditImageUrl(null);
    setEditImagePreview(null);
    setEditImagePreviewIsLocal(false);

    if (editImageInputRef.current) {
      editImageInputRef.current.value = '';
    }
  };

  const handleDeletePost = async () => {
    if (deleting) return;
    setDeleting(true);
    try {
      await deletePost(post.postId);
      // Post will be removed by parent component or page refresh
      window.location.reload();
    } catch (err: any) {
      alert(err?.message || 'Unable to delete post.');
    } finally {
      setDeleting(false);
    }
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

  const authorAvatarUrl = post.profilePicture 
    ? resolveImageUrl(post.profilePicture)
    : `https://ui-avatars.com/api/?name=${encodeURIComponent(authorLabel)}`;

  return (
    <div className="bg-white/70 backdrop-blur-xl rounded-2xl shadow-[0_16px_40px_rgba(15,23,42,0.08)] border border-white/60 p-5 transition-all hover:-translate-y-0.5 hover:shadow-[0_20px_50px_rgba(15,23,42,0.12)] dark:bg-slate-900/60 dark:border-slate-800/60">
      <div className="flex items-center gap-3 mb-3">
        <Link to={profilePath} className="flex items-center gap-3 group">
          <img
            src={authorAvatarUrl}
            className="w-10 h-10 rounded-full object-cover ring-2 ring-transparent transition group-hover:ring-cyan-300/60"
            alt={authorLabel}
          />
          <div>
            <p className="font-semibold text-slate-900 transition group-hover:text-cyan-700 dark:text-slate-100 dark:group-hover:text-cyan-200">
              {authorLabel}
            </p>
            <span className="text-xs text-slate-400 dark:text-slate-500">
              {new Date(post.createdAt).toLocaleString()}
              {isEdited && (
                <span> • Edited: {new Date(post.updatedAt).toLocaleString()}</span>
              )}
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

      {postImageUrl && (
        <img
          src={postImageUrl}
          className="w-full rounded-lg mb-3 max-h-96 object-cover"
          alt="Post image"
        />
      )}

      {hasSharedPost && (
        <div className="mb-3 rounded-xl border border-white/70 bg-white/60 p-3 shadow-sm dark:border-slate-800/60 dark:bg-slate-900/50">
          <div className="flex items-center justify-between text-[11px] font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400">
            <span>Shared post</span>
            {post.sharedPost && (
              <span className="font-normal text-slate-400 dark:text-slate-500">
                {new Date(post.sharedPost.createdAt).toLocaleString()}
              </span>
            )}
          </div>

          {post.sharedPost ? (
            <div className="mt-2 rounded-lg border border-white/70 bg-white/80 p-3 dark:border-slate-700/60 dark:bg-slate-900/70">
              <Link
                to={
                  user?.userId === post.sharedPost.userId
                    ? '/profile'
                    : `/users/${post.sharedPost.userId}`
                }
                className="flex items-center gap-3"
              >
                {post.sharedPost.profilePicture ? (
                  <img
                    src={resolveImageUrl(post.sharedPost.profilePicture)}
                    className="h-9 w-9 rounded-full object-cover"
                    alt="Shared author"
                  />
                ) : (
                  <img
                    src={`https://ui-avatars.com/api/?name=${encodeURIComponent(
                      buildDisplayName(
                        post.sharedPost.firstName,
                        post.sharedPost.lastName,
                        post.sharedPost.userName,
                        post.sharedPost.userId
                      )
                    )}`}
                    className="h-9 w-9 rounded-full object-cover"
                    alt="Shared author"
                  />
                )}
                <div>
                  <p className="text-sm font-semibold text-slate-900 dark:text-slate-100">
                    {buildDisplayName(
                      post.sharedPost.firstName,
                      post.sharedPost.lastName,
                      post.sharedPost.userName,
                      post.sharedPost.userId
                    )}
                  </p>
                  <p className="text-xs text-slate-400 dark:text-slate-500">
                    {new Date(post.sharedPost.createdAt).toLocaleString()}
                  </p>
                </div>
              </Link>

              <p className="mt-3 whitespace-pre-wrap text-sm text-slate-700 dark:text-slate-200">
                {post.sharedPost.content}
              </p>

              {sharedPostImageUrl && (
                <img
                  src={sharedPostImageUrl}
                  className="mt-3 w-full rounded-lg max-h-80 object-cover"
                  alt="Shared post image"
                />
              )}
            </div>
          ) : (
            <div className="mt-2 rounded-lg border border-dashed border-slate-200/80 bg-white/80 p-3 text-sm text-slate-500 dark:border-slate-700/60 dark:bg-slate-900/60 dark:text-slate-400">
              Original post is unavailable.
            </div>
          )}
        </div>
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
          onClick={handleShareToggle}
          className="flex items-center gap-1 text-slate-500 hover:text-emerald-600 transition-colors dark:text-slate-400 dark:hover:text-emerald-300"
        >
          <Share2 className="w-4 h-4" />
          Share
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

        {canEdit && (
          <div className="ml-auto flex items-center gap-2">
            <button
              onClick={() => setEditOpen(true)}
              className="flex items-center gap-1 text-xs text-cyan-600 hover:text-cyan-700 dark:text-cyan-400 dark:hover:text-cyan-300"
              type="button"
            >
              <Edit className="w-4 h-4" />
              Edit
            </button>
            <button
              onClick={() => setDeleteConfirm(true)}
              className="flex items-center gap-1 text-xs text-rose-500 hover:text-rose-600 dark:text-rose-400 dark:hover:text-rose-300"
              type="button"
            >
              <Trash2 className="w-4 h-4" />
              Delete
            </button>
          </div>
        )}
      </div>

      {shareOpen && (
        <div className="mt-4 rounded-2xl border border-emerald-200/70 bg-emerald-50/70 p-4 dark:border-emerald-500/30 dark:bg-emerald-500/10">
          <form onSubmit={handleShareSubmit} className="space-y-3">
            <div>
              <label className="text-xs font-semibold uppercase tracking-wide text-emerald-700 dark:text-emerald-300">
                Say something about this post
              </label>
              <textarea
                value={shareDraft}
                onChange={(event) => setShareDraft(event.target.value)}
                placeholder="Add your thoughts..."
                rows={3}
                className="mt-1 w-full rounded-xl border border-emerald-200/70 bg-white/80 px-3 py-2 text-sm text-slate-700 outline-none focus:border-emerald-300 focus:ring-2 focus:ring-emerald-200/70 dark:border-emerald-500/30 dark:bg-slate-900/70 dark:text-slate-100 dark:focus:border-emerald-400/50 dark:focus:ring-emerald-500/20"
              />
            </div>
            <div>
              <label className="text-xs font-semibold uppercase tracking-wide text-emerald-700 dark:text-emerald-300">
                Privacy
              </label>
              <select
                value={sharePrivacy}
                onChange={(event) => setSharePrivacy(event.target.value as 'Public' | 'Friends' | 'Private')}
                className="mt-1 w-full rounded-xl border border-emerald-200/70 bg-white/80 px-3 py-2 text-sm font-medium text-slate-700 focus:outline-none focus:ring-2 focus:ring-emerald-200/70 dark:border-emerald-500/30 dark:bg-slate-900/70 dark:text-slate-100 dark:focus:ring-emerald-500/20"
              >
                <option value="Public">Public</option>
                <option value="Friends">Friends</option>
                <option value="Private">Private</option>
              </select>
            </div>

            {shareError && (
              <div className="rounded-xl border border-rose-200/70 bg-white/80 px-3 py-2 text-xs text-rose-700 dark:border-rose-500/30 dark:bg-slate-900/60 dark:text-rose-200">
                {shareError}
              </div>
            )}

            <div className="flex flex-wrap items-center gap-2">
              <button
                type="submit"
                disabled={shareSubmitting}
                className="rounded-xl bg-emerald-600 px-4 py-2 text-xs font-semibold text-white shadow-sm transition-colors hover:bg-emerald-700 disabled:bg-emerald-300"
              >
                {shareSubmitting ? 'Sharing...' : 'Share post'}
              </button>
              <button
                type="button"
                onClick={() => setShareOpen(false)}
                className="rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

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

      {editOpen && (
        <div className="mt-4 rounded-2xl border border-cyan-200/70 bg-cyan-50/70 p-4 dark:border-cyan-500/30 dark:bg-cyan-500/10">
          <div className="mb-3 flex items-center justify-between">
            <h3 className="text-sm font-semibold text-cyan-700 dark:text-cyan-300">Edit Post</h3>
            <button
              onClick={() => setEditOpen(false)}
              className="text-cyan-600 hover:text-cyan-700 dark:text-cyan-400 dark:hover:text-cyan-300"
              type="button"
            >
              <X className="w-4 h-4" />
            </button>
          </div>
          <form onSubmit={handleEditSubmit} className="space-y-3">
            <div>
              <label className="text-xs font-semibold uppercase tracking-wide text-cyan-700 dark:text-cyan-300">
                Content
              </label>
              <textarea
                value={editContent}
                onChange={(event) => setEditContent(event.target.value)}
                placeholder="What's on your mind?"
                rows={4}
                className="mt-1 w-full rounded-xl border border-cyan-200/70 bg-white/80 px-3 py-2 text-sm text-slate-700 outline-none focus:border-cyan-300 focus:ring-2 focus:ring-cyan-200/70 dark:border-cyan-500/30 dark:bg-slate-900/70 dark:text-slate-100 dark:focus:border-cyan-400/50 dark:focus:ring-cyan-500/20"
              />
            </div>
            <div>
              <label className="text-xs font-semibold uppercase tracking-wide text-cyan-700 dark:text-cyan-300">
                Image
              </label>
              <div className="mt-1 space-y-2">
                <input
                  ref={editImageInputRef}
                  type="file"
                  accept="image/*"
                  onChange={handleEditImageChange}
                  className="hidden"
                />
                <button
                  type="button"
                  onClick={() => editImageInputRef.current?.click()}
                  className="flex w-full items-center justify-center gap-2 rounded-xl border border-cyan-200/70 bg-white/80 px-3 py-2 text-xs font-semibold text-cyan-600 hover:bg-cyan-50/50 dark:border-cyan-500/30 dark:bg-slate-900/70 dark:text-cyan-400 dark:hover:bg-slate-800/70"
                >
                  <Upload className="w-4 h-4" />
                  Choose Image
                </button>
                
                {editImagePreview && (
                  <div className="relative rounded-lg overflow-hidden border border-cyan-200/70 dark:border-cyan-500/30">
                    <img
                      src={editImagePreview}
                      alt="Edit preview"
                      className="w-full max-h-48 object-cover"
                    />
                    <button
                      type="button"
                      onClick={handleRemoveEditImage}
                      className="absolute top-2 right-2 rounded-full bg-rose-600 p-1.5 text-white hover:bg-rose-700 shadow-lg"
                    >
                      <X className="w-3 h-3" />
                    </button>
                  </div>
                )}
              </div>
            </div>
            <div>
              <label className="text-xs font-semibold uppercase tracking-wide text-cyan-700 dark:text-cyan-300">
                Privacy
              </label>
              <select
                value={editPrivacy}
                onChange={(event) => setEditPrivacy(event.target.value as 'Public' | 'Friends' | 'Private')}
                className="mt-1 w-full rounded-xl border border-cyan-200/70 bg-white/80 px-3 py-2 text-sm font-medium text-slate-700 focus:outline-none focus:ring-2 focus:ring-cyan-200/70 dark:border-cyan-500/30 dark:bg-slate-900/70 dark:text-slate-100 dark:focus:ring-cyan-500/20"
              >
                <option value="Public">Public</option>
                <option value="Friends">Friends</option>
                <option value="Private">Private</option>
              </select>
            </div>

            {editError && (
              <div className="rounded-xl border border-rose-200/70 bg-white/80 px-3 py-2 text-xs text-rose-700 dark:border-rose-500/30 dark:bg-slate-900/60 dark:text-rose-200">
                {editError}
              </div>
            )}

            <div className="flex flex-wrap items-center gap-2">
              <button
                type="submit"
                disabled={editSubmitting}
                className="rounded-xl bg-cyan-600 px-4 py-2 text-xs font-semibold text-white shadow-sm transition-colors hover:bg-cyan-700 disabled:bg-cyan-300"
              >
                {editSubmitting ? 'Updating...' : 'Update post'}
              </button>
              <button
                type="button"
                onClick={() => setEditOpen(false)}
                className="rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {deleteConfirm && (
        <div className="mt-4 rounded-2xl border border-rose-200/70 bg-rose-50/70 p-4 dark:border-rose-500/30 dark:bg-rose-500/10">
          <div className="mb-3 flex items-center justify-between">
            <h3 className="text-sm font-semibold text-rose-700 dark:text-rose-300">Delete Post</h3>
            <button
              onClick={() => setDeleteConfirm(false)}
              className="text-rose-600 hover:text-rose-700 dark:text-rose-400 dark:hover:text-rose-300"
              type="button"
            >
              <X className="w-4 h-4" />
            </button>
          </div>
          <p className="mb-4 text-sm text-rose-700 dark:text-rose-200">
            Are you sure you want to delete this post? This action cannot be undone.
          </p>
          <div className="flex gap-2">
            <button
              onClick={handleDeletePost}
              disabled={deleting}
              className="rounded-xl bg-rose-600 px-4 py-2 text-xs font-semibold text-white shadow-sm transition-colors hover:bg-rose-700 disabled:bg-rose-300"
            >
              {deleting ? 'Deleting...' : 'Delete permanently'}
            </button>
            <button
              onClick={() => setDeleteConfirm(false)}
              className="rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
            >
              Cancel
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default PostCard;
