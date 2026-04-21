import { useEffect, useState } from 'react';
import { Flag, Users, UserCheck, UserX, RefreshCw, Search, Filter, FileText, Trash2 } from 'lucide-react';
import { getUsers, searchUsers, updateUser } from '../services/users';
import { deletePost, getPosts } from '../services/posts';
import { getPendingPostReports, reviewPostReport } from '../services/postReports';
import { useAuth } from '../contexts/AuthContext';
import type { Post, PostReportDetail, User } from '../types';

const AdminDashboard = () => {
  const { user } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [userLoading, setUserLoading] = useState(true);
  const [userError, setUserError] = useState<string | null>(null);
  const [userActionError, setUserActionError] = useState<string | null>(null);
  const [updatingUserId, setUpdatingUserId] = useState<string | null>(null);
  const [userQuery, setUserQuery] = useState('');
  const [statusFilter, setStatusFilter] = useState<'all' | 'active' | 'inactive'>('all');

  const [posts, setPosts] = useState<Post[]>([]);
  const [postsLoading, setPostsLoading] = useState(true);
  const [postsError, setPostsError] = useState<string | null>(null);
  const [postActionError, setPostActionError] = useState<string | null>(null);
  const [postsPage, setPostsPage] = useState(1);
  const [postsHasMore, setPostsHasMore] = useState(true);
  const [postQuery, setPostQuery] = useState('');
  const [deletingPostId, setDeletingPostId] = useState<string | null>(null);

  const [reports, setReports] = useState<PostReportDetail[]>([]);
  const [reportsLoading, setReportsLoading] = useState(true);
  const [reportsError, setReportsError] = useState<string | null>(null);
  const [reportsActionError, setReportsActionError] = useState<string | null>(null);
  const [reviewingReportId, setReviewingReportId] = useState<string | null>(null);
  const [reportsPage, setReportsPage] = useState(1);
  const [reportsHasMore, setReportsHasMore] = useState(true);

  const postsPageSize = 8;
  const reportsPageSize = 8;

  const loadUsers = async (query?: string) => {
    setUserLoading(true);
    setUserError(null);
    try {
      const trimmedQuery = query?.trim() ?? '';
      const result = trimmedQuery
        ? await searchUsers(trimmedQuery, 1, 50)
        : await getUsers();
      setUsers(result);
    } catch (err) {
      setUserError('Unable to load users right now.');
    } finally {
      setUserLoading(false);
    }
  };

  const loadPosts = async (pageNumber = 1, append = false) => {
    setPostsLoading(true);
    setPostsError(null);
    try {
      const result = await getPosts(pageNumber, postsPageSize);
      setPosts((current) => (append ? [...current, ...result] : result));
      setPostsHasMore(result.length === postsPageSize);
      setPostsPage(pageNumber);
    } catch (err) {
      setPostsError('Unable to load posts right now.');
    } finally {
      setPostsLoading(false);
    }
  };

  const loadReports = async (pageNumber = 1, append = false) => {
    setReportsLoading(true);
    setReportsError(null);
    try {
      const result = await getPendingPostReports(pageNumber, reportsPageSize);
      setReports((current) => (append ? [...current, ...result] : result));
      setReportsHasMore(result.length === reportsPageSize);
      setReportsPage(pageNumber);
    } catch (err) {
      setReportsError('Unable to load reports right now.');
    } finally {
      setReportsLoading(false);
    }
  };

  useEffect(() => {
    loadUsers().catch(() => undefined);
    loadPosts(1, false).catch(() => undefined);
    loadReports(1, false).catch(() => undefined);
  }, []);

  const handleUserSearchSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    await loadUsers(userQuery);
  };

  const handleUserSearchClear = async () => {
    setUserQuery('');
    await loadUsers('');
  };

  const handleLoadMorePosts = async () => {
    if (postsLoading || !postsHasMore) {
      return;
    }

    await loadPosts(postsPage + 1, true);
  };

  const handleLoadMoreReports = async () => {
    if (reportsLoading || !reportsHasMore) {
      return;
    }

    await loadReports(reportsPage + 1, true);
  };

  const handleRefreshAll = async () => {
    await Promise.all([loadUsers(userQuery), loadPosts(1, false), loadReports(1, false)]);
  };

  const handleToggleActive = async (target: User) => {
    if (updatingUserId) {
      return;
    }

    setUserActionError(null);
    setUpdatingUserId(target.userId);
    try {
      const updated = await updateUser(target.userId, {
        isActive: !(target.isActive ?? true)
      });
      setUsers((current) =>
        current.map((item) => (item.userId === updated.userId ? updated : item))
      );
    } catch (err) {
      setUserActionError('Unable to update user status.');
    } finally {
      setUpdatingUserId(null);
    }
  };

  const handleDeletePost = async (target: Post) => {
    if (deletingPostId) {
      return;
    }

    const confirmed = window.confirm('Delete this post? This cannot be undone.');
    if (!confirmed) {
      return;
    }

    setPostActionError(null);
    setDeletingPostId(target.postId);
    try {
      await deletePost(target.postId);
      setPosts((current) => current.filter((item) => item.postId !== target.postId));
    } catch (err) {
      setPostActionError('Unable to delete post.');
    } finally {
      setDeletingPostId(null);
    }
  };

  const handleReviewReport = async (reportId: string) => {
    if (reviewingReportId) {
      return;
    }

    setReportsActionError(null);
    setReviewingReportId(reportId);
    try {
      await reviewPostReport(reportId, true);
      setReports((current) => current.filter((item) => item.postReportId !== reportId));
    } catch (err) {
      setReportsActionError('Unable to update the report status.');
    } finally {
      setReviewingReportId(null);
    }
  };

  const handleDeleteReportedPost = async (target: PostReportDetail) => {
    if (deletingPostId) {
      return;
    }

    const confirmed = window.confirm('Delete this post? This cannot be undone.');
    if (!confirmed) {
      return;
    }

    setReportsActionError(null);
    setDeletingPostId(target.postId);
    try {
      await deletePost(target.postId);
      await reviewPostReport(target.postReportId, true);
      setReports((current) => current.filter((item) => item.postId !== target.postId));
      setPosts((current) => current.filter((item) => item.postId !== target.postId));
    } catch (err) {
      setReportsActionError('Unable to delete the reported post.');
    } finally {
      setDeletingPostId(null);
    }
  };

  const formatTimestamp = (value: string) => {
    const parsed = new Date(value);
    if (Number.isNaN(parsed.getTime())) {
      return value;
    }
    return parsed.toLocaleString();
  };

  const totalUsers = users.length;
  const activeUsers = users.filter((item) => item.isActive ?? true).length;
  const inactiveUsers = totalUsers - activeUsers;

  const filteredUsers = users.filter((entry) => {
    const isActive = entry.isActive ?? true;
    if (statusFilter === 'active' && !isActive) {
      return false;
    }
    if (statusFilter === 'inactive' && isActive) {
      return false;
    }
    return true;
  });

  const normalizedPostQuery = postQuery.trim().toLowerCase();
  const filteredPosts = posts.filter((entry) => {
    if (!normalizedPostQuery) {
      return true;
    }

    const haystack = [
      entry.content,
      entry.userName,
      entry.firstName,
      entry.lastName
    ]
      .filter(Boolean)
      .join(' ')
      .toLowerCase();

    return haystack.includes(normalizedPostQuery);
  });

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold text-slate-900 tracking-tight dark:text-slate-100">Admin Dashboard</h1>
          <p className="text-slate-600 dark:text-slate-400">Manage user accounts and content moderation.</p>
        </div>
        <button
          onClick={() => handleRefreshAll()}
          className="inline-flex items-center gap-2 rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-sm font-medium text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
          type="button"
        >
          <RefreshCw className="h-4 w-4" />
          Refresh all
        </button>
      </div>

      <div className="grid gap-4 sm:grid-cols-3">
        <div className="rounded-2xl border border-white/60 bg-white/80 p-4 shadow-[0_12px_30px_rgba(15,23,42,0.08)] dark:border-slate-800/70 dark:bg-slate-900/70">
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-cyan-500/10 text-cyan-600 dark:text-cyan-400">
              <Users className="h-5 w-5" />
            </div>
            <div>
              <p className="text-sm text-slate-500 dark:text-slate-400">Total users</p>
              <p className="text-2xl font-semibold text-slate-900 dark:text-slate-100">{totalUsers}</p>
            </div>
          </div>
        </div>
        <div className="rounded-2xl border border-white/60 bg-white/80 p-4 shadow-[0_12px_30px_rgba(15,23,42,0.08)] dark:border-slate-800/70 dark:bg-slate-900/70">
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-emerald-500/10 text-emerald-600 dark:text-emerald-400">
              <UserCheck className="h-5 w-5" />
            </div>
            <div>
              <p className="text-sm text-slate-500 dark:text-slate-400">Active users</p>
              <p className="text-2xl font-semibold text-slate-900 dark:text-slate-100">{activeUsers}</p>
            </div>
          </div>
        </div>
        <div className="rounded-2xl border border-white/60 bg-white/80 p-4 shadow-[0_12px_30px_rgba(15,23,42,0.08)] dark:border-slate-800/70 dark:bg-slate-900/70">
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-rose-500/10 text-rose-600 dark:text-rose-400">
              <UserX className="h-5 w-5" />
            </div>
            <div>
              <p className="text-sm text-slate-500 dark:text-slate-400">Inactive users</p>
              <p className="text-2xl font-semibold text-slate-900 dark:text-slate-100">{inactiveUsers}</p>
            </div>
          </div>
        </div>
      </div>

      {userError && (
        <div className="rounded-2xl border border-rose-200/70 bg-rose-50/70 px-4 py-3 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
          {userError}
        </div>
      )}

      {userActionError && (
        <div className="rounded-2xl border border-amber-200/70 bg-amber-50/70 px-4 py-3 text-sm text-amber-700 dark:border-amber-500/30 dark:bg-amber-500/10 dark:text-amber-200">
          {userActionError}
        </div>
      )}

      <div className="rounded-2xl border border-white/60 bg-white/80 shadow-[0_16px_40px_rgba(15,23,42,0.12)] dark:border-slate-800/70 dark:bg-slate-900/70">
        <div className="flex flex-col gap-4 border-b border-white/60 px-6 py-4 dark:border-slate-800/70 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-100">User management</h2>
            <p className="text-sm text-slate-500 dark:text-slate-400">Toggle account access for each user.</p>
          </div>
          <button
            onClick={() => loadUsers(userQuery)}
            className="inline-flex items-center gap-2 rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
            type="button"
          >
            <RefreshCw className="h-4 w-4" />
            Refresh users
          </button>
        </div>
        <div className="border-b border-white/60 px-6 py-4 dark:border-slate-800/70">
          <form onSubmit={handleUserSearchSubmit} className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
            <div className="relative w-full lg:max-w-md">
              <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
              <input
                value={userQuery}
                onChange={(event) => setUserQuery(event.target.value)}
                placeholder="Search by name or username"
                className="w-full rounded-xl border border-white/70 bg-white/80 py-2 pl-9 pr-3 text-sm text-slate-700 outline-none transition-all focus:border-cyan-200 focus:ring-2 focus:ring-cyan-300/70 dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:focus:border-cyan-500/40 dark:focus:ring-cyan-500/40"
              />
            </div>
            <div className="flex flex-wrap items-center gap-3">
              <div className="flex items-center gap-2 text-sm text-slate-500 dark:text-slate-400">
                <Filter className="h-4 w-4" />
                <select
                  value={statusFilter}
                  onChange={(event) => setStatusFilter(event.target.value as 'all' | 'active' | 'inactive')}
                  className="rounded-xl border border-white/70 bg-white/80 px-3 py-2 text-sm text-slate-700 outline-none transition-all focus:border-cyan-200 focus:ring-2 focus:ring-cyan-300/70 dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:focus:border-cyan-500/40 dark:focus:ring-cyan-500/40"
                >
                  <option value="all">All statuses</option>
                  <option value="active">Active only</option>
                  <option value="inactive">Inactive only</option>
                </select>
              </div>
              <button
                type="submit"
                className="rounded-xl bg-gradient-to-r from-teal-500 via-cyan-500 to-amber-400 px-4 py-2 text-xs font-semibold text-white shadow-lg shadow-cyan-500/20 transition-all hover:from-teal-600 hover:via-cyan-600 hover:to-amber-500"
              >
                Search
              </button>
              <button
                type="button"
                onClick={() => handleUserSearchClear()}
                className="rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
              >
                Clear
              </button>
            </div>
          </form>
        </div>
        <div className="divide-y divide-white/70 dark:divide-slate-800/70">
          {userLoading ? (
            <div className="px-6 py-8 text-sm text-slate-500 dark:text-slate-400">Loading users...</div>
          ) : filteredUsers.length === 0 ? (
            <div className="px-6 py-8 text-sm text-slate-500 dark:text-slate-400">No users found.</div>
          ) : (
            filteredUsers.map((entry) => {
              const isActive = entry.isActive ?? true;
              const isSelf = user?.userId === entry.userId;

              return (
                <div
                  key={entry.userId}
                  className="flex flex-col gap-3 px-6 py-4 sm:flex-row sm:items-center sm:justify-between"
                >
                  <div className="flex items-center gap-4">
                    <div className="flex h-11 w-11 items-center justify-center rounded-full bg-gradient-to-br from-teal-500 via-cyan-500 to-amber-400 text-sm font-semibold text-white">
                      {(entry.firstName?.[0] || entry.userName?.[0] || 'U').toUpperCase()}
                    </div>
                    <div>
                      <p className="text-sm font-semibold text-slate-900 dark:text-slate-100">
                        {[entry.firstName, entry.lastName].filter(Boolean).join(' ') || entry.userName}
                      </p>
                      <p className="text-xs text-slate-500 dark:text-slate-400">{entry.email}</p>
                    </div>
                  </div>

                  <div className="flex flex-wrap items-center gap-3">
                    <span
                      className={`rounded-full px-3 py-1 text-xs font-semibold ${
                        isActive
                          ? 'bg-emerald-100/70 text-emerald-700 dark:bg-emerald-500/10 dark:text-emerald-300'
                          : 'bg-rose-100/70 text-rose-700 dark:bg-rose-500/10 dark:text-rose-300'
                      }`}
                    >
                      {isActive ? 'Active' : 'Inactive'}
                    </span>

                    {isSelf ? (
                      <span className="text-xs font-semibold text-slate-500 dark:text-slate-400">Your account</span>
                    ) : (
                      <button
                        onClick={() => handleToggleActive(entry)}
                        disabled={updatingUserId === entry.userId}
                        className={`rounded-xl px-4 py-2 text-xs font-semibold transition-colors ${
                          isActive
                            ? 'bg-rose-500 text-white hover:bg-rose-600'
                            : 'bg-emerald-500 text-white hover:bg-emerald-600'
                        } disabled:cursor-not-allowed disabled:bg-slate-300 disabled:text-slate-600`}
                        type="button"
                      >
                        {updatingUserId === entry.userId ? 'Updating...' : isActive ? 'Deactivate' : 'Activate'}
                      </button>
                    )}
                  </div>
                </div>
              );
            })
          )}
        </div>
      </div>

      {postsError && (
        <div className="rounded-2xl border border-rose-200/70 bg-rose-50/70 px-4 py-3 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
          {postsError}
        </div>
      )}

      {postActionError && (
        <div className="rounded-2xl border border-amber-200/70 bg-amber-50/70 px-4 py-3 text-sm text-amber-700 dark:border-amber-500/30 dark:bg-amber-500/10 dark:text-amber-200">
          {postActionError}
        </div>
      )}

      {reportsError && (
        <div className="rounded-2xl border border-rose-200/70 bg-rose-50/70 px-4 py-3 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
          {reportsError}
        </div>
      )}

      {reportsActionError && (
        <div className="rounded-2xl border border-amber-200/70 bg-amber-50/70 px-4 py-3 text-sm text-amber-700 dark:border-amber-500/30 dark:bg-amber-500/10 dark:text-amber-200">
          {reportsActionError}
        </div>
      )}

      <div className="rounded-2xl border border-white/60 bg-white/80 shadow-[0_16px_40px_rgba(15,23,42,0.12)] dark:border-slate-800/70 dark:bg-slate-900/70">
        <div className="flex flex-col gap-4 border-b border-white/60 px-6 py-4 dark:border-slate-800/70 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-100">Post moderation</h2>
            <p className="text-sm text-slate-500 dark:text-slate-400">Review and remove posts that violate policy.</p>
          </div>
          <button
            onClick={() => loadPosts(1, false)}
            className="inline-flex items-center gap-2 rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
            type="button"
          >
            <RefreshCw className="h-4 w-4" />
            Refresh posts
          </button>
        </div>
        <div className="border-b border-white/60 px-6 py-4 dark:border-slate-800/70">
          <div className="relative w-full lg:max-w-md">
            <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
            <input
              value={postQuery}
              onChange={(event) => setPostQuery(event.target.value)}
              placeholder="Search posts by author or content"
              className="w-full rounded-xl border border-white/70 bg-white/80 py-2 pl-9 pr-3 text-sm text-slate-700 outline-none transition-all focus:border-cyan-200 focus:ring-2 focus:ring-cyan-300/70 dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:focus:border-cyan-500/40 dark:focus:ring-cyan-500/40"
            />
          </div>
        </div>
        <div className="divide-y divide-white/70 dark:divide-slate-800/70">
          {postsLoading ? (
            <div className="px-6 py-8 text-sm text-slate-500 dark:text-slate-400">Loading posts...</div>
          ) : filteredPosts.length === 0 ? (
            <div className="px-6 py-8 text-sm text-slate-500 dark:text-slate-400">No posts found.</div>
          ) : (
            filteredPosts.map((entry) => {
              const authorName = [entry.firstName, entry.lastName].filter(Boolean).join(' ') || entry.userName || 'Unknown';

              return (
                <div key={entry.postId} className="flex flex-col gap-4 px-6 py-5">
                  <div className="flex flex-wrap items-center justify-between gap-3">
                    <div className="flex items-center gap-3">
                      <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-cyan-500/10 text-cyan-600 dark:text-cyan-400">
                        <FileText className="h-5 w-5" />
                      </div>
                      <div>
                        <p className="text-sm font-semibold text-slate-900 dark:text-slate-100">{authorName}</p>
                        <p className="text-xs text-slate-500 dark:text-slate-400">{entry.userName}</p>
                      </div>
                    </div>
                    <button
                      onClick={() => handleDeletePost(entry)}
                      disabled={deletingPostId === entry.postId}
                      className="inline-flex items-center gap-2 rounded-xl bg-rose-500 px-3 py-2 text-xs font-semibold text-white transition-colors hover:bg-rose-600 disabled:cursor-not-allowed disabled:bg-slate-300 disabled:text-slate-600"
                      type="button"
                    >
                      <Trash2 className="h-4 w-4" />
                      {deletingPostId === entry.postId ? 'Deleting...' : 'Delete'}
                    </button>
                  </div>
                  <p className="text-sm text-slate-600 dark:text-slate-300">{entry.content}</p>
                  <div className="flex flex-wrap items-center gap-3 text-xs text-slate-500 dark:text-slate-400">
                    <span className="rounded-full bg-slate-100 px-3 py-1 text-slate-700 dark:bg-slate-800 dark:text-slate-200">
                      {entry.privacy || 'Public'}
                    </span>
                    <span>{entry.likeCount} likes</span>
                    <span>{formatTimestamp(entry.createdAt)}</span>
                  </div>
                </div>
              );
            })
          )}
        </div>
        {postsHasMore && !postsLoading && (
          <div className="border-t border-white/60 px-6 py-4 text-center dark:border-slate-800/70">
            <button
              onClick={() => handleLoadMorePosts()}
              className="rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
              type="button"
            >
              Load more
            </button>
          </div>
        )}
      </div>

      <div className="rounded-2xl border border-white/60 bg-white/80 shadow-[0_16px_40px_rgba(15,23,42,0.12)] dark:border-slate-800/70 dark:bg-slate-900/70">
        <div className="flex flex-col gap-4 border-b border-white/60 px-6 py-4 dark:border-slate-800/70 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-100">Reported posts</h2>
            <p className="text-sm text-slate-500 dark:text-slate-400">Investigate reports before taking action.</p>
          </div>
          <button
            onClick={() => loadReports(1, false)}
            className="inline-flex items-center gap-2 rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
            type="button"
          >
            <RefreshCw className="h-4 w-4" />
            Refresh reports
          </button>
        </div>
        <div className="divide-y divide-white/70 dark:divide-slate-800/70">
          {reportsLoading ? (
            <div className="px-6 py-8 text-sm text-slate-500 dark:text-slate-400">Loading reports...</div>
          ) : reports.length === 0 ? (
            <div className="px-6 py-8 text-sm text-slate-500 dark:text-slate-400">No reports pending.</div>
          ) : (
            reports.map((report) => {
              const reporterName =
                [report.reporterFirstName, report.reporterLastName].filter(Boolean).join(' ') ||
                report.reporterUserName ||
                'Unknown reporter';
              const authorName =
                [report.postAuthorFirstName, report.postAuthorLastName].filter(Boolean).join(' ') ||
                report.postAuthorUserName ||
                'Unknown author';

              return (
                <div key={report.postReportId} className="flex flex-col gap-4 px-6 py-5">
                  <div className="flex flex-wrap items-center justify-between gap-3">
                    <div className="flex items-center gap-3">
                      <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-rose-500/10 text-rose-600 dark:text-rose-400">
                        <Flag className="h-5 w-5" />
                      </div>
                      <div>
                        <p className="text-sm font-semibold text-slate-900 dark:text-slate-100">{authorName}</p>
                        <p className="text-xs text-slate-500 dark:text-slate-400">Reported by {reporterName}</p>
                      </div>
                    </div>
                    <div className="flex flex-wrap items-center gap-2">
                      <button
                        onClick={() => handleReviewReport(report.postReportId)}
                        disabled={reviewingReportId === report.postReportId}
                        className="inline-flex items-center gap-2 rounded-xl border border-white/70 bg-white/80 px-3 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white disabled:cursor-not-allowed disabled:opacity-60 dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
                        type="button"
                      >
                        {reviewingReportId === report.postReportId ? 'Updating...' : 'Dismiss'}
                      </button>
                      <button
                        onClick={() => handleDeleteReportedPost(report)}
                        disabled={deletingPostId === report.postId}
                        className="inline-flex items-center gap-2 rounded-xl bg-rose-500 px-3 py-2 text-xs font-semibold text-white transition-colors hover:bg-rose-600 disabled:cursor-not-allowed disabled:bg-slate-300 disabled:text-slate-600"
                        type="button"
                      >
                        <Trash2 className="h-4 w-4" />
                        {deletingPostId === report.postId ? 'Deleting...' : 'Delete post'}
                      </button>
                    </div>
                  </div>
                  <div className="space-y-2 text-sm text-slate-600 dark:text-slate-300">
                    <p>{report.postContent}</p>
                    <div className="rounded-xl border border-amber-200/70 bg-amber-50/70 px-3 py-2 text-xs text-amber-700 dark:border-amber-500/30 dark:bg-amber-500/10 dark:text-amber-200">
                      <p className="font-semibold">Reason</p>
                      <p>{report.reason || 'No reason provided.'}</p>
                      {report.description && (
                        <p className="mt-2 text-amber-600 dark:text-amber-100">{report.description}</p>
                      )}
                    </div>
                  </div>
                  <div className="flex flex-wrap items-center gap-3 text-xs text-slate-500 dark:text-slate-400">
                    <span className="rounded-full bg-slate-100 px-3 py-1 text-slate-700 dark:bg-slate-800 dark:text-slate-200">
                      {report.postPrivacy || 'Public'}
                    </span>
                    <span>Reported {formatTimestamp(report.createdAt)}</span>
                    <span>Posted {formatTimestamp(report.postCreatedAt)}</span>
                  </div>
                </div>
              );
            })
          )}
        </div>
        {reportsHasMore && !reportsLoading && (
          <div className="border-t border-white/60 px-6 py-4 text-center dark:border-slate-800/70">
            <button
              onClick={() => handleLoadMoreReports()}
              className="rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-700 shadow-sm shadow-slate-200/40 transition-colors hover:bg-white dark:border-slate-800/70 dark:bg-slate-900/70 dark:text-slate-200 dark:hover:bg-slate-900"
              type="button"
            >
              Load more
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default AdminDashboard;
